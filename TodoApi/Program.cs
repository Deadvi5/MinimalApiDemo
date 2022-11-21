using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoApi;
using TodoApi.Validation;

WebApplicationBuilder builder = WebApplication
    .CreateBuilder(args);

// Configure auth
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

// Configure the database
builder.Services.AddDbContext<TodoDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("todos")));

// Configure Open API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<SwaggerGeneratorOptions>(o => o.InferSecuritySchemes = true);

// Configure OpenTelemetry
builder.AddOpenTelemetry();

builder.Services.AddSingleton<NewTodoValidator>();
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.Map("/", () => Results.Redirect("/swagger"));



// Configure the APIs
RouteGroupBuilder group = app.MapGroup("/todos");
group.AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory);
group.MapTodos();

// Configure the prometheus endpoint for scraping metrics
app.MapPrometheusScrapingEndpoint();
// NOTE: This should only be exposed on an internal port!
//.RequireHost("*:9100");

app.Run();
