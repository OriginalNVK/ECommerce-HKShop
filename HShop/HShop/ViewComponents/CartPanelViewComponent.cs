using HShop.Data;
using HShop.Helpers;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HShop.ViewComponents
{
    public class CartPanelViewComponent : ViewComponent
    {
        private readonly Hshop2023Context context;

        public CartPanelViewComponent(Hshop2023Context context)
        {
            this.context = context;
        }
        public IViewComponentResult Invoke()
        {
            var maKH = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst(MyConstant.CLAIM_CUSTOMERID)?.Value : null;
            if(maKH == null)
            {
                return View(new CartModel
                {
                    Quantity = 0,
                    Total = 0,
                    Items = new List<CartItem>()
                });
            }

            var cartItems = context.Carts.Where(c => c.MaKh == maKH).Select(c => new CartItem
            {
                MaHH = c.MaHh,
                TenHH = c.MaHhNavigation.TenHh,
                DonGia = c.DonGia,
                SoLuong = c.SoLuong,
                Hinh = c.MaHhNavigation.Hinh
            }).ToList();
            return View(new CartModel()
            {
                Quantity = cartItems.Sum(p => p.SoLuong),
                Total = cartItems.Sum(p=>p.ThanhTien),
                Items = cartItems
            });
        }
    }
}
