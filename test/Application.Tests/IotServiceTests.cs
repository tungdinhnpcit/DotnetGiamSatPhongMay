using Domain.Interfaces;
using Moq; // Thư viện giả lập
using Xunit;


namespace GiamSat.Application.Tests
{
    public class IotServiceTests
    {
        //[Fact]
        //public async Task XuLyDuLieu_DuLieuHopLe_PhaiGoiLuuVaoRepository()
        //{
        //    // 1. Arrange
        //    // Tạo một "Repository giả"
        //    var mockRepo = new Mock<IThietBiRepository>();

        //    // Setup hành vi: Khi gọi GetById thì trả về null (máy mới) hoặc object (máy cũ)
        //    // Ở đây mình ví dụ đơn giản là chỉ cần Mock thôi

        //    var service = new IotService(mockRepo.Object); // Inject đồ giả vào

        //    // 2. Act
        //    await service.XuLyDuLieu("TB01", 30, 60);

        //    // 3. Assert
        //    // Kiểm tra xem hàm LuuNhatKyHoatDongAsync có được gọi đúng 1 lần không?
        //    mockRepo.Verify(
        //        x => x.LuuNhatKyHoatDongAsync("TB01", 30, 60, It.IsAny<DateTime>()),
        //        Times.Once
        //    );
        //}
    }
}