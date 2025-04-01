using HShop.Data;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly Hshop2023Context db;

        public OrdersController(Hshop2023Context db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Detail(int id)
        {
            var hoaDons = await db.ChiTietHds
                           .Include(h => h.MaHhNavigation)
                           .Where(h => h.MaHd == id).ToListAsync();

            if (hoaDons == null)
            {
                return NotFound();
            }

            var result = hoaDons.Select(h => new DetailInvoiceVM
            {
                MaCt = h.MaCt,
                MaHd = h.MaHd,
                MaHh = h.MaHh,
                DonGia = h.DonGia,
                SoLuong = h.SoLuong,
                GiamGia = h.GiamGia,
                TenHangHoa = h.MaHhNavigation?.TenHh,
                Hinh = h.MaHhNavigation?.Hinh,
                ThanhTien = h.DonGia * h.SoLuong
            }).ToList();

            return View(result);
        }

        public async Task<IActionResult> Update(int id)
        {
            return View(id);
        }
    }
}
