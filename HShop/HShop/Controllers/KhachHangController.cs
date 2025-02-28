using AutoMapper;
using HShop.Data;
using HShop.Helpers;
using HShop.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HShop.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly IMapper _mapper;
        private readonly Hshop2023Context db;

        public KhachHangController(Hshop2023Context context, IMapper mapper)
        {
            _mapper = mapper;
            db = context;
        }

        #region Đăng ký 
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile Hinh)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRandomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true; // sẽ xử lý khi dùng mail để active
                    khachHang.VaiTro = 0;

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UpLoadHinh(Hinh, "KhachHang");
                    }

                    db.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Index", "HangHoa");
                }
                catch (Exception ex) {
                    // do nothing
                }
            }
            return View(model); 
        }
        #endregion

        #region Đăng nhập
        [HttpGet]
        public IActionResult DangNhap(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;  
            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> DangNhap(LoginVM model, string? returnUrl)
        {
			ViewBag.ReturnUrl = returnUrl;
            if(ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.Username);
                if(khachHang == null)
                {
                    ModelState.AddModelError("Lỗi", "Sai thông tin đăng nhập");
                }
                else
                {
                    if(!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("Lỗi", "Tài khoản của bạn đã bị khóa vui lòng liên hệ Admin");
                    }
                    else
                    {
                        if(khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                        {
                            ModelState.AddModelError("Lỗi", "Sai mật khẩu đăng nhập");
                        }
                        else
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Name, khachHang.HoTen),
                                new Claim("CustomerID", khachHang.MaKh),

                                //claim role - động
                                new Claim(ClaimTypes.Role, "Customer")
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimsPrincipal);
                            if (Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                    }
                }
            }
			return View();
		}
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
