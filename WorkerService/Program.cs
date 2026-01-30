using Domain.Interfaces;
using Domain.Models;         // Chứa SensorOptions
using Infrastructure.Sensors; // Chứa IrmSensor
using Microsoft.Extensions.Options;
using System.Threading.Channels;
using WorkerService;

var builder = Host.CreateApplicationBuilder(args);

// 1. Đăng ký Channel để truyền dữ liệu giữa Worker và các phần khác (nếu có)
builder.Services.AddSingleton(Channel.CreateUnbounded<SensorReadResult>());

builder.Services.Configure<List<SensorOptions>>(
    builder.Configuration.GetSection("SensorSettings"));

builder.Services.AddSingleton<IEnumerable<ISensor>>(sp =>
{
    // Lấy giá trị đã được map từ appsettings.json
    var optionsList = sp.GetRequiredService<IOptions<List<SensorOptions>>>().Value;

    // Duyệt qua danh sách options và tạo các instance IrmSensor tương ứng
    return optionsList.Select(opt => new IrmSensor(opt)).ToList();
});

// 3. Đăng ký Worker Service (Class chạy ngầm vòng lặp quét sensor)
builder.Services.AddHostedService<MonitoringWorker>();

var host = builder.Build();
host.Run();
