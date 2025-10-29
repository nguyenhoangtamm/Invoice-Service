using Invoice.Domain.Common.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Invoice.Infrastructure.Services;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(24); // Run cleanup every 24 hours

    public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
                    await jwtService.CleanupExpiredTokensAsync();
                    _logger.LogInformation("Token cleanup completed at {Time}", DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token cleanup");
            }

            await Task.Delay(_period, stoppingToken);
        }
    }
}

