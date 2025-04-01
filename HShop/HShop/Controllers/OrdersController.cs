using HShop.Data;
using HShop.Helpers;
using HShop.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace HShop.Controllers
{
    [Authorize(Roles="1")]
    public class OrdersController : Controller
    {
        private readonly Hshop2023Context db;

        public OrdersController(Hshop2023Context db)
        {
            this.db = db;
        }

        [Route("Admin/Orders/Detail/{id}")]
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

        [Route("Admin/Orders/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var donHang = await db.HoaDons.FirstOrDefaultAsync(h => h.MaHd == id);
            var cthds = await db.ChiTietHds.Where(c => c.MaHd == id).ToListAsync();
            db.HoaDons.Remove(donHang);
            db.ChiTietHds.RemoveRange(cthds);
            await db.SaveChangesAsync();
            return Redirect("/Admin/Orders");
        }

        public async Task<IActionResult> Confirm(int id, DateTime deliveryDate)
        {
			if (deliveryDate < DateTime.Now.Date)
			{
				TempData["ErrorMessage"] = "Ngày giao hàng không thể ở trong quá khứ";
                return Redirect("/admin/orders/detail/" + id);
			}
			using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
					var donHang = await db.HoaDons.SingleOrDefaultAsync(h => h.MaHd == id);
					if (donHang == null)
					{
						return NotFound();
					}
					donHang.NgayGiao = deliveryDate;
					donHang.MaTrangThai = 2;
					var MaKH = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst(MyConstant.CLAIM_CUSTOMERID)?.Value : "Guest";
					if (MaKH == "Guest")
					{
						return Redirect("/404");
					}
					donHang.MaNv = MaKH;
                    await db.SaveChangesAsync();
					await transaction.CommitAsync();
					TempData["SuccessMessage"] = "Đã xác nhận đơn hàng thành công";
					return Redirect("/admin/orders");
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					// Log lỗi ở đây (sử dụng ILogger)
					TempData["ErrorMessage"] = "Có lỗi xảy ra khi xác nhận đơn hàng";
					return Redirect("/admin/orders/detail/" + id);
				}
			}
				
        }
    }
}
