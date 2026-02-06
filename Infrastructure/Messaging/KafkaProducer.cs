using Confluent.Kafka;
using Application.Interfaces;
using System.Text.Json;

namespace Infrastructure.Messaging
{
    public class KafkaProducer : IEventProducer
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(string bootstrapServers)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task ProduceAsync<T>(string topic, T message)
        {
            var json = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
        }
    }
}
