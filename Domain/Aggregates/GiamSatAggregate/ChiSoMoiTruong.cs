using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Aggregates.GiamSatAggregate
{
    public record ChiSoMoiTruong
    {
        public double NhietDo { get; init; }
        public double DoAm { get; init; }
        public DateTime ThoiDiemGhi { get; init; }

        public ChiSoMoiTruong(double nhietDo, double doAm)
        {
            // Validation logic
            if (nhietDo < -20 || nhietDo > 80) throw new DomainException("Nhiệt độ không hợp lệ.");
            if (doAm < 0 || doAm > 100) throw new DomainException("Độ ẩm phải từ 0-100%.");

            NhietDo = nhietDo;
            DoAm = doAm;
            ThoiDiemGhi = DateTime.Now;
        }
    }
}
