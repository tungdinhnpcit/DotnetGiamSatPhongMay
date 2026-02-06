using API.Extensions;
using Application.Interfaces;
using Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Configuration.AddJsonFile("KafkaConfig/BootstrapServer.json", optional: false, reloadOnChange: true);

// Gọi cấu hình Swagger từ Extension
builder.Services.AddSwaggerInfrastructure();

// Gọi cấu hình Kafka từ Extension (truyền configuration vào)
builder.Services.AddKafkaInfrastructure(builder.Configuration);


var app = builder.Build();
app.UseSwaggerInfrastructure();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
