using AutoMapper;
using HShop.Data;
using HShop.Helpers;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                    return RedirectToAction("Index", "HangHoa ");
                }
                catch (Exception ex) {
                    // do nothing
                }
            }
            return View(model); 
        }
    }
}
