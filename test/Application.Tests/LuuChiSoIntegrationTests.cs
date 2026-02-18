using Application.Commands;
using Application.Commands.EventHandlers;
using Application.Configurations;
using Application.Interfaces;
using Confluent.Kafka;
using Domain.Aggregates.GiamSatAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Application.Tests
{
    public class LuuChiSoIntegrationTests
    {
        private readonly Mock<IProducer<string, string>> _kafkaProducerMock;
        private readonly Mock<IThietBiRepository> _repoMock;
        private readonly IServiceProvider _serviceProvider;

        public LuuChiSoIntegrationTests()
        {
            _kafkaProducerMock = new Mock<IProducer<string, string>>();
            _repoMock = new Mock<IThietBiRepository>();

            var services = new ServiceCollection();

            // Đăng ký Dispatcher dùng Kafka thật nhưng Producer giả
            services.AddSingleton<IDomainEventDispatcher>(
                new KafkaDomainEventDispatcher(_kafkaProducerMock.Object));

            services.AddScoped<LuuChiSoCommandHandler>();
            services.AddSingleton(_repoMock.Object);
            services.AddOptions();
            services.Configure<DeviceSettings>(opt => opt.DefaultMaxTemperature = 25);

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task LuuChiSo_PhaiGuiTinNhanLenKafka()
        {
            var handler = _serviceProvider.GetRequiredService<LuuChiSoCommandHandler>();
            var command = new LuuChiSoCommand { ThietBiId = "TB01", NhietDo = 30 };

            await handler.HandleAsync(command);

            // Kiểm tra xem Producer có gọi hàm gửi tin nhắn không
            _kafkaProducerMock.Verify(p => p.ProduceAsync(
                "telemetry-events",
                It.IsAny<Message<string, string>>(),
                default),
                Times.Once);
        }
    }
}
