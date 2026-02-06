using Domain.Common;
using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // Khởi tạo Value Object (tự validate bên trong nó)
            this.ChiSoHienTai = new ChiSoMoiTruong(nhietDo, doAm);

            // Logic kiểm tra cả Nhiệt độ và Độ ẩm
            if (nhietDo > NguongNhietDo)
            {
                // Nếu nóng quá -> Bắn Domain Event
                // Các bên khác (Email, SignalR) sẽ nghe Event này
                AddDomainEvent(new CanhBaoNhietDoEvent(this.MaThietBi, nhietDo));
            }
        }

        public void NgungHoatDong()
        {
            this.TrangThai = DeviceStatus.Inactive;
        }
    }

    public enum DeviceStatus { Active, Inactive, Error }
}
