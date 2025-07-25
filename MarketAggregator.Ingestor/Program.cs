using MarketAggregator.Infrastructure;
using MarketAggregator.Ingestor;
using MarketAggregator.Ingestor.WorkerService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafkaProducer(builder.Configuration);
builder.Services.AddIngestorInfrastructure();

builder.Services.AddHostedService<StreamMarketDataWorker>();

var host = builder.Build();
host.Run();