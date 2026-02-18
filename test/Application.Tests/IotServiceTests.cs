using Application.Commands;
using Application.Commands.EventHandlers;
using Application.Configurations; // Cập nhật namespace theo vị trí mới của DeviceSettings
using Application.Interfaces;
using Domain.Aggregates.GiamSatAggregate;
using Domain.Events;
using FluentAssertions; // Nên dùng thêm thư viện này để Assert cho "đẹp"
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GiamSat.Application.Tests
{
    public class TelemetryDataReceivedHandlerTests
    {
        [Fact]
        public async Task Handle_KhiNhanEvent_PhaiGoiHamSaveTelemetry()
        {
            // Arrange
            var repoMock = new Mock<IThietBiRepository>();
            var handler = new TelemetryDataReceivedHandler(repoMock.Object);
            var ev = new TelemetryDataReceivedEvent("TB01", 30, 50, DateTime.Now);

            // Act
            await handler.Handle(ev, CancellationToken.None);

            // Assert
            // Kiểm tra xem Handler này có thực sự gọi xuống DB để lưu log không
            repoMock.Verify(r => r.SaveTelemetryAsync("TB01", 30, 50), Times.Once);
        }
    }
    public class LuuChiSoCommandHandlerTests
    {
        private readonly Mock<IThietBiRepository> _repoMock;
        private readonly Mock<IDomainEventDispatcher> _dispatcherMock;
        private readonly LuuChiSoCommandHandler _handler;

        public LuuChiSoCommandHandlerTests()
        {
            _repoMock = new Mock<IThietBiRepository>();
            _dispatcherMock = new Mock<IDomainEventDispatcher>();

            // Giả lập Options từ appsettings
            var options = Microsoft.Extensions.Options.Options.Create(new DeviceSettings
            {
                DefaultMaxTemperature = 25,
                DefaultMaxHumidity = 70
            });

            // Khởi tạo Handler với các đối tượng Mock
            _handler = new LuuChiSoCommandHandler(
                _repoMock.Object,
                options,
                _dispatcherMock.Object);
        }

        [Fact]
        public async Task Handle_KhiThietBiChuaTonTai_PhaiKhoiTaoMoiVoiNguongMacDinh()
        {
            // 1. Arrange
            var ma = "NEW_DEVICE";
           

            var command = new LuuChiSoCommand { ThietBiId = ma, NhietDo = 20, DoAm = 40 };

            // 2. Act
            await _handler.HandleAsync(command);

            // 3. Assert
            // Kiểm tra xem có gọi hàm AddAsync để tạo mới thiết bị không
            _repoMock.Verify(r => r.AddAsync(It.Is<ThietBiGiamSat>(t =>
                t.MaThietBi == ma &&
                t.NguongNhietDo == 25)), // 25 lấy từ DeviceSettings mock ở Constructor
                Times.Once);
        }
    }
}