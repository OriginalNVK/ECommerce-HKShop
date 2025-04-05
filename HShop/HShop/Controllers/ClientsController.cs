using HShop.Data;
using HShop.Helpers;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HShop.Controllers
{
	public class ClientsController : Controller
	{
		private readonly Hshop2023Context db;

		public ClientsController(Hshop2023Context db)
        {
			this.db = db;
		}

		[HttpGet]
		[Route("/admin/client/update/{id}")]
        public async Task<IActionResult> Update(string? id)
		{
			var Client = await db.KhachHangs.FirstOrDefaultAsync(c => c.MaKh == id);
			// DTO
			var result = new ClientVM
			{
				MaKH = Client.MaKh,
				HoTen = Client.HoTen,
				GioiTinh = Client.GioiTinh,
				NgaySinh = Client.NgaySinh,
				DienThoai = Client.DienThoai,
				DiaChi = Client.DiaChi,
				Email = Client.Email,
				Hinh = Client.Hinh,
			};
			return View(result);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit([Bind("MaKH, HoTen, GioiTinh, NgaySinh, DiaChi, DienThoai, Email")] ClientVM Client,
			IFormFile Hinh)
		{
			if (ModelState.IsValid)
			{
				// Xử lý lưu ảnh
				if (Hinh != null && Hinh.Length > 0)
				{
					Client.Hinh = MyUtil.UpLoadHinh(Hinh, "HangHoa");
				}

				var ExistsClient = await db.KhachHangs.FirstOrDefaultAsync(c => c.MaKh == Client.MaKH);
				if (ExistsClient != null)
				{
					ExistsClient.HoTen = Client.HoTen;
					ExistsClient.GioiTinh = Client.GioiTinh;
					ExistsClient.NgaySinh = Client.NgaySinh;
					ExistsClient.DienThoai = Client.DienThoai;
					ExistsClient.DiaChi = Client.DiaChi;
					ExistsClient.Email = Client.Email;
					if(Client.Hinh != null)
					{
						ExistsClient.Hinh = Client.Hinh;
					}
					db.KhachHangs.Update(ExistsClient);
					await db.SaveChangesAsync();
					return Redirect("/admin/clients");
				}
				return NotFound();
			}
			return Redirect("/admin/clients/update/" + Client.MaKH);
		}
	}
}
