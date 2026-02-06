using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IotController : ControllerBase
    {
        private readonly IEventProducer _producer;

        public IotController(IEventProducer producer)
        {
            _producer = producer;
        }
        [HttpPost("telemetry")]
        public async Task<IActionResult> ReceiveTelemetry([FromBody] TelemetryDataDto data)
        {
            // 1. Nhận data từ thiết bị
            // 2. Bắn ngay vào Kafka Topic "iot-sensor-data" để xử lý bất đồng bộ
            // Điều này giúp API phản hồi cực nhanh, chịu tải cao
            await _producer.ProduceAsync("iot-sensor-data", data);

            return Accepted(new { status = "Queued" });
        }
    }
}
