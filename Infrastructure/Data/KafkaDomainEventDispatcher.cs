using Application.Interfaces;
using Confluent.Kafka;
using Domain.Common;
using MediatR;
using System.Text.Json;

namespace Infrastructure.Data
{
    public class KafkaDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IProducer<string, string> _producer;

        public KafkaDomainEventDispatcher(IProducer<string, string> producer)
        {
            _producer = producer;
        }

        public async Task DispatchAndClearEvents(Entity entity)
        {
            var events = entity.DomainEvents.ToList();
            entity.ClearDomainEvents();

            foreach (var ev in events)
            {
                var topic = "telemetry-events"; // Hoặc ev.GetType().Name
                var message = JsonSerializer.Serialize(ev);

                await _producer.ProduceAsync(topic, new Message<string, string>
                {
                    Key = entity.Id.ToString(),
                    Value = message
                });
            }
        }
    }
}
