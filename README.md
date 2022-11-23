## Todo REST MINIMAL API

Todo REST API samples using ASP.NET Core minimal APIs. It showcases:
- Using EntityFramework and PostgreSQL for data access
- JWT authentication
- OpenAPI support
- API Versioning
- Writing tests for your REST API

## Prerequisites

### .NET
1. [Install .NET 7](https://dotnet.microsoft.com/en-us/download)

### Database

1. PostgreSQL intance: browse `Docker` folder and run 
    - `docker-start.bat` for Windows
    - `docker-start.sh` for Linux/MacOS   
1. Install the **dotnet-ef** tool: `dotnet tool install dotnet-ef -g`
1. Navigate to the `TodoApi` folder and run `dotnet ef database update` to create the database.
1. Learn more about [dotnet-ef](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

### Authentication
1. Run `dotnet user-jwts create` to create a JWT token for your user and `dotnet user-jwts create -n admin --role admin` to create an admin user.
1. You should be able to use these tokens to make authenticated requests to the endpoint.
1. Learn more about [user-jwts](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security?view=aspnetcore-7.0#using-dotnet-user-jwts-to-improve-development-time-testing)


## Optional

### OpenTelemetry
TodoApi uses OpenTelemetry to collect logs, metrics and spans.
If you wish to view the collected telemetry, follow the steps below.

#### Metrics
1. Prometheus is included in Docker Compose
1. Open [Prometheus in your browser](http://localhost:9090/)
1. Query the collected metrics

#### Distributed trace

1. Jaeger is included in Docker Compose
1. Uncomment `.AddOtlpExporter` below `builder.Services.AddOpenTelemetryTracing`, in the `TodoApi/OpenTelemetryExtensions.cs` file
1. Open [Jaeger in your browser](http://localhost:16686/)
1. View the collected spans

