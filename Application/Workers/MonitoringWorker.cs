using System.Threading.Channels;

namespace Application
{
    // Application/Workers/MonitoringWorker.cs
    //public class MonitoringWorker : BackgroundService
    //{
    //    private readonly IEnumerable<ISensor> _sensors;
    //    private readonly Channel<SensorReadResult> _channel;
    //    private readonly ILogger<MonitoringWorker> _logger;

    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        while (!stoppingToken.IsCancellationRequested)
    //        {
    //            foreach (var sensor in _sensors)
    //            {
    //                var result = await sensor.ReadDataAsync(stoppingToken);
    //                await _channel.Writer.WriteAsync(result, stoppingToken);

    //                if (!result.IsOnline)
    //                    _logger.LogWarning("Sensor {Id} is OFFLINE", result.SensorId);
    //            }
    //            await Task.Delay(1000, stoppingToken);
    //        }
    //    }
    //}
}
