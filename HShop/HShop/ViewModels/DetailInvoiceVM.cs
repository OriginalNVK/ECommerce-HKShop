using HShop.Data;

namespace HShop.ViewModels
{
    public class DetailInvoiceVM
    { 
        public int MaCt { get; set; }

        public int MaHd { get; set; }

        public int MaHh { get; set; }

        public double DonGia { get; set; }

        public int SoLuong { get; set; }

        public double GiamGia { get; set; }

        public string Hinh { get; set; }

        public string TenHangHoa { get; set; }

        public double ThanhTien { get; set; }
    }
}
