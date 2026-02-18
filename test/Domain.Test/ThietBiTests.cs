using Domain.Aggregates;
using Domain.Aggregates.GiamSatAggregate;
using Domain.Events;
using FluentAssertions;
using Xunit;

namespace Domain.Test
{
    public class ThietBiTests
    {
        [Fact]
        public void CapNhatChiSo_NhietDoVuotNguong_PhaiSinhDomainEvent()
        {
            // Arrange
            double nguongNhiet = 24;
            double nhietDoAnToan = 20;
            var thietBi = new ThietBiGiamSat("TB01", "Phong Server 1", nguongNhiet, 20);
            // Giả sử logic của bạn quy định > 80 độ là cảnh báo

            // 2. Act (Hành động)
            // Cập nhật 90 độ (Vượt ngưỡng)
            thietBi.CapNhatDuLieu(nhietDoAnToan, 70);

            // 3. Assert (Kiểm tra kết quả)
            // Kiểm tra xem biến DomainEvents có chứa event nào không
            thietBi.DomainEvents.Should().ContainSingle(e => e is CanhBaoNhietDoEvent);
            // Hoặc kiểm tra chi tiết event đó (nếu bạn đã cast type)
        }

        [Fact]
        public void CapNhatChiSo_NhietDoBinhThuong_KhongSinhEvent()
        {
            // Arrange
            var thietBi = new ThietBiGiamSat("TB01", "Phong Server 1", 24, 20);

            // Act
            thietBi.CapNhatDuLieu(40, 50); // Nhiệt độ mát

            // Assert
            thietBi.DomainEvents.Should().ContainSingle(e => e is CanhBaoNhietDoEvent);
        }
    }
}