using API.Helper;
using Application.Interfaces;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Infrastructure.Messaging;
using KafkaConsumerService = Infrastructure.Messaging.KafkaConsumerService;

namespace API.Extensions
{
    public static class KafkaExtensions
    {
        public static IServiceCollection AddKafkaInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Đọc giá trị cấu hình
            var kafkaSettings = configuration.GetSection("KafkaSettings");
            var bootstrapServers = kafkaSettings["BootstrapServers"];
            var topicName = "iot-sensor-data";
            // Tạo Topic tự động nếu chưa có (Dùng AdminClient)
            EnsureTopicExists(bootstrapServers, topicName);

            // Validate cấu hình
            if (string.IsNullOrEmpty(bootstrapServers))
                throw new ArgumentNullException("Kafka BootstrapServers chưa được cấu hình!");

            // Lưu ý: Nếu bạn cần biến mainTopic ở nơi khác, bạn có thể đăng ký nó vào DI container hoặc dùng Options Pattern.
            // var mainTopic = kafkaSettings["Topics:UserActionTopic"]; 

            // Cấu hình Producer (Confluent Native) - Dùng cho nội bộ Infrastructure
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                SocketTimeoutMs = 5000,
                MessageTimeoutMs = 3000,
                Acks = Acks.Leader // Đảm bảo Leader nhận được tin là ok
            };

            services.AddSingleton<IProducer<Null, string>>(sp =>
                new ProducerBuilder<Null, string>(producerConfig).Build());

            // QUAN TRỌNG: Đăng ký Interface IEventProducer với DI
            // Để Controller gọi _producer.ProduceAsync() thì nó chạy vào code của Infrastructure
            services.AddSingleton<IEventProducer, KafkaProducer>();

            // Cấu hình Consumer (Background Service)
            // Cần truyền config vào để Consumer Service dùng
            services.AddSingleton(new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "giam-sat-phong-may-group", // Định danh nhóm consumer
                AutoOffsetReset = AutoOffsetReset.Earliest, // Đọc từ đầu nếu chưa có offset
                EnableAutoCommit = false // Tự commit tay để đảm bảo an toàn dữ liệu
            });

            // 3. Cấu hình Consumer (Hosted Service)
            services.AddHostedService<KafkaConsumerService>();

            return services;
        }
        private static void EnsureTopicExists(string bootstrapServers, string topicName)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();

            try
            {
                // Kiểm tra xem topic đã có chưa
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
                var topicExists = metadata.Topics.Any(t => t.Topic == topicName);

                if (!topicExists)
                {
                    // Nếu chưa có, tạo mới với 1 Partition và 1 Replication Factor
                    adminClient.CreateTopicsAsync(new TopicSpecification[]
                    {
                        new TopicSpecification
                        {
                            Name = topicName,
                            NumPartitions = 1,
                            ReplicationFactor = 1
                        }
                    }).Wait(); // Wait để đảm bảo tạo xong mới chạy tiếp

                    Console.WriteLine($"[Kafka] Đã tạo topic mới: {topicName}");
                }
            }
            catch (Exception ex)
            {
                // Có thể bỏ qua lỗi nếu Topic đã tồn tại song song (race condition)
                Console.WriteLine($"[Kafka Warning] Không thể tạo topic: {ex.Message}");
            }
        }
    }
}
