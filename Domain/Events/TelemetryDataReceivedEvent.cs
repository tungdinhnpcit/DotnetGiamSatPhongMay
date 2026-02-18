using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public class TelemetryDataReceivedEvent : IDomainEvent, INotification
    {
        public string MaThietBi { get; }
        public double NhietDo { get; }
        public double DoAm { get; }
        public DateTime NgayGhi { get; }

        public TelemetryDataReceivedEvent(string maThietBi, double nhietDo, double doAm, DateTime ngayGhi)
        {
            MaThietBi = maThietBi;
            NhietDo = nhietDo;
            DoAm = doAm;
            NgayGhi = ngayGhi;
        }
    }
}
