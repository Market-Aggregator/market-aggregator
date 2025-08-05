using MarketOverviewService.Worker.Workers;
using MarketOverviewService.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<AlpacaMarketConsumerWorker>();

var host = builder.Build();
host.Run();