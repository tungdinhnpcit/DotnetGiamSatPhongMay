using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public abstract class Entity
    {
        // Định danh duy nhất cho Entity
        public virtual Guid Id { get; protected set; }

        // Danh sách các sự kiện Domain (Domain Events)
        // Dùng để thông báo các thay đổi quan trọng (VD: Vượt ngưỡng nhiệt độ)
        private readonly List<IDomainEvent> _domainEvents = new();
        // Property này phải là PUBLIC để KafkaDispatcher đọc được
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected Entity()
        {
            // Khởi tạo Id mới nếu là tạo mới hoàn toàn
            Id = Guid.NewGuid();
        }

        // Phương thức để thêm sự kiện vào danh sách chờ xử lý
        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            if (domainEvent != null)
            {
                _domainEvents.Add(domainEvent);
            }
        }

        // Xóa danh sách sự kiện sau khi đã được gửi đi thành công
        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        // Ghi đè so sánh bằng Id (Vì 2 Entity bằng nhau khi Id bằng nhau)
        public override bool Equals(object obj)
        {
            if (obj is not Entity other) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
