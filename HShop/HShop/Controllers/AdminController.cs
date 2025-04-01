using HShop.Data;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HShop.Controllers
{
	public class AdminController : Controller
	{
		private readonly Hshop2023Context db;

		public AdminController(Hshop2023Context db)
        {
			this.db = db;
		}
        public async Task<IActionResult> Index()
		{
			var slDonHang = await db.HoaDons.CountAsync();
			var slHangHoa = await db.HangHoas.CountAsync();
			var result = new ThongKeVM
			{
				SlDonHang = slDonHang,
				SlHangHoa = slHangHoa
			};
			return View(result);
		}

		public async Task<IActionResult> Orders()
		{
			var allOrders = await db.HoaDons.OrderByDescending(h => h.NgayDat).ToListAsync();
            var result = (from i in allOrders
                          join k in db.KhachHangs on i.MaKh equals k.MaKh into khachHangGroup
                          from kh in khachHangGroup.DefaultIfEmpty()
                          join t in db.TrangThais on i.MaTrangThai equals t.MaTrangThai into trangThaiGroup
                          from tt in trangThaiGroup.DefaultIfEmpty()
                          select new InvoiceVM
                          {
                              MaHd = i.MaHd,
                              HoTen = i.HoTen ?? kh.HoTen,
                              NgayDat = i.NgayDat,
                              DiaChi = i.DiaChi,
                              CachThanhToan = i.CachThanhToan,
                              CachVanChuyen = i.CachVanChuyen,
                              TrangThai = tt.MoTa,
                              GhiChu = i.GhiChu ?? "",
                              DienThoai = i.DienThoai ?? kh.DienThoai
                          }).ToList();
            return View(result);
		}
    }
}
