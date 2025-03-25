using HShop.Data;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HShop.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;
        public CategoryViewComponent(Hshop2023Context context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(l => new MenuLoaiVM
            {
                MaLoai = l.MaLoai,
                TenLoai = l.TenLoai,
                SoLuong = l.HangHoas.Count
            }).OrderBy(p => p.TenLoai);

            return View(data);
        }
    }
}
