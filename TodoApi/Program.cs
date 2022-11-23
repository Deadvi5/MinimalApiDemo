using Asp.Versioning;
using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoApi;
using TodoApi.Services;
using TodoApi.Validation;
using TodoApi.Versioning;

WebApplicationBuilder builder = WebApplication
    .CreateBuilder(args);

var services = builder.Services;

// Configure auth
services.AddAuthentication().AddJwtBearer();
services.AddAuthorization();

// Configure the database
services.AddDbContext<TodoDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("todos")));

//Api versioning
// Add services to the container.
services.AddEndpointsApiExplorer();
services.AddApiVersioning(
            options =>
            {
                // reporting api versions will return the headers
                // "api-supported-versions" and "api-deprecated-versions"
                options.ReportApiVersions = true;

                options.Policies.Sunset( 0.9 )
                                .Effective( DateTimeOffset.Now.AddDays( 60 ) )
                                .Link( "policy.html" )
                                    .Title( "Versioning Policy" )
                                    .Type( "text/html" );

                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;

            } )
        .AddApiExplorer(
            options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            } )
        // this enables binding ApiVersion as a endpoint callback parameter. if you don't use it, then
        // you should remove this configuration.
        .EnableApiVersionBinding();
services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
services.Configure<SwaggerGeneratorOptions>(o => o.InferSecuritySchemes = true);
services.AddSwaggerGen( options => options.OperationFilter<SwaggerDefaultValues>() );

// Configure OpenTelemetry
builder.AddOpenTelemetry();

//Dependency Injection
services.AddSingleton<IValidator<NewTodo>, NewTodoValidator>();
services.AddScoped<IPostTodoService, PostTodoService>();

WebApplication app = builder.Build();

app.Map("/", () => Results.Redirect("/swagger"));


//Create version set
ApiVersionSet versionSet = app.NewApiVersionSet()
                    .HasApiVersion(1.0)
                    .HasApiVersion(2.0)
                    .ReportApiVersions()
                    .Build();

// Configure the APIs
RouteGroupBuilder groupV1 = app.MapGroup("/V1.0/todos");
groupV1.AddEndpointFilterFactory(FluentValidationFilter.ValidationFilterFactory);
groupV1.MapTodos().WithApiVersionSet(versionSet).MapToApiVersion(1.0).HasApiVersion(1.0);


RouteGroupBuilder groupV2 = app.MapGroup("/V2.0/todos");
groupV2.AddEndpointFilterFactory(FluentValidationFilter.ValidationFilterFactory);
groupV2.MapTodos().WithApiVersionSet(versionSet).MapToApiVersion(2.0).HasApiVersion(1.0); 

// Configure the prometheus endpoint for scraping metrics
app.MapPrometheusScrapingEndpoint();
// NOTE: This should only be exposed on an internal port!
//.RequireHost("*:9100");


if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            var descriptions = app.DescribeApiVersions();

            // build a swagger endpoint for each discovered API version
            foreach ( var description in descriptions )
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint( url, name );
            }
        } );
}

app.Run();
