#pragma warning disable CA1506
using BasicImplementation.BackgroundServices;
using BasicImplementation.Extensions;
using BasicImplementation.Models;
using Kafka.Models;
using OrderService.Controllers;
using OrderService.Extensions;
using OrderService.Interceptor;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<KafkaProducerProcessOptions>().Bind(builder.Configuration.GetSection("KafkaProducerProcessOptions"));
builder.Services.AddOptions<KafkaProducerConfigOptions>().Bind(builder.Configuration.GetSection("KafkaProducerConfigOptions"));
builder.Services.AddOptions<KafkaConsumerProcessOptions>().Bind(builder.Configuration.GetSection("KafkaConsumerProcessOptions"));
builder.Services.AddOptions<KafkaConsumerConfigOptions>().Bind(builder.Configuration.GetSection("KafkaConsumerConfigOptions"));

builder.Services.AddOptions<PostgresConnectionOptions>().Bind(builder.Configuration.GetSection("Postgres"));

builder.Services.AddHostedService<MigrationHostedService>();

builder.Services.AddMigration();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddOrderServiceKafka();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddGrpc(options => options.Interceptors.Add<ClientInterceptor>());

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddServices();

WebApplication app = builder.Build();

app.UseRouting();
app.MapGrpcService<ProductController>();
app.MapGrpcService<OrderController>();
app.MapGrpcReflectionService();
app.Run();