using WorkerService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<MonitoringWorker>();

var host = builder.Build();
host.Run();
