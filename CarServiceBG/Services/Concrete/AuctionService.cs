using AutoMapper;
using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using CarServiceBG.DTOs;
using CarServiceBG.Hubs;
using CarServiceBG.Services.Abstract;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CarServiceBG.Services.Concrete
{
    public class AuctionService : IAuctionService
    {
        private readonly CarServiceDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IHubContext<AuctionHub> _hub;


        public AuctionService(CarServiceDbContext context, IMapper mapper, IPhotoService photoService, IHubContext<AuctionHub> hub)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
            _hub = hub;
        }

        public async Task<bool> CreateProductAsync(AuctionProductCreateDto dto, string sellerId)
        {
            var photoUrl = await _photoService.UploadImageAsync(new PhotoCreateDto { File = dto.Photo });
            if (!Guid.TryParse(sellerId, out Guid parsedSellerId)) return false;

            var startTime = DateTime.UtcNow.AddMinutes(1);


            var product = new AuctionProduct
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                StartPrice = dto.StartPrice,
                CurrentPrice = dto.StartPrice,
                PhotoUrl = photoUrl,
                Status = "Scheduled",
                SellerId = parsedSellerId,
                StartTime = DateTime.UtcNow.AddMinutes(1),
                EndTime = null
            };

            _context.AuctionProducts.Add(product);
            await _context.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("AuctionStartingSoon", $"{product.Title} auction will start in 1 minute.");

            return true;
        }


        public async Task HandleAuctionStartAsync(Guid productId)
        {
            var product = await _context.AuctionProducts.FindAsync(productId);
            if (product == null) return;

            // Bildirim gönder
            await _hub.Clients.All.SendAsync("AuctionStartingSoon", $"{product.Title} auction will start in 1 minute.");

            // 1 dakika bekle
            await Task.Delay(TimeSpan.FromMinutes(1));

            // Aktif hale getir
            product.Status = "Active";
            await _context.SaveChangesAsync();

            // Auction bitişine kadar bekle ve sonra işle
            if (product.EndTime.HasValue)
            {
                var remainingTime = product.EndTime.Value - DateTime.UtcNow;
                if (remainingTime > TimeSpan.Zero)
                {
                    await Task.Delay(remainingTime);
                    await HandleAuctionEndAsync(productId);
                }
                else
                {
                    await HandleAuctionEndAsync(productId);
                }
            }
            else
            {
                await HandleAuctionEndAsync(productId);
            }
        }

        public async Task HandleAuctionEndAsync(Guid productId)
        {
            var product = await _context.AuctionProducts.Include(p => p.Bids).FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null || product.Status == "Ended")
                return;

            product.Status = "Ended";
            await _context.SaveChangesAsync();

            var winningBid = product.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();

            if (winningBid != null)
            {
                var user = await _context.Users.FindAsync(winningBid.UserId);
                string winnerName = user?.FullName ?? "Unknown";

                await _hub.Clients.All.SendAsync("AuctionEnded", $"Auction for '{product.Title}' has ended. Winner: {winnerName}.");
            }
            else
            {
                await _hub.Clients.All.SendAsync("AuctionEnded", $"Auction for '{product.Title}' has ended. No bids were placed.");
            }
        }


        public async Task<bool> PlaceBidAsync(AuctionBidDto dto)
        {
            var product = await _context.AuctionProducts
                .Include(p => p.Bids)
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId);

            if (product == null || product.Status != "Active" || dto.Amount <= product.CurrentPrice)
                return false;

            // aynı miktara aynı kullanıcıdan bid engeli (opsiyonel)
            if (product.Bids.Any(x => x.Amount == dto.Amount && x.UserId == dto.UserId))
                return false;

            product.CurrentPrice = dto.Amount;
            product.EndTime = DateTime.UtcNow.AddSeconds(60);

            _context.AuctionBids.Add(new AuctionBid
            {
                Id = Guid.NewGuid(),
                Amount = dto.Amount,
                ProductId = dto.ProductId,
                UserId = dto.UserId,
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(); // önce DB, sonra yayın — tutarlılık için

            await _hub.Clients.All.SendAsync("ReceiveBidUpdate",
                dto.ProductId, dto.Amount, dto.UserId, product.EndTime);

            return true;
        }


        public async Task EndAuctionAsync(Guid productId)
        {
            var product = await _context.AuctionProducts
                .Include(p => p.Bids)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null || product.Status == "Ended") return;

            product.Status = "Ended";
            await _context.SaveChangesAsync();

            var win = product.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();
            if (win != null)
            {
                var user = await _context.Users.FindAsync(win.UserId);
                var name = user?.FullName ?? "Unknown";
                await _hub.Clients.All.SendAsync("AuctionWinner", productId, name, win.Amount);
            }
            else
            {
                await _hub.Clients.All.SendAsync("AuctionEnded",
                    $"Auction for '{product.Title}' has ended. No bids were placed.");
            }

        }




        public async Task<List<AuctionProductResponseDto>> GetAllVisibleProductsAsync()
        {
            var items = await _context.AuctionProducts
                .OrderByDescending(p => p.Status == "Active")
                .ThenByDescending(p => p.Status == "Scheduled")
                .ThenByDescending(p => p.StartTime) // en yeni üstte dursun
                .ToListAsync();

            return _mapper.Map<List<AuctionProductResponseDto>>(items);
        }


        private async Task<string?> UploadPhoto(IFormFile? file)
        {
            if (file == null) return null;
            var path = Path.Combine("wwwroot/uploads", Guid.NewGuid() + Path.GetExtension(file.FileName));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            return "/uploads/" + Path.GetFileName(path);
        }


        public async Task<List<AuctionProductResponseDto>> GetAllActiveProductsAsync()
        {
            var products = await _context.AuctionProducts
                .ToListAsync();

            return _mapper.Map<List<AuctionProductResponseDto>>(products);
        }

        public async Task<AuctionProductResponseDto?> GetByIdAsync(Guid productId)
        {
            var product = await _context.AuctionProducts.FindAsync(productId);
            return product == null ? null : _mapper.Map<AuctionProductResponseDto>(product);
        }

        public async Task<bool> DeleteAuctionAsync(Guid auctionId)
        {
            // Zamanlayıcı varsa durdur

            // Eğer aktifse önce bitir (opsiyonel ama iyi pratik)
            var auction = await _context.AuctionProducts.FirstOrDefaultAsync(x => x.Id == auctionId);
            if (auction == null) return false;

            if (auction.Status == "Active")
            {
                await EndAuctionAsync(auctionId); // kazananı duyurur ve Ended yapar
                                                  // yeniden oku çünkü state değişti
                auction = await _context.AuctionProducts.FirstOrDefaultAsync(x => x.Id == auctionId);
                if (auction == null) return false;
            }

            // Transaction içinde bağlı kayıtları sil
            using var tx = await _context.Database.BeginTransactionAsync();

            var bids = await _context.AuctionBids
                .Where(b => b.ProductId == auctionId)
                .ToListAsync();
            _context.AuctionBids.RemoveRange(bids);

            var txs = await _context.AuctionTransactions
                .Where(t => t.AuctionProductId == auctionId)
                .ToListAsync();
            _context.AuctionTransactions.RemoveRange(txs);

            _context.AuctionProducts.Remove(auction);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return true;
        }


        // AuctionService.cs
        public async Task CheckAndActivateAuctionsAsync()
        {
            var now = DateTime.UtcNow;
            var scheduled = await _context.AuctionProducts
                .Where(x => x.Status == "Scheduled" && x.StartTime <= now)
                .ToListAsync();
            foreach (var product in scheduled)
            {
                product.Status = "Active";
                product.EndTime = now.AddSeconds(60);
                await _context.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("AuctionStarted", product.Id, product.Title);
            }
        }

        public async Task CheckAndEndAuctionsAsync()
        {
            var now = DateTime.UtcNow;
            var active = await _context.AuctionProducts
                .Include(p => p.Bids)
                .Where(x => x.Status == "Active" && x.EndTime <= now)
                .ToListAsync();
            foreach (var product in active)
            {
                product.Status = "Ended";
                await _context.SaveChangesAsync();

                var winningBid = product.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();
                if (winningBid != null)
                {
                    var user = await _context.Users.FindAsync(winningBid.UserId);
                    string winnerName = user?.FullName ?? "Unknown";
                    await _hub.Clients.All.SendAsync("AuctionWinner", product.Id, winnerName, winningBid.Amount);
                }
                else
                {
                    await _hub.Clients.All.SendAsync("AuctionEnded", $"Auction for '{product.Title}' has ended. No bids were placed.");
                }
            }
        }


        public async Task<List<AuctionProductResponseDto>> GetPendingAndActiveAuctionsAsync()
        {
            var auctions = await _context.AuctionProducts
                .Where(x => x.Status == "Scheduled" || x.Status == "Active")
                .ToListAsync();

            return _mapper.Map<List<AuctionProductResponseDto>>(auctions);
        }

        public async Task ActivateAuctionAsync(Guid auctionId)
        {
            var product = await _context.AuctionProducts.FirstOrDefaultAsync(x => x.Id == auctionId);
            if (product == null || product.Status == "Active") return;

            product.Status = "Active";
            product.EndTime = DateTime.UtcNow.AddSeconds(60);

            await _context.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("AuctionStarted", product.Id, product.Title);
        }




    }
}
