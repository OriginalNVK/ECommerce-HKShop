using System.ComponentModel.DataAnnotations;

namespace HShop.ViewModels
{
    public class ClientVM
    {
        public string MaKH {  get; set; }

		public string? MatKhau { get; set; }

		public string HoTen { get; set; } = null!;

        public bool GioiTinh { get; set; }

        public DateTime NgaySinh { get; set; }

        public string? DiaChi { get; set; }

        public string? DienThoai { get; set; }

        public string Email { get; set; } = null!;

		public int VaiTro { get; set; }

		public string? Hinh { get; set; }
    }
}
