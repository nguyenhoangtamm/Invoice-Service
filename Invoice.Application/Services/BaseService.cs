using AutoMapper;
using Invoice.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Invoice.Application.Services;

public abstract class BaseService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILogger<BaseService> _logger;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IMapper _mapper;

    protected BaseService(IHttpContextAccessor httpContextAccessor, ILogger<BaseService> logger,
         IUnitOfWork unitOfWork, IMapper mapper)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    protected IHttpContextAccessor HttpContextAccessor => _httpContextAccessor;

    protected string? ProfileCode
    {
        get { return _httpContextAccessor.HttpContext?.Items["ProfileCode"]?.ToString(); }
    }

    protected string? UserName
    {
        get 
        { 
            // L?y t? JWT claims tr??c
            var username = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            if (!string.IsNullOrEmpty(username))
                return username;
                
            // Fallback v? Items n?u có
            return _httpContextAccessor.HttpContext?.Items["UserName"]?.ToString(); 
        }
    }

    protected string? UserId
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }

    protected string? UserEmail
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }

    protected List<string> Roles
    {
        get
        {
            try
            {
                // L?y t? JWT claims tr??c
                var roles = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)?
                    .Select(c => c.Value).ToList();
                if (roles != null && roles.Any())
                    return roles;
                    
                // Fallback v? Items n?u có
                return _httpContextAccessor.HttpContext?.Items["Roles"] as List<string> ?? new List<string>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new List<string>();
            }
        }
    }

    protected void LogInformation(string message)
    {
        _logger.LogInformation(message);
    }

    protected void LogError(string message, Exception ex)
    {
        _logger.LogError(ex, message);
    }
}

