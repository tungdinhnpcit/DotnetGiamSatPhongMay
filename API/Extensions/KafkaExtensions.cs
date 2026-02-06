using API.Helper;
using Confluent.Kafka;

namespace API.Extensions
{
    public static class KafkaExtensions
    {
        public static IServiceCollection AddKafkaInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Đọc giá trị cấu hình
            var kafkaSettings = configuration.GetSection("KafkaSettings");
            var bootstrapServers = kafkaSettings["BootstrapServers"];

            // Lưu ý: Nếu bạn cần biến mainTopic ở nơi khác, bạn có thể đăng ký nó vào DI container hoặc dùng Options Pattern.
            // var mainTopic = kafkaSettings["Topics:UserActionTopic"]; 

            // 2. Cấu hình Producer
            services.AddSingleton<IProducer<Null, string>>(sp =>
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = bootstrapServers,
                    SocketTimeoutMs = 5000,
                    MessageTimeoutMs = 3000
                };
                return new ProducerBuilder<Null, string>(producerConfig).Build();
            });

            // 3. Cấu hình Consumer (Hosted Service)
            services.AddHostedService<KafkaConsumerService>();

            return services;
        }
    }
}
