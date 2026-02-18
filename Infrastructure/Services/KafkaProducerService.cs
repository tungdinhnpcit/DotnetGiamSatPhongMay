using Application.Interfaces; // Nơi chứa IDomainEventDispatcher
using Confluent.Kafka;
using Domain.Common;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class KafkaDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic;

        public KafkaDomainEventDispatcher(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
            _topic = configuration["Kafka:TelemetryTopic"];
        }

        public async Task DispatchAndClearEvents(Entity entity)
        {
            foreach (var domainEvent in entity.DomainEvents)
            {
                // Chỉ gửi sự kiện Telemetry, các sự kiện khác có thể xử lý nội bộ hoặc topic khác
                if (domainEvent is TelemetryDataReceivedEvent telemetryEvent)
                {
                    var messageValue = JsonSerializer.Serialize(telemetryEvent);

                    await _producer.ProduceAsync(_topic, new Message<Null, string>
                    {
                        Value = messageValue
                    });
                }
            }
            entity.ClearDomainEvents();
        }
    }
}