using Domain.Interfaces;
using Domain.Models;
using FluentModbus;
using Polly;
using Polly.Retry;
using System.Net;

namespace Infrastructure.Sensors
{
    // Infrastructure/Sensors/IrmSensor.cs
    public class IrmSensor : ISensor
    {
        private readonly SensorOptions _options;
        private readonly ModbusTcpClient _client;
        private readonly RetryPolicy _retryPolicy; // Chính sách Retry của Polly
        public int Id => _options.Id;

        public IrmSensor(SensorOptions options)
        {
            _options = options;
            _client = new ModbusTcpClient();

            // Khởi tạo chính sách Exponential Backoff tương tự bản Python
            _retryPolicy = Policy.Handle<Exception>().WaitAndRetry(
         retryCount: 3, //
         sleepDurationProvider: retryAttempt =>
             TimeSpan.FromSeconds(Math.Min(
                 _options.ReconnectBase * Math.Pow(_options.ReconnectBackoff, retryAttempt - 1),
                 _options.ReconnectMax //
             )),
         onRetry: (exception, timeSpan, retryCount, context) =>
         {
             Console.WriteLine($"Retry {retryCount} sau {timeSpan.TotalSeconds}s do lỗi: {exception.Message}");
         });
        }

        public Task<SensorReadResult> ReadDataAsync(CancellationToken ct)
        {
            // Sử dụng Task.Run hoặc Task.FromResult để bao bọc logic đồng bộ
            // Điều này giúp tách biệt logic 'ref/unsafe' ngầm định của thư viện khỏi máy trạng thái async
            return Task.Run(() =>
            {
                return _retryPolicy.Execute(() => // Chuyển sang dùng Polly đồng bộ nếu cần
                {
                    if (!_client.IsConnected)
                    {
                        var ipAddress = IPAddress.Parse(_options.Host); //
                        var endPoint = new IPEndPoint(ipAddress, _options.Port); //
                        _client.Connect(endPoint, ModbusEndianness.BigEndian);
                    }

                    // Đọc dữ liệu đồng bộ - FluentModbus 5.0.0 sử dụng Span bên trong 
                    // nên cần thực thi trong ngữ cảnh không phải phương thức async
                    //var data = _client.ReadInputRegisters<short>(_options.SlaveId, _options.BaseAddress, 2);

                    var data = _client.ReadInputRegisters<short>(_options.SlaveId, _options.BaseAddress, 2);
                    float temp = (data[0] / 10.0f) + _options.TempOffset; //
                    float humi = (data[1] / 10.0f) + _options.HumiOffset; //

                    return new SensorReadResult(_options.Id, temp, humi, true);
                });
            }, ct);
        }
    }
}
