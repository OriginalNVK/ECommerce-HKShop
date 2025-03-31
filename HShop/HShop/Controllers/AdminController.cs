using HShop.Data;
using Microsoft.AspNetCore.Mvc;

namespace HShop.Controllers
{
	public class AdminController : Controller
	{
		private readonly Hshop2023Context db;

		public AdminController(Hshop2023Context db)
        {
			this.db = db;
		}
        public IActionResult Index()
		{
			return View();
		}
	}
}
