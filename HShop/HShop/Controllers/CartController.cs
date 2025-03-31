using HShop.Data;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using HShop.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using Microsoft.EntityFrameworkCore;

namespace HShop.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context db;

        public CartController(Hshop2023Context context) => db = context;

        private async Task<List<CartItem>> GetCart()
        {
            var maKH = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst(MyConstant.CLAIM_CUSTOMERID)?.Value : "Guest";
            if(maKH == "Guest")
            {
                return null;
            }
            var CartItems = await db.Carts.Where(c => c.MaKh == maKH).Select(c => new CartItem
            {
                MaHH = c.MaHh,
                TenHH = c.MaHhNavigation.TenHh,
                DonGia = c.DonGia,
                SoLuong = c.SoLuong,
                Hinh = c.MaHhNavigation.Hinh
            }).ToListAsync();
            return CartItems;
        }

        public async Task<IActionResult> Index()
        {
            var cartItems = await GetCart();
            if(cartItems == null)
            {
                return Redirect("/KhachHang/DangNhap");
            }
            return View(cartItems);
        }

        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var maKH = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst(MyConstant.CLAIM_CUSTOMERID)?.Value : "Guest";

            if(maKH == "Guest")
            {
				return Redirect("/KhachHang/DangNhap");
			}
            var item = await db.Carts.FirstOrDefaultAsync(c => c.MaKh == maKH && c.MaHh == id);
            if(item == null)
            {
                var hangHoa = await db.HangHoas.SingleOrDefaultAsync(p => p.MaHh == id);
                if(hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new Cart
                {
                    MaKh = maKH,
                    MaHh = hangHoa.MaHh,    
                    DonGia = hangHoa.DonGia ?? 0,
                    SoLuong = quantity,    
                    NgayThem = DateTime.Now,
                };
                await db.Carts.AddAsync(item);
            }
            else
            {
                item.SoLuong += quantity;
                db.Carts.Update(item);
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> RemoveCart(int id)
        {
            var maKH = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst(MyConstant.CLAIM_CUSTOMERID)?.Value : "Guest";
            var item = await db.Carts.FirstOrDefaultAsync(c => c.MaKh == maKH && c.MaHh == id);
            if (item != null)
            {
                db.Carts.Remove(item);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            var maKH = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst(MyConstant.CLAIM_CUSTOMERID)?.Value : "Guest";
            var Carts = db.Carts.Where(c => c.MaKh == maKH).Select(c => new CartItem
            {
                MaHH = c.MaHh,
                TenHH = c.MaHhNavigation.TenHh,
                DonGia = c.DonGia,
                SoLuong = c.SoLuong,    
                Hinh = c.MaHhNavigation.Hinh
            }).ToList();
            if (Carts.Count == 0)
            {
                return Redirect("/");
            }
            
            return View(Carts);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            var customerID = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MyConstant.CLAIM_CUSTOMERID).Value;
            var Carts = await db.Carts.Where(c => c.MaKh == customerID).ToListAsync();
            if (ModelState.IsValid)
            {
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = await db.KhachHangs.SingleOrDefaultAsync(kh => kh.MaKh == customerID);
                }
                var HoaDon = new HoaDon()
                {
                    MaKh = customerID,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "Grab",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu
                };

                db.Database.BeginTransaction();
                try
                {
                    await db.AddAsync(HoaDon);
                    await db.SaveChangesAsync();
                    var cthds = new List<ChiTietHd>();
                    foreach(var item in Carts)
                    {
                        cthds.Add(new ChiTietHd()
                        {
                            MaHd = HoaDon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            GiamGia = 0
                        });
                    }
                    await db.ChiTietHds.AddRangeAsync(cthds);
                    db.Carts.RemoveRange(Carts);
                    await db.SaveChangesAsync();
					db.Database.CommitTransaction();
                    return View("Success");
                }
                catch
                {
                    Console.WriteLine("Have Error");
                }
            }

            var CartItems = await db.Carts.Where(c => c.MaKh == customerID).Select(c => new CartItem
            {
                MaHH = c.MaHh,
                TenHH = c.MaHhNavigation.TenHh,
                DonGia = c.DonGia,
                SoLuong = c.SoLuong,
                Hinh = c.MaHhNavigation.Hinh
            }).ToListAsync();
            if (CartItems.Count == 0)
            {
                return Redirect("/");
            }
            
            return View(CartItems);
        }
    }
}
