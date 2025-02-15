using HShop.Data;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HShop.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context db;

        public HangHoaController(Hshop2023Context context) => db = context;
        public IActionResult Index(int? loai)
        {
            var HangHoas = db.HangHoas.AsQueryable();

            if(loai.HasValue)
            {
                HangHoas = HangHoas.Where(p => p.MaLoai == loai.Value);
            }

            var result = HangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            return View(result);
        }

        public IActionResult Search(string query)
        {
            var HangHoas = db.HangHoas.AsQueryable();

            if (query != null)
            {
                HangHoas = HangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = HangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            return View(result);
        }
    }
}
