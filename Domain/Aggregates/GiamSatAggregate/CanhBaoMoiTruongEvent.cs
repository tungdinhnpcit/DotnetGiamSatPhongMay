using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Aggregates.GiamSatAggregate
{
    // Sử dụng 'record' trong C# giúp code ngắn gọn và bất biến (immutable)
    public record CanhBaoMoiTruongEvent(string MaThietBi, double NhietDo, double DoAm) : IDomainEvent;
}
