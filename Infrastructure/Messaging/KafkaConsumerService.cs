using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs;
using System.Text.Json;
using Application.Interfaces; // Giả sử bạn có IThietBiRepository

namespace Infrastructure.Messaging
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ConsumerConfig _config;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServiceProvider _serviceProvider; // Dùng để tạo Scope
        private const string TopicName = "iot-sensor-data"; // Topic cần nghe

        public KafkaConsumerService(ConsumerConfig config, IServiceScopeFactory scopeFactory)
    {
        _config = config;
        _scopeFactory = scopeFactory;
    }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Tạo Consumer
            using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();

            // Đăng ký nhận tin từ Topic
            consumer.Subscribe(TopicName);
            _logger.LogInformation($"Kafka Consumer đã lắng nghe topic: {TopicName}");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        // 1. Chờ tin nhắn (Block 100ms)
                        var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(100));
                        if (consumeResult == null)
                        {
                            await Task.Delay(100, stoppingToken);
                            continue;
                        }    
                        

                        _logger.LogInformation($"Nhận dữ liệu: {consumeResult.Message.Value}");

                        // 2. Xử lý logic nghiệp vụ (DDD)
                        await ProcessMessageAsync(consumeResult.Message.Value);

                        // 3. Commit offset (Đánh dấu đã xử lý xong)
                        consumer.Commit(consumeResult);
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Lỗi nhận tin Kafka: {e.Error.Reason}");
                        await Task.Delay(1000, stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }

        private async Task ProcessMessageAsync(string messageJson)
        {
            // Deserialize JSON về DTO
            var data = JsonSerializer.Deserialize<TelemetryDataDto>(messageJson);
            if (data == null) return;

            // QUAN TRỌNG: Tạo Scope mới để gọi Repository/Application Service
            using (var scope = _serviceProvider.CreateScope())
            {
                // Lấy Service từ Scope ra (Service này có kết nối DB)
                // Ví dụ: IGiamSatService hoặc IRepository
                // var giamSatService = scope.ServiceProvider.GetRequiredService<IGiamSatService>();

                // Gọi logic nghiệp vụ
                // await giamSatService.XuLyDuLieuTuIot(data);

                _logger.LogInformation($"Đã xử lý xong dữ liệu máy: {data.MaThietBi}");
            }
        }
    }
}