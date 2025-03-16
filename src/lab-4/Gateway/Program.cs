#pragma warning disable CA1506
using Gateway.Middleware;
using Gateway.Models;
using Library;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ClientOptions>(builder.Configuration.GetSection("OrderService"));
builder.Services.AddGrpcClient<Orders.ProcessingService.Contracts.OrderService.OrderServiceClient>(
    (sp, o) =>
{
    IOptions<ClientOptions> options = sp.GetRequiredService<IOptions<ClientOptions>>();
    o.Address = options.Value.OuterUrl;
});
builder.Services.AddGrpcClient<OrderService.OrderServiceClient>(
    "OrderServiceClient1",
    (sp, o) =>
{
    IOptions<ClientOptions> options = sp.GetRequiredService<IOptions<ClientOptions>>();
    o.Address = options.Value.BaseUrl;
});
builder.Services.AddGrpcClient<ProductService.ProductServiceClient>((sp, o) =>
{
    IOptions<ClientOptions> options = sp.GetRequiredService<IOptions<ClientOptions>>();
    o.Address = options.Value.BaseUrl;
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.UseAllOfForInheritance();
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