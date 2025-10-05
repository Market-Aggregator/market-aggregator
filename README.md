# Market Aggregator
A real-time market data aggregation platform built with .NET microservices. The system ingests live stock trades and quotes from multiple providers (e.g., Alpaca API), streams events via Kafka, and persists them into PostgreSQL for querying and visualization. Designed with Clean Architecture principles, the aggregator provides a robust foundation for future extensions into trading engines, backtesting, and analytics.
<!-- Improved compatibility of back to top link -->
<a id="readme-top"></a>

<!-- PROJECT LOGO -->
<br />
<div>
  <!-- <a href="https://github.com/aliffamir/market-aggregator"> -->
  <!--   <img src="public/logo.png" alt="Logo" width="80" height="80"> -->
  <!-- </a> -->


  <p>
  </p>
</div>

<!-- ABOUT THE PROJECT -->

## About The Project

<!-- <div display="flex"> -->
<!--  <img src="public/market-aggregator-architecture.png" alt="market aggregator architecture" width="100%" /> -->
<!-- </div> -->

The **Market Aggregator** aims to consolidate live financial data into a structured ecosystem of services.  
It currently consists of:

- **MarketFeedService** – connects to WebSocket APIs (e.g., Alpaca), authenticates, subscribes to symbols, and publishes trades/quotes into Kafka topics.
- **MarketOverviewService** – consumes Kafka topics, persists data with EF Core into PostgreSQL, and exposes APIs for downstream systems.

The system is designed to be **scalable, resilient, and extendable** for financial applications, from market monitoring dashboards to algorithmic trading.

### Built With

#### Core Services

- [.NET 9](https://dotnet.microsoft.com/) (C#)
- [Kafka](https://kafka.apache.org/) (event streaming)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) (ORM)
- [PostgreSQL](https://www.postgresql.org/) (database)

#### Testing & Tooling

- [xUnit](https://xunit.net/) (unit testing)
- [Reqnroll](https://reqnroll.net/) (BDD testing)
- [Docker](https://www.docker.com/) (local infra & service orchestration) Market Aggregator
