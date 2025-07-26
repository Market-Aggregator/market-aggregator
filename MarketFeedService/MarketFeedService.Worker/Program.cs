using MarketFeedService.Worker.Workers;
using MarketFeedService.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<AlpacaMarketWorker>();

var host = builder.Build();
host.Run();