using HShop.Data;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using HShop.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;

namespace HShop.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context db;

        public CartController(Hshop2023Context context) => db = context;

        private List<CartItem> GetCart()
        {
            var maKH = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst(MyConstant.CLAIM_CUSTOMERID)?.Value : "Guest";
            if(maKH == "Guest")
            {
                return null;
            }
            var CartItems = db.Carts.Where(c => c.MaKh == maKH).Select(c => new CartItem
            {
                MaHH = c.MaHh,
                TenHH = c.MaHhNavigation.TenHh,
                DonGia = c.DonGia,
                SoLuong = c.SoLuong,
                Hinh = c.MaHhNavigation.Hinh
            }).ToList();
            return CartItems;
        }

        public IActionResult Index()
        {
            var cartItems = GetCart();
            return View(cartItems);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var maKH = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst(MyConstant.CLAIM_CUSTOMERID)?.Value : "Guest";

            var item = db.Carts.FirstOrDefault(c => c.MaKh == maKH && c.MaHh == id);
            if(item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
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
                db.Carts.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
                db.Carts.Update(item);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var maKH = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst(MyConstant.CLAIM_CUSTOMERID)?.Value : "Guest";
            var item = db.Carts.FirstOrDefault(c => c.MaKh == maKH && c.MaHh == id);
            if (item != null)
            {
                db.Carts.Remove(item);
                db.SaveChanges();
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
        public IActionResult Checkout(CheckoutVM model)
        {
            var customerID = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MyConstant.CLAIM_CUSTOMERID).Value;
            var Carts = db.Carts.Where(c => c.MaKh == customerID).ToList();
            if (ModelState.IsValid)
            {
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerID);
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
                    db.Add(HoaDon);
                    db.SaveChanges();
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
                    db.ChiTietHds.AddRange(cthds);
                    db.Carts.RemoveRange(Carts);
                    db.SaveChanges();
					db.Database.CommitTransaction();
                    return View("Success");
                }
                catch
                {
                    Console.WriteLine("Have Error");
                }
            }

            var CartItems = db.Carts.Where(c => c.MaKh == customerID).Select(c => new CartItem
            {
                MaHH = c.MaHh,
                TenHH = c.MaHhNavigation.TenHh,
                DonGia = c.DonGia,
                SoLuong = c.SoLuong,
                Hinh = c.MaHhNavigation.Hinh
            }).ToList();
            if (CartItems.Count == 0)
            {
                return Redirect("/");
            }
            
            return View(CartItems);
        }
    }
}
