using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public interface IDomainEvent
    {
        // Bạn có thể thêm DateTime OccurredOn { get; } nếu muốn lưu thời gian xảy ra sự kiện
    }
}
