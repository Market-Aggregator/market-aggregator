using MarketAggregator.Infrastructure;
using MarketAggregator.Ingestor.WorkerService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddIngestorInfrastructure();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();