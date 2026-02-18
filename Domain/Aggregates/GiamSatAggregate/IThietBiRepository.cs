
namespace Domain.Aggregates.GiamSatAggregate
{
    public interface IThietBiRepository
    {
        Task<int> InsertChiSoMoiTruongAsync(ChiSoMoiTruong chiSo);
    }
}
