# PeculiarCardGame backend

## Solution structure

Solution consists of the following projects:
- PeculiarCardGame.Data - anything related to data storing, e.g. `DbContext`, models or migrations
- PeculiarCardGame.Services - business logic implementation
- PeculiarCardGame.Shared - some enums and classes shared between projects
- PeculiarCardGame.Tests - collection of unit and API tests
- PeculiarCardGame.WebApi - web API exposing endpoints for application usage

## Setup

### Requirements

This application is developed using .NET 7, which is the main requirement. Additionally, to use provided `docker compose` file which runs SQL Server, a version of Docker that supports Docker Compose v2 is required.

## Configuration

Configuration is stored in 2 or 3 places, depending on whether you use standalone database or provided `docker compose` file:
1. most of configuration will be stored in `appsettings.json` and `appsettings.[environment].json` files of `PeculiarCardGame.WebApi` project
2. confidential data, such as SQL connection string, or bearer token security key for environments other that Development, is stored in dotnet secret store, or can alternatively passed as environment variables
3. `docker compose`-hosted SQL Server sa password is stored as `MSSQL_SA_PASSWORD` environment variable, e.g. set in `.env`

The first one is pushed to git remote, but the other two have to be filled manually after cloning the repo for application to run correctly.

Current list of configuration entries stored in secret store:
- `Authentication__Bearer__Key` - secret key used to sign and validate JWTs, at least 128 bits
- `SqlServer__ConnectionString` - db connection string

SQL connection string for development using docker-hosted SQL Server: `Data Source=localhost,11433;User Id=sa;Password=[password];TrustServerCertificate=True`

