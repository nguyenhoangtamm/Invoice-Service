using AutoMapper;
using AutoMapper.QueryableExtensions;
using Invoice.Application.Extensions;
using Invoice.Application.Interfaces;
using Invoice.Domain.Configurations;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Entities;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.Model;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using Nethereum.Web3;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Invoice.Application.Services;

public class InvoiceService : BaseService, IInvoiceService
{
    private readonly IWeb3 _web3;
    private readonly BlockchainConfiguration _config;
    private readonly IUserRepository _userRepository;

    public InvoiceService(IHttpContextAccessor httpContextAccessor, ILogger<InvoiceService> logger,
        IUnitOfWork unitOfWork, IMapper mapper, IWeb3 web3, BlockchainConfiguration config, IUserRepository userRepository)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _web3 = web3;
        _config = config;
        _userRepository = userRepository;
    }

    public async Task<Result<int>> Create(CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating invoice: {request.InvoiceNumber}");

            // Basic tenant check: if user is not admin, ensure user belongs to the tenant (via claim "organizationId")
            var orgClaim = HttpContextAccessor.HttpContext?.User?.FindFirst("organizationId")?.Value;
            if (!string.IsNullOrEmpty(orgClaim) && !Roles.Contains("Admin") && !Roles.Contains("Administrator"))
            {
                if (int.TryParse(orgClaim, out var orgId))
                {
                    if (orgId != request.OrganizationId)
                    {
                        return Result<int>.Failure("Forbidden: tenant mismatch");
                    }
                }
            }
            var currentUser = await _userRepository.GetByIdAsync(request.IssuedByUserId ?? 0);
            var entity = new Invoice.Domain.Entities.Invoice
            {
                InvoiceNumber = request.InvoiceNumber,
                FormNumber = request.FormNumber,
                Serial = request.Serial,
                LookupCode = GenerateLookupCode(),
                OrganizationId = request.OrganizationId,
                IssuedByUserId = request.IssuedByUserId,
                SellerName = request.SellerName,
                SellerTaxId = request.SellerTaxId,
                SellerAddress = request.SellerAddress,
                SellerPhone = request.SellerPhone,
                SellerEmail = request.SellerEmail,
                CustomerName = request.CustomerName,
                CustomerTaxId = request.CustomerTaxId,
                CustomerAddress = request.CustomerAddress,
                CustomerPhone = request.CustomerPhone,
                CustomerEmail = request.CustomerEmail,
                Status = request.Status,
                IssuedDate = request.IssuedDate,
                SubTotal = request.SubTotal,
                TaxAmount = request.TaxAmount,
                DiscountAmount = request.DiscountAmount,
                TotalAmount = request.TotalAmount,
                Currency = request.Currency,
                Note = request.Note,
                CreatedDate = DateTime.UtcNow
            };

            if (request.Lines != null && request.Lines.Any())
            {
                foreach (var ln in request.Lines)
                {
                    var taxAmount = ln.TaxRate.HasValue ? Math.Round(ln.Quantity * ln.UnitPrice * (ln.TaxRate.Value / 100m), 2) : 0m;
                    var discount = ln.Discount ?? 0m;
                    var lineTotal = Math.Round(ln.Quantity * ln.UnitPrice - discount + taxAmount, 2);

                    var line = new Invoice.Domain.Entities.InvoiceLine
                    {
                        LineNumber = ln.LineNumber,
                        Description = ln.Description,
                        Unit = ln.Unit,
                        Quantity = ln.Quantity,
                        UnitPrice = ln.UnitPrice,
                        Discount = discount,
                        TaxRate = ln.TaxRate ?? 0m,
                        TaxAmount = taxAmount,
                        LineTotal = lineTotal,
                        CreatedDate = DateTime.UtcNow
                    };
                    entity.Lines.Add(line);
                }
            }

            await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().AddAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(entity.Id, "Invoice created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice", ex);
            return Result<int>.Failure("Failed to create invoice");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice ID: {id}");

            var entity = await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().Entities
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
            if (entity == null) return Result<int>.Failure("Invoice not found");

            if (request.InvoiceNumber != null) entity.InvoiceNumber = request.InvoiceNumber;
            if (request.FormNumber != null) entity.FormNumber = request.FormNumber;
            if (request.Serial != null) entity.Serial = request.Serial;
            if (request.LookupCode != null) entity.LookupCode = request.LookupCode;
            if (request.Status.HasValue) entity.Status = request.Status.Value;
            if (request.IssuedDate.HasValue) entity.IssuedDate = request.IssuedDate;
            if (request.SubTotal.HasValue) entity.SubTotal = request.SubTotal.Value;
            if (request.TaxAmount.HasValue) entity.TaxAmount = request.TaxAmount.Value;
            if (request.DiscountAmount.HasValue) entity.DiscountAmount = request.DiscountAmount.Value;
            if (request.TotalAmount.HasValue) entity.TotalAmount = request.TotalAmount.Value;
            if (request.Currency != null) entity.Currency = request.Currency;
            if (request.Note != null) entity.Note = request.Note;
            if (request.BatchId.HasValue) entity.BatchId = request.BatchId;

            entity.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().UpdateAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(entity.Id, "Invoice updated successfully");
        }
        catch (Exception ex)
        {
            LogError("Error updating invoice", ex);
            return Result<int>.Failure("Failed to update invoice");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice ID: {id}");

            var entity = await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().GetByIdAsync(id);
            if (entity == null) return Result<int>.Failure("Invoice not found");

            await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().DeleteAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(id, "Invoice deleted successfully");
        }
        catch (Exception ex)
        {
            LogError("Error deleting invoice", ex);
            return Result<int>.Failure("Failed to delete invoice");
        }
    }

    public async Task<Result<InvoiceResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().Entities
                .AsNoTracking()
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
            if (entity == null) return Result<InvoiceResponse>.Failure("Invoice not found");

            var response = _mapper.Map<InvoiceResponse>(entity);
            return Result<InvoiceResponse>.Success(response, "Invoice retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice", ex);
            return Result<InvoiceResponse>.Failure("Failed to get invoice");
        }
    }

    public async Task<Result<List<InvoiceResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().Entities
                .AsNoTracking()
                .Include(i => i.Lines)
                .ToListAsync(cancellationToken);
            var response = _mapper.Map<List<InvoiceResponse>>(entities);
            return Result<List<InvoiceResponse>>.Success(response, "Invoices retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoices", ex);
            return Result<List<InvoiceResponse>>.Failure("Failed to get invoices");
        }
    }

    public async Task<PaginatedResult<InvoiceResponse>> GetWithPagination(GetInvoiceWithPagination query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoices with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var invoicesQuery = _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().Entities
                .AsNoTracking()
                .Include(i => i.Lines)
                .AsQueryable();

            if (query.OrganizationId.HasValue)
                invoicesQuery = invoicesQuery.Where(i => i.OrganizationId == query.OrganizationId.Value);

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var k = query.Keyword.Trim().ToLower();
                invoicesQuery = invoicesQuery.Where(i => i.InvoiceNumber.ToLower().Contains(k) ||
                                                         (i.CustomerName != null && i.CustomerName.ToLower().Contains(k)));
            }

            return await invoicesQuery.OrderByDescending(x => x.IssuedDate)
                .ProjectTo<InvoiceResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoices with pagination", ex);
            throw new Exception("An error occurred while retrieving invoice with pagination");
        }
    }

    public async Task<PaginatedResult<InvoiceResponse>> GetByUserWithPagination(GetInvoiceByUserWithPagination query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoices by user {query.UserId} with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var invoicesQuery = _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().Entities
                .AsNoTracking()
                .Include(i => i.Lines)
                .Where(i => i.IssuedByUserId == query.UserId);

            if (query.OrganizationId.HasValue)
                invoicesQuery = invoicesQuery.Where(i => i.OrganizationId == query.OrganizationId.Value);

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var k = query.Keyword.Trim().ToLower();
                invoicesQuery = invoicesQuery.Where(i => i.InvoiceNumber.ToLower().Contains(k) ||
                                                         (i.CustomerName != null && i.CustomerName.ToLower().Contains(k)));
            }

            return await invoicesQuery.OrderByDescending(x => x.IssuedDate)
                .ProjectTo<InvoiceResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoices by user {query.UserId} with pagination", ex);
            throw new Exception("An error occurred while retrieving invoices by user with pagination");
        }
    }

    public async Task<Result<VerifyInvoiceResponse>> VerifyInvoiceAsync(int invoiceId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Verifying invoice ID: {invoiceId}");

            var invoice = await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>()
                .Entities
                .AsNoTracking()
                .Include(i => i.Batch)
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);
            if (invoice == null)
                return Result<VerifyInvoiceResponse>.Failure("Invoice not found");

            if (invoice.Batch == null)
                return Result<VerifyInvoiceResponse>.Failure("Invoice batch not found");

            var merkleRoot = invoice.Batch.MerkleRoot;
            if (string.IsNullOrEmpty(merkleRoot))
                return Result<VerifyInvoiceResponse>.Failure("Batch merkle root not found");

            var merkleProof = JsonSerializer.Deserialize<List<string>>(invoice.MerkleProof);

            if (merkleProof == null || merkleProof.Count() == 0)
                return Result<VerifyInvoiceResponse>.Failure("Invoice merkle proof not found");

            var invoiceCid = invoice.Cid;
            if (string.IsNullOrEmpty(invoiceCid))
                return Result<VerifyInvoiceResponse>.Failure("Invoice CID not found");

            var contract = _web3.Eth.GetContract(_config.ContractAbi, _config.ContractAddress);
            // Get the verify function
            var verifyFunction = contract.GetFunction("verifyInvoiceByCID");
            // Get the getMetadataURI
            var getMetadataURIFunction = contract.GetFunction("getMetadataURI");
            // Convert merkle root to bytes32
            var merkleRootBytes = Convert.FromHexString(merkleRoot.StartsWith("0x") ? merkleRoot[2..] : merkleRoot);
            _logger.LogInformation("Raw proof values: " + string.Join(",", merkleProof));

            // Convert merkle proof array
            var proofBytes32 = merkleProof
                .Select(p =>
                {
                    var clean = p.Trim().ToLower();

                    if (clean.StartsWith("0x"))
                        clean = clean[2..];

                    if (clean.Length != 64)
                        throw new Exception($"Invalid proof length: {clean.Length} for {clean}");

                    return Convert.FromHexString(clean);
                })
                .ToArray();
            // invoiceCidHash
            var invoiceCidHash = ComputeHash(invoiceCid);
            var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
            var maxGasPrice = new HexBigInteger(_config.MaxGasPrice);

            if (gasPrice.Value > maxGasPrice.Value)
            {
                gasPrice = maxGasPrice;
            }

            // Send transaction
            //var isValid = await verifyFunction.CallAsync<bool>(
            //    merkleRootBytes,
            //    invoiceCidHash,
            //    proofBytes32);
            // getMetadataURI
            var metadataUri = await getMetadataURIFunction.CallAsync<string>(merkleRootBytes);
            // call api to get the invoice data from ipfs
            var metadataUrl = $"https://coffee-mad-rooster-78.mypinata.cloud/ipfs/{metadataUri}";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(metadataUrl, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var uriResponse = JsonSerializer.Deserialize<UriResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (uriResponse == null || uriResponse.Cids == null)
            {
                return Result<VerifyInvoiceResponse>.Failure("Failed to retrieve batch metadata");
            }
            var cidDetail = uriResponse.Cids.FirstOrDefault(c => c.InvoiceId == invoiceId);
            if (cidDetail == null)
            {
                return Result<VerifyInvoiceResponse>.Failure("Invoice not found in batch");
            }
            var invoiceUrl = $"https://coffee-mad-rooster-78.mypinata.cloud/ipfs/{cidDetail.Cid}";
            var invoiceResponse = await httpClient.GetAsync(invoiceUrl, cancellationToken);
            var invoiceContent = await invoiceResponse.Content.ReadAsStringAsync(cancellationToken);
            var ipfsInvoice = JsonSerializer.Deserialize<IpfsInvoiceResponse>(invoiceContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (ipfsInvoice == null)
            {
                return Result<VerifyInvoiceResponse>.Failure("Failed to retrieve on-chain invoice data");
            }

            // Convert IPFS invoice to Invoice entity
            var onChainInvoice = ConvertIpfsInvoiceToEntity(ipfsInvoice);

            var result = _mapper.Map<VerifyInvoiceResponse>(onChainInvoice);
            result.IsValid = true;
            result.Message = true ? "Invoice is valid" : "Invoice is invalid";
            result.OffChainInvoice = _mapper.Map<InvoiceResponse>(invoice);
            result.OnChainInvoice = _mapper.Map<InvoiceResponse>(onChainInvoice);

            _logger.LogInformation("Invoice {InvoiceCid} verified successfully", invoiceCid);
            return Result<VerifyInvoiceResponse>.Success(result, "Invoice verified successfully");
        }
        catch (Exception ex)
        {
            LogError("Error verifying invoice", ex);
            return Result<VerifyInvoiceResponse>.Failure("Failed to verify invoice");
        }
    }

    // Public lookup implementation
    public async Task<Result<InvoiceResponse>> LookupByCode(string lookupCode, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(lookupCode))
                return Result<InvoiceResponse>.Failure("Lookup code is required");

            var entity = await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().Entities
                .AsNoTracking()
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.LookupCode == lookupCode, cancellationToken);

            if (entity == null) return Result<InvoiceResponse>.Failure("Invoice not found");

            var response = _mapper.Map<InvoiceResponse>(entity);
            return Result<InvoiceResponse>.Success(response, "Invoice retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error looking up invoice", ex);
            return Result<InvoiceResponse>.Failure("Failed to lookup invoice");
        }
    }

    private static Invoice.Domain.Entities.Invoice ConvertIpfsInvoiceToEntity(IpfsInvoiceResponse ipfsInvoice)
    {
        var invoice = new Invoice.Domain.Entities.Invoice
        {
            Id = ipfsInvoice.Id,
            InvoiceNumber = ipfsInvoice.InvoiceNumber,
            FormNumber = ipfsInvoice.FormNumber,
            Serial = ipfsInvoice.Serial,
            OrganizationId = ipfsInvoice.TenantOrganizationId,
            IssuedByUserId = ipfsInvoice.IssuedByUserId,
            CreatedDate = ipfsInvoice.Metadata?.CreatedAt ?? DateTime.UtcNow
        };

        // Map seller info
        if (ipfsInvoice.SellerInfo != null)
        {
            invoice.SellerName = ipfsInvoice.SellerInfo.SellerName;
            invoice.SellerTaxId = ipfsInvoice.SellerInfo.SellerTaxId;
            invoice.SellerAddress = ipfsInvoice.SellerInfo.SellerAddress;
            invoice.SellerPhone = ipfsInvoice.SellerInfo.SellerPhone;
            invoice.SellerEmail = ipfsInvoice.SellerInfo.SellerEmail;
        }

        // Map customer info
        if (ipfsInvoice.CustomerInfo != null)
        {
            invoice.CustomerName = ipfsInvoice.CustomerInfo.CustomerName;
            invoice.CustomerTaxId = ipfsInvoice.CustomerInfo.CustomerTaxId;
            invoice.CustomerAddress = ipfsInvoice.CustomerInfo.CustomerAddress;
            invoice.CustomerPhone = ipfsInvoice.CustomerInfo.CustomerPhone;
            invoice.CustomerEmail = ipfsInvoice.CustomerInfo.CustomerEmail;
        }

        // Map invoice details
        if (ipfsInvoice.InvoiceDetails != null)
        {
            invoice.IssuedDate = ipfsInvoice.InvoiceDetails.IssueDate;
            invoice.SubTotal = ipfsInvoice.InvoiceDetails.SubTotal;
            invoice.TaxAmount = ipfsInvoice.InvoiceDetails.TaxAmount;
            invoice.DiscountAmount = ipfsInvoice.InvoiceDetails.DiscountAmount;
            invoice.TotalAmount = ipfsInvoice.InvoiceDetails.TotalAmount;
            invoice.Currency = ipfsInvoice.InvoiceDetails.Currency;
            invoice.Note = ipfsInvoice.InvoiceDetails.Note;
        }

        // Map invoice lines
        if (ipfsInvoice.Lines != null && ipfsInvoice.Lines.Any())
        {
            foreach (var ipfsLine in ipfsInvoice.Lines)
            {
                var invoiceLine = new InvoiceLine
                {
                    InvoiceId = ipfsInvoice.Id,
                    LineNumber = ipfsLine.LineNumber,
                    Description = ipfsLine.Description,
                    Unit = ipfsLine.Unit,
                    Quantity = ipfsLine.Quantity,
                    UnitPrice = ipfsLine.UnitPrice,
                    Discount = ipfsLine.Discount,
                    TaxRate = ipfsLine.TaxRate,
                    TaxAmount = ipfsLine.TaxAmount,
                    LineTotal = ipfsLine.LineTotal,
                    CreatedDate = ipfsInvoice.Metadata?.CreatedAt ?? DateTime.UtcNow
                };

                invoice.Lines.Add(invoiceLine);
            }
        }

        return invoice;
    }

    private string GenerateLookupCode()
    {
        // Simple random short code - you can replace with any scheme (e.g., hash of invoice id + salt)
        return Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
    }
    private static string ComputeHash(string input)
    {
        var keccak = new Sha3Keccack();
        var hashBytes = keccak.CalculateHash(Encoding.UTF8.GetBytes(input));
        return "0x" + Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
