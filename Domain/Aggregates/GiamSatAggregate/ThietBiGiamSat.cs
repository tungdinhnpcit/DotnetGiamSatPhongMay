using Domain.Common;
using Domain.Events;

namespace Domain.Aggregates.GiamSatAggregate
{
    public class ThietBiGiamSat : Entity
    {
        public string MaThietBi { get; private set; } // MAC Address hoặc Serial
        public string TenViTri { get; private set; }
        public ChiSoMoiTruong ChiSoHienTai { get; private set; }
        public double NguongNhietDo { get; private set; }
        public double NguongDoAm { get; private set; }
        public DeviceStatus TrangThai { get; private set; }

        private ThietBiGiamSat() : base() { }

        public ThietBiGiamSat(string ma, string viTri, double maxT = 80, double maxH = 50) : base()
        {
            MaThietBi = ma;
            TenViTri = viTri;
            NguongNhietDo = maxT;
            NguongDoAm = maxH;
        }

        // --- BEHAVIORS (Nghiệp vụ) ---

        public void CapNhatDuLieu(double nhietDo, double doAm)
        {
            // 1. Luôn bắn event ghi nhận dữ liệu (Để lưu lịch sử/log)
            AddDomainEvent(new TelemetryDataReceivedEvent(this.MaThietBi, nhietDo, doAm, DateTime.Now));

            // 2. Kiểm tra ngưỡng để bắn cảnh báo
            if (nhietDo > NguongNhietDo)
            {
                AddDomainEvent(new CanhBaoNhietDoEvent(this.MaThietBi, nhietDo));
            }

            if (doAm > NguongDoAm)
            {
                // Thêm event cảnh báo độ ẩm nếu cần
            }
        }

        public void NgungHoatDong()
        {
            this.TrangThai = DeviceStatus.Inactive;
        }
    }

    public enum DeviceStatus { Active, Inactive, Error }
}
