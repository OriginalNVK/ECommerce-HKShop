using HShop.Data;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;

namespace HShop.ViewComponents
{
    public class NewProductsViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;

        public NewProductsViewComponent(Hshop2023Context db)
        {
            this.db = db;
        }

        public IViewComponentResult Invoke()
        {
            var model = new NewProductsModel()
            {
                Categories = db.Loais
                .Select(l => new CategoryProducts
                {
                    MaLoai = l.MaLoai,
                    TenLoai = l.TenLoai,
                    Products = db.HangHoas
                        .Where(h => h.MaLoai == l.MaLoai)
                        .OrderByDescending(h => h.NgaySx) // Sắp xếp theo sản phẩm mới nhất
                        .Take(5) // Lấy 5 sản phẩm
                        .Select(h => new HangHoaVM
                        {
                            MaHh = h.MaHh,
                            TenHH = h.TenHh,
                            DonGia = h.DonGia ?? 0,
                            Hinh = h.Hinh ?? "default.png",
                            GiamGia = h.GiamGia
                        })
                        .ToList()
                })
                .ToList()
            };

            return View(model);
        }
    }
}
