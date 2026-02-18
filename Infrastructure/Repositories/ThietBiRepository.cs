using Domain.Aggregates.GiamSatAggregate;
using Infrastructure.Data;
using Dapper;
namespace Infrastructure.Repositories
{
    public class ThietBiRepository : IThietBiRepository
    {
        private readonly DapperContext _context;

        public ThietBiRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> InsertChiSoMoiTruongAsync(ChiSoMoiTruong chiSo)
        {
            // LƯU Ý: Sử dụng cú pháp tham số :TenBien của Oracle
            var query = @"
                INSERT INTO CHI_SO_MOI_TRUONG (ID, THIET_BI_ID, NHIET_DO, DO_AM, THOI_GIAN_GHI) 
                VALUES (:Id, :ThietBiId, :NhietDo, :DoAm, :ThoiGianGhi)";

            using (var connection = _context.CreateConnection())
            {
                // Dapper sẽ tự động map các properties trong object 'chiSo' với các tham số ':TenBien'
                return await connection.ExecuteAsync(query, chiSo);
            }
        }
    }
}
