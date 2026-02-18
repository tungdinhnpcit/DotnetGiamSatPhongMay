using Domain.Aggregates.GiamSatAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class LuuChiSoCommand
    {
        public string ThietBiId { get; set; }
        public double NhietDo { get; set; }
        public double DoAm { get; set; }
    }

    public class LuuChiSoCommandHandler
    {
        private readonly IThietBiRepository _repository;

        public LuuChiSoCommandHandler(IThietBiRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> HandleAsync(LuuChiSoCommand command)
        {
            var chiSoMoiTruong = new ChiSoMoiTruong
            (
                command.NhietDo,
                command.DoAm
            );

            var rowsAffected = await _repository.InsertChiSoMoiTruongAsync(chiSoMoiTruong);
           
            return rowsAffected > 0;
        }
    }
}
