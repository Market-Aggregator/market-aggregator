using MarketOverviewService.Core.Entities;
using MarketOverviewService.Core.Interfaces;
using MarketOverviewService.Infrastructure.Persistence.Data;

namespace MarketOverviewService.Infrastructure.Persistence.Repositories;

public class EfStockQuoteRepository : IStockQuoteRepository
{
    private readonly AppDbContext _context;
    public EfStockQuoteRepository(AppDbContext context) {
        _context = context;
    }

    public async Task<StockQuote?> CreateAsync(StockQuote stockQuote)
    {
        _context.StockQuotes.Add(stockQuote);
        await _context.SaveChangesAsync();

        return stockQuote;
    }
}