using Confluent.Kafka;
using Domain.Aggregates.GiamSatAggregate;
using Domain.Events;
using System.Text.Json;

public class TelemetryConsumerWorker : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IServiceProvider _serviceProvider;

    public TelemetryConsumerWorker(IConsumer<string, string> consumer, IServiceProvider serviceProvider)
    {
        _consumer = consumer;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe("telemetry-events");

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(stoppingToken);
            var ev = JsonSerializer.Deserialize<TelemetryDataReceivedEvent>(result.Message.Value);

            if (ev != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IThietBiRepository>();

                // THỰC HIỆN LƯU DỮ LIỆU VÀO DB TẠI ĐÂY
                await repo.SaveTelemetryAsync(ev.MaThietBi, ev.NhietDo, ev.DoAm);
            }
        }
    }
}