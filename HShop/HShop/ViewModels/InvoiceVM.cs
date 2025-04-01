namespace HShop.ViewModels
{
    public class InvoiceVM
    {
        public int MaHd { get; set; }

        public string HoTen { get; set; }

        public DateTime NgayDat { get; set; }

        public string DiaChi { get; set; }

        public string CachThanhToan { get; set; }

        public string CachVanChuyen { get; set; }

        public string TrangThai { get; set; }

        public string GhiChu {  get; set; }

        public string DienThoai { get; set; }
    }
}

/*
 * <th>Mã HD</th>
                        <th>Ngày tạo</th>
                        <th>Khách hàng</th>
                        <th>Địa chỉ</th>
                        <th>PT Thanh toán</th>
                        <th>PT Vận chuyển</th>
                        <th>Trạng thái</th>
                        <th>Điện thoại</th>
 */