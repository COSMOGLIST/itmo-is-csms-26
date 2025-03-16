#pragma warning disable CA1506
using Lab3.BackgroundServices;
using Lab3.Middleware;
using Microsoft.OpenApi.Models;
using Task1.Extensions;
using Task1.Models;
using Task2;
using Task3.Extensions;
using Task3.Models;
using ConfigurationUpdateService = Lab3.BackgroundServices.ConfigurationUpdateService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRefitConfigurationService();
var customConfigurationProvider = new CustomConfigurationProvider();
builder.Services.AddSingleton<CustomConfigurationProvider>(_ => customConfigurationProvider);
builder.Services.Configure<ConfigurationUpdateOptions>(o => o.UpdateInterval = new TimeSpan(0, 0, 60));
builder.Configuration.AddJsonFile("appsettings.json").Add(new CustomConfigurationSource(customConfigurationProvider));

builder.Services.AddHostedService<ConfigurationUpdateService>();
builder.Services.AddHostedService<MigrationHostedService>();

builder.Services.Configure<ServiceOptions>(o => o.PageSize = 10);
builder.Services.Configure<ConfigurationOptions>(builder.Configuration.GetSection("OuterService"));

builder.Services.AddOptions<PostgresConnectionOptions>().Bind(builder.Configuration.GetSection("Postgres"));

builder.Services.AddMigration();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
});

builder.Services.AddScoped<ExceptionFormattingMiddleware>();

WebApplication app = builder.Build();

app.UseMiddleware<ExceptionFormattingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
});

app.UseRouting();
app.MapControllers();
app.Run();