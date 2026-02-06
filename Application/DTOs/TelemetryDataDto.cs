using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TelemetryDataDto
    {
        public string MaThietBi { get; set; }
        public double NhietDo { get; set; }
        public double DoAm { get; set; }
        public DateTime ThoiGian { get; set; }
    }
}
