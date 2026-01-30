using Domain.Interfaces;
using System.Threading.Channels;

namespace WorkerService
{
    public class MonitoringWorker : BackgroundService
    {
        private readonly IEnumerable<ISensor> _sensors;
        private readonly Channel<SensorReadResult> _channel;
        private readonly ILogger<MonitoringWorker> _logger;

        public MonitoringWorker(
            IEnumerable<ISensor> sensors,
            Channel<SensorReadResult> channel,
            ILogger<MonitoringWorker> logger)
        {
            _sensors = sensors;
            _channel = channel;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var sensor in _sensors)
                {
                    var result = await sensor.ReadDataAsync(stoppingToken);

                    _logger.LogInformation($"Sensor {result.SensorId} read value temp: {result.Temp}; humi:{result.Humi} at {DateTime.Now}");

                    await _channel.Writer.WriteAsync(result, stoppingToken);

                    if (!result.IsOnline)
                        _logger.LogWarning("Sensor {Id} is OFFLINE", result.SensorId);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
