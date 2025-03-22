using HShop.Data;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using HShop.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace HShop.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context db;

        public CartController(Hshop2023Context context) => db = context;

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MyConstant.CART_KEY) ?? new List<CartItem>();
        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHH == id);
            if(item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if(hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaHH = hangHoa.MaHh,
                    TenHH = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? string.Empty,
                    SoLuong = quantity    
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.Set(MyConstant.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHH == id);
            if(item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MyConstant.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }
            
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerID = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MyConstant.CLAIM_CUSTOMERID).Value;
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
                    foreach(var item in Cart)
                    {
                        cthds.Add(new ChiTietHd()
                        {
                            MaHd = HoaDon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHH,
                            GiamGia = 0
                        });
                    }
                    db.AddRange(cthds);
                    db.SaveChanges();
					db.Database.CommitTransaction();
					HttpContext.Session.Set<List<CartItem>>(MyConstant.CART_KEY, new List<CartItem>());
                    return View("Success");
                }
                catch
                {
                    Console.WriteLine("Have Error");
                }
            }
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }

            return View(Cart);
        }
    }
}
