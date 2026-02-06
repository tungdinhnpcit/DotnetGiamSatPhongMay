using Domain.Common;
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

        public ThietBiGiamSat(string ma, string viTri, double maxT, double maxH) : base()
        {
            MaThietBi = ma;
            TenViTri = viTri;
            NguongNhietDo = maxT;
            NguongDoAm = maxH;
        }

        // --- BEHAVIORS (Nghiệp vụ) ---

        public void TiepNhanDuLieu(double t, double h)
        {
            // Khởi tạo Value Object (tự validate bên trong nó)
            this.ChiSoHienTai = new ChiSoMoiTruong(t, h);

            // Logic kiểm tra cả Nhiệt độ và Độ ẩm
            if (t > NguongNhietDo || h > NguongDoAm)
            {
                AddDomainEvent(new CanhBaoMoiTruongEvent(this.MaThietBi, t, h));
            }
        }

        public void NgungHoatDong()
        {
            this.TrangThai = DeviceStatus.Inactive;
        }
    }

    public enum DeviceStatus { Active, Inactive, Error }
}
