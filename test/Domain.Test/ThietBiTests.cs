using Domain.Aggregates;
using Domain.Aggregates.GiamSatAggregate;
using Domain.Events;
using FluentAssertions;

namespace Domain.Test
{
    public class ThietBiTests
    {
        [Fact]
        public void CapNhatChiSo_NhietDoVuotNguong_PhaiSinhDomainEvent()
        {
            // Arrange
            double nguongNhiet = 24;
            var thietBi = new ThietBiGiamSat("TB01", "Phong Server 1", nguongNhiet, 50);

            // Act: Cập nhật 30 độ (Lớn hơn ngưỡng 24)
            thietBi.CapNhatDuLieu(30, 40);

            // Assert
            thietBi.DomainEvents.Should().ContainSingle(e => e is CanhBaoNhietDoEvent);
        }

        //[Fact]
        //public void CapNhatChiSo_NhietDoBinhThuong_KhongSinhEvent()
        //{
        //    // Arrange
        //    double nguongNhiet = 24;
        //    var thietBi = new ThietBiGiamSat("TB01", "Phong Server 1", nguongNhiet, 50);

        //    // Act: Cập nhật 20 độ (Nhỏ hơn ngưỡng 24)
        //    thietBi.CapNhatDuLieu(20, 40);

        //    // Assert
        //    thietBi.DomainEvents.Should().BeEmpty(); // Không được có event nào
        //}
    }
}