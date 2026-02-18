
namespace Domain.Aggregates.GiamSatAggregate
{
    public interface IThietBiRepository
    {
        Task<int> AddAsync(ThietBiGiamSat thietBi);
        Task<int> InsertChiSoMoiTruongAsync(ChiSoMoiTruong chiSo);
        Task<ThietBiGiamSat> GetByMaThietBiAsync(string ma);
        Task<bool> UpdateAsync(ThietBiGiamSat thietBi); // Cập nhật trạng thái hiện tại
        Task<int> SaveTelemetryAsync(string maThietBi, double t, double h); // Ghi log lịch sử
    }
}
