namespace Domain.Interfaces
{
    // Domain/Interfaces/ISensor.cs
    public interface ISensor
    {
        int Id { get; }
        Task<SensorReadResult> ReadDataAsync(CancellationToken ct);
    }

    // Domain/Models/SensorReadResult.cs
    public record SensorReadResult(int SensorId, float? Temp, float? Humi, bool IsOnline);
}
