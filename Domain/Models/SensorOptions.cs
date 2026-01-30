using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SensorOptions
    {
        public int Id { get; set; }
        public string Host { get; set; } = "192.168.31.50";
        public int Port { get; set; } = 502;
        public byte SlaveId { get; set; } = 1;
        public int BaseAddress { get; set; } = 2;
        public float TempOffset { get; set; } = 0;
        public float HumiOffset { get; set; } = 0;

        // Các tham số cho Exponential Backoff
        public double ReconnectBase { get; set; } = 5;    // base_delay
        public double ReconnectMax { get; set; } = 60;    // max_delay
        public double ReconnectBackoff { get; set; } = 1.5; // backoff
    }
}
