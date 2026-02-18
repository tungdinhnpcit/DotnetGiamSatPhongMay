using Confluent.Kafka;
using Testcontainers.Kafka;
using Xunit;

namespace Application.Tests.Integration
{
    public class KafkaIntegrationTests : IAsyncLifetime
    {
        // Khai báo Container Kafka
        private readonly KafkaContainer _kafkaContainer = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:7.4.0") // Chọn version Kafka
            .Build();
            
        public async Task DisposeAsync()
        {
            await _kafkaContainer.DisposeAsync();
        }

        public Task InitializeAsync()
        {
            return _kafkaContainer.StartAsync();
        }

        [Fact]
        public async Task Test_GuiVaNhanTin_TuKafkaThat()
        {
            // --- ARRANGE (CHUẨN BỊ) ---
            // Lấy địa chỉ server ảo mà Testcontainers vừa tạo (VD: localhost:55001)
            var bootstrapServers = _kafkaContainer.GetBootstrapAddress();
            var topic = "test-topic";

            var producerConfig = new ProducerConfig { BootstrapServers = bootstrapServers };
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "test-group",
                AutoOffsetReset = AutoOffsetReset.Earliest // Đọc từ đầu
            };

            // --- ACT (HÀNH ĐỘNG) ---

            // Bước 1: Gửi tin nhắn lên Kafka thật
            using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                await producer.ProduceAsync(topic, new Message<Null, string> { Value = "Hello Kafka Real!" });
            }

            // Bước 2: Thử đọc tin nhắn đó về
            string receivedMessage = null;
            using (var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);

                // Chờ tối đa 5 giây để nhận tin
                var result = consumer.Consume(TimeSpan.FromSeconds(5));
                if (result != null)
                {
                    receivedMessage = result.Message.Value;
                }
            }

            // --- ASSERT (KIỂM TRA) ---
            Assert.NotNull(receivedMessage);
            Assert.Equal("Hello Kafka Real!", receivedMessage);
        }
    }
}
