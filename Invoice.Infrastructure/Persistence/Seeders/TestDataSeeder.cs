using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Infrastructure.Persistence.Seeders;

public static class TestDataSeeder
{
    public static async Task SeedSampleDataAsync(ApplicationDbContext context, UserManager<User> userManager)
    {
        // find admin and regular user
        var admin = await userManager.FindByEmailAsync("admin@Invoice.com");
        var user = await userManager.FindByEmailAsync("user@Invoice.com");
        if (admin == null || user == null) return;

        // Organization
        var orgName = "Default Organization";
        var org = await context.Organizations.FirstOrDefaultAsync(o => o.OrganizationName == orgName);
        if (org == null)
        {
            org = new Organization
            {
                OrganizationName = orgName,
                OrganizationEmail = "info@default.org",
                OrganizationPhone = "+84-900000000",
                OrganizationAddress = "Headquarter",
                // Set owner reference to existing admin user to satisfy FK
                UserId = admin.Id,
                CreatedDate = DateTime.UtcNow
            };
            await context.Organizations.AddAsync(org);
            await context.SaveChangesAsync();
        }

        // ApiKey
        var existingKey = await context.ApiKeys.FirstOrDefaultAsync(k => k.OrganizationId == org.Id);
        if (existingKey == null)
        {
            var apiKey = new ApiKey
            {
                OrganizationId = org.Id,
                KeyHash = Guid.NewGuid().ToString("N"),
                Name = "DefaultApiKey",
                Active = true,
                CreatedDate = DateTime.UtcNow
            };
            await context.ApiKeys.AddAsync(apiKey);
            await context.SaveChangesAsync();
        }

        // Invoice with lines
        var invoiceNumber = "INV-1001";
        var existingInvoice = await context.Invoices.Include(i => i.Lines).FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
        Invoice.Domain.Entities.Invoice newInvoice;
        if (existingInvoice == null)
        {
            var invoice = new Invoice.Domain.Entities.Invoice
            {
                InvoiceNumber = invoiceNumber,
                FormNumber = "F-001",
                Serial = "S-01",
                OrganizationId = org.Id,
                IssuedByUserId = user.Id,
                SellerName = "Seller Co",
                SellerTaxId = "TAX123",
                CustomerName = "Test Customer",
                CustomerTaxId = "CUST123",
                Status = InvoiceStatus.Uploaded,
                IssuedDate = DateTime.UtcNow,
                SubTotal = 100m,
                TaxAmount = 10m,
                DiscountAmount = 0m,
                TotalAmount = 110m,
                Currency = "VND",
                Note = "Seeded invoice",
                CreatedDate = DateTime.UtcNow
            };

            invoice.Lines.Add(new InvoiceLine
            {
                LineNumber = 1,
                Description = "Product A",
                Quantity = 2m,
                UnitPrice = 30m,
                LineTotal = 60m,
                TaxAmount = 6m,
                CreatedDate = DateTime.UtcNow
            });

            invoice.Lines.Add(new InvoiceLine
            {
                LineNumber = 2,
                Description = "Product B",
                Quantity = 1m,
                UnitPrice = 40m,
                LineTotal = 40m,
                TaxAmount = 4m,
                CreatedDate = DateTime.UtcNow
            });

            await context.Invoices.AddAsync(invoice);
            await context.SaveChangesAsync();
            newInvoice = invoice;
        }
        else
        {
            newInvoice = existingInvoice;
        }

        // InvoiceBatch
        var batchExternalId = "BATCH-1";
        var existingBatch = await context.InvoiceBatches.FirstOrDefaultAsync(b => b.BatchId == batchExternalId);
        if (existingBatch == null)
        {
            var batch = new InvoiceBatch
            {
                BatchId = batchExternalId,
                Count = 1,
                MerkleRoot = "root",
                Status = BatchStatus.Initial,
                CreatedDate = DateTime.UtcNow
            };

            await context.InvoiceBatches.AddAsync(batch);
            await context.SaveChangesAsync();

            // assign invoice to batch
            newInvoice.BatchId = batch.Id;
            newInvoice.Status = InvoiceStatus.Batched;
            newInvoice.UpdatedDate = DateTime.UtcNow;

            context.Invoices.Update(newInvoice);
            await context.SaveChangesAsync();
        }
    }
}
