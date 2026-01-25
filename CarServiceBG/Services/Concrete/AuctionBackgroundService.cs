using CarServiceBG.Services.Abstract;

namespace CarServiceBG.Services
{
    public class AuctionBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuctionBackgroundService> _logger;

        public AuctionBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<AuctionBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuctionBackgroundService started at {Time}", DateTime.UtcNow);

            // Başlanğıcda mövcud auction-ları yoxla və aktiv et
            await InitializeExistingAuctions(stoppingToken);

            // Hər 5 saniyədə bir yoxla
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAuctions(stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("AuctionBackgroundService is stopping.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in auction background service");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }

        private async Task InitializeExistingAuctions(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();

            try
            {
                // Mövcud pending və active auction-ları yüklə
                var auctions = await auctionService.GetPendingAndActiveAuctionsAsync();
                _logger.LogInformation("Found {Count} pending/active auctions on startup", auctions.Count);

                var now = DateTime.UtcNow;

                foreach (var auction in auctions)
                {
                    // Scheduled amma vaxtı keçmiş olanları aktivləşdir
                    if (auction.Status == "Scheduled" && auction.StartTime <= now)
                    {
                        _logger.LogInformation("Activating scheduled auction {Id} - {Title}", auction.Id, auction.Title);
                        await auctionService.ActivateAuctionAsync(auction.Id);
                    }
                    // Active amma vaxtı bitmiş olanları bitir
                    else if (auction.Status == "Active" && auction.EndTime.HasValue && auction.EndTime <= now)
                    {
                        _logger.LogInformation("Ending expired active auction {Id} - {Title}", auction.Id, auction.Title);
                        await auctionService.EndAuctionAsync(auction.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing existing auctions");
            }
        }

        private async Task CheckAuctions(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();

            try
            {
                // Scheduled-ları aktivləşdir
                await auctionService.CheckAndActivateAuctionsAsync();

                // Active olanları bitir
                await auctionService.CheckAndEndAuctionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking auctions");
            }
        }
    }
}