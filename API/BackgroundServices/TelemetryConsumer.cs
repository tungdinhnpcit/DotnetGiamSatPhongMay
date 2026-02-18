using Application.Interfaces; // Nơi chứa IThietBiRepository
using Confluent.Kafka;
using Domain.Aggregates.GiamSatAggregate;
using Domain.Events;
using System.Text.Json;

namespace API.BackgroundServices
{
    public class TelemetryConsumer : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _topic;

        public TelemetryConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            _topic = configuration["Kafka:TelemetryTopic"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    var json = consumeResult.Message.Value;

                    var telemetryEvent = JsonSerializer.Deserialize<TelemetryDataReceivedEvent>(json);

                    if (telemetryEvent != null)
                    {
                        // Vì BackgroundService là Singleton, Repository là Scoped
                        // nên phải tạo Scope thủ công
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var repository = scope.ServiceProvider.GetRequiredService<IThietBiRepository>();

                            // Gọi hàm lưu vào DB
                            await repository.SaveTelemetryAsync(
                                telemetryEvent.MaThietBi,
                                telemetryEvent.NhietDo,
                                telemetryEvent.DoAm
                            );

                            Console.WriteLine($"Đã lưu từ Kafka: {telemetryEvent.MaThietBi}");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _consumer.Close();
            }
        }
    }
}