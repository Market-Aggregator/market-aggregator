
using MarketOverviewService.Core.Entities;
using MarketOverviewService.Core.Interfaces;
using MarketOverviewService.Infrastructure.Persistence.Data;

namespace MarketOverviewService.Infrastructure.Persistence.Repositories;

public class EfStockTradeRepository : IStockTradeRepository
{
    private readonly AppDbContext _context;
    public EfStockTradeRepository(AppDbContext context) {
        _context = context;
    }

    public async Task<StockTrade?> CreateAsync(StockTrade stockTrade)
    {
        _context.StockTrades.Add(stockTrade);
        await _context.SaveChangesAsync();

        return stockTrade;
    }
}