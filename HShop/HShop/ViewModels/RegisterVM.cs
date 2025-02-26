using System.ComponentModel.DataAnnotations;

namespace HShop.ViewModels
{
    public class RegisterVM
    {
        [Key]
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Max length is 20 character")]
        public string MaKh { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string? MatKhau { get; set; }

        [Display(Name = "Họ tên")]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters")]
        public string HoTen { get; set; } = null!;

        public bool GioiTinh { get; set; } = true;

        [Display(Name ="Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [MaxLength(60, ErrorMessage = "Max length is 60 characters")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [MaxLength(24, ErrorMessage = "Max length is 24 characters")]
        [RegularExpression(@"0\d{9}", ErrorMessage = "Invalid format VN SĐT")]
        [Display(Name = "Điện thoại")]
        public string DienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Invalid format")]
        public string Email { get; set; }

        public string? Hinh { get; set; }
    }
}
