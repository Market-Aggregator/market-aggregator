using MarketOverviewService.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace MarketOverviewService.Infrastructure.Persistence.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<StockTrade> StockTrades => Set<StockTrade>();
    public DbSet<StockQuote> StockQuotes => Set<StockQuote>();
}