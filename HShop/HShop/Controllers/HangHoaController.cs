using Microsoft.AspNetCore.Mvc;

namespace HShop.Controllers
{
    public class HangHoaController : Controller
    {
        public IActionResult Index(int? loai)
        {
            return View();
        }
    }
}
