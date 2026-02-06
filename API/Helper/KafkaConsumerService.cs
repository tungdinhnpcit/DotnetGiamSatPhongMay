using Confluent.Kafka;

namespace API.Helper
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly string _topic = "db-processing-topic"; // Topic chứa lệnh ghi DB
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(IServiceScopeFactory scopeFactory, ILogger<KafkaConsumerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var config = new ConsumerConfig
            {
                // Sử dụng IP máy bạn để khớp với Docker setup
                BootstrapServers = "10.21.69.11:9092",
                GroupId = "grpc-backend-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };
            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                _consumer.Subscribe(_topic);
                _logger.LogInformation($"Kafka Consumer started. Listening on {_topic}...");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        // 1. Consume message từ Kafka
                        var consumeResult = _consumer.Consume(stoppingToken);
                        var rawData = consumeResult.Message.Value;

                        _logger.LogInformation($"Kafka Received: {rawData}");

                        // 2. Gọi hàm xử lý và ghi DB
                        await SaveToDatabase(rawData);
                    }
                    catch (OperationCanceledException) { break; }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Lỗi xử lý Kafka: {ex.Message}");
                    }
                }
            }, stoppingToken);
        }

        private async Task SaveToDatabase(string json)
        {
            // Tạo Scope để có thể sử dụng các Service Scoped (như DbContext)
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    // GIẢ LẬP: Lấy DbContext từ DI
                    // var _dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

                    _logger.LogWarning("==> [DATABASE]: Đang giải mã và ghi dữ liệu...");

                    // Giả lập delay ghi DB
                    await Task.Delay(500);

                    // Logic thực tế sẽ kiểu:
                    // var data = JsonSerializer.Deserialize<MyModel>(json);
                    // _dbContext.Users.Add(data);
                    // await _dbContext.SaveChangesAsync();

                    Console.WriteLine($"[DATABASE] Thành công: {json}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[DATABASE ERROR]: {ex.Message}");
                }
            }
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
