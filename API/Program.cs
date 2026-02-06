using API.Extensions;
using Application.Interfaces;
using Infrastructure.Messaging;

try
{
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

    // THÊM ĐOẠN NÀY ĐỂ DEBUG
    var url = "http://localhost:5005";
    Console.WriteLine($"--------------------------------------------------");
    Console.WriteLine($"✅ API ĐÃ KHỞI ĐỘNG THÀNH CÔNG!");
    Console.WriteLine($"👉 Swagger UI: {url}/swagger");
    Console.WriteLine($"--------------------------------------------------");

    app.Run();
}
catch (Exception ex)
{
    // ĐÂY LÀ DÒNG QUAN TRỌNG NHẤT: In lỗi ra màn hình
            Console.WriteLine("--------------------------------------------------");
    Console.WriteLine("LỖI KHỞI ĐỘNG (FATAL ERROR):");
    Console.WriteLine(ex.ToString());
    Console.WriteLine("--------------------------------------------------");

    // Giữ màn hình không bị tắt để đọc lỗi
    Console.ReadLine();
}
