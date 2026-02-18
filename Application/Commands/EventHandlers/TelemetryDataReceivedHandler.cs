using Application.Configurations;
using Application.Interfaces;
using Domain.Aggregates.GiamSatAggregate;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Commands.EventHandlers
{
    public class TelemetryDataReceivedHandler : INotificationHandler<TelemetryDataReceivedEvent>
    {
        private readonly IThietBiRepository _repository;

        public TelemetryDataReceivedHandler(IThietBiRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(TelemetryDataReceivedEvent notification, CancellationToken cancellationToken)
        {
            // ĐÂY CHÍNH LÀ NƠI GỌI HÀM LƯU DỮ LIỆU VÀO DB
            // Bạn cần thêm hàm SaveTelemetryAsync vào IThietBiRepository
            await _repository.SaveTelemetryAsync(
                notification.MaThietBi,
                notification.NhietDo,
                notification.DoAm
            );

            // Có thể thêm log để kiểm tra trong lúc debug
            // Console.WriteLine($"[Event Log]: Đã ghi nhận dữ liệu cho thiết bị {notification.MaThietBi}");
        }
    }

    public class LuuChiSoCommandHandler
    {
        private readonly IThietBiRepository _repository;
        private readonly DeviceSettings _settings;
        private readonly IDomainEventDispatcher _dispatcher;

        public LuuChiSoCommandHandler(IThietBiRepository repository, IOptions<DeviceSettings> options, IDomainEventDispatcher dispatcher)
        {
            _repository = repository;
            _settings = options.Value;
            _dispatcher = dispatcher;
        }

        public async Task<bool> HandleAsync(LuuChiSoCommand command)
        {
            // 1. Tìm thiết bị trong DB (Để biết ngưỡng nhiệt độ/độ ẩm của nó)
            var thietBi = await _repository.GetByMaThietBiAsync(command.ThietBiId);

            if (thietBi == null)
            {
                // Nếu thiết bị mới tinh, có thể khởi tạo với ngưỡng từ appsettings
                thietBi = new ThietBiGiamSat(command.ThietBiId, "Vị trí mặc định", _settings.DefaultMaxTemperature);
                await _repository.AddAsync(thietBi);
            }

            // 2. Yêu cầu Domain Entity thực hiện nghiệp vụ
            // Hàm này sẽ tự kiểm tra ngưỡng và AddDomainEvent nếu cần
            thietBi.CapNhatDuLieu(command.NhietDo, command.DoAm);

            await _dispatcher.DispatchAndClearEvents(thietBi);

            return true;
        }
    }
}
