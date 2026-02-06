using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public class CanhBaoNhietDoEvent : IDomainEvent
    {
        public string MaThietBi { get; }
        public double NhietDo { get; }

        public CanhBaoNhietDoEvent(string ma, double nhiet)
        {
            MaThietBi = ma;
            NhietDo = nhiet;
        }
    }
}
