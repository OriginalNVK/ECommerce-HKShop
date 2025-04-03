using System.Text;

namespace HShop.Helpers
{
    public static class MyUtil
    {

        public static string UpLoadHinh(IFormFile Hinh, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
                using (var myFile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    Hinh.CopyTo(myFile);
                }
                return Hinh.FileName;
            }
            catch (Exception ex)
            {
				// Log lỗi (nếu cần)
				Console.WriteLine($"Lỗi khi upload hình: {ex.Message}");
				return string.Empty;
			}
        }
        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM,./;'[]!@#$%^&*()";
            var sb = new StringBuilder();
            var rd = new Random();
            for(int i = 0; i < length; i++) 
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }

        public static string convertStatus(string status)
        {
            switch (status)
            {
                case "cancelled":
                    return "Đơn hàng đã bị hủy";
                case "pending":
                    return "Đơn hàng chưa được duyệt";
                case "processing":
                    return "Đơn hàng đang chờ giao";
                case "paid":
                    return "Đơn hàng đã được thanh toán";
                case "completed":
                    return "Đơn hàng đã hoàn thành";
                default:
                    return "Đơn hàng lỗi";
            }
        }
    }
}
