using MarketOverviewService.Api.Hubs;
using MarketOverviewService.Infrastructure;
using MarketOverviewService.Api.Workers;
using MarketOverviewService.Api.Messaging;
using MarketOverviewService.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    // CORS policy for client running locally
    options.AddPolicy("AllowFrontendLocalDev", policy => policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddInfrastructure(builder.Configuration);
// builder.Services.AddHostedService<AlpacaMarketConsumerWorker>();
builder.Services.AddHostedService<AlpacaTestMarketConsumerWorker>();

builder.Services.AddSingleton<IPublisher, SignalRPublisher>();

// builder.WebHost.UseUrls("http://localhost:5098", "https://localhost:7174");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontendLocalDev");

app.MapHub<MarketHub>("/stock");

app.Run();