using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HShop.Data;
using HShop.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HShop.Controllers
{
	[Authorize(Roles = "1")]
	public class ProductsController : Controller
	{
		private readonly Hshop2023Context db;

		public ProductsController(Hshop2023Context db)
		{
			this.db = db;
		}

		// GET: HangHoas/Details/5
		[Route("/admin/products/detail/{id}")]
		public async Task<IActionResult> Detail(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await db.HangHoas
				.Include(h => h.MaLoaiNavigation)
				.Include(h => h.MaNccNavigation)
				.FirstOrDefaultAsync(m => m.MaHh == id);

			if (product == null)
			{
				return NotFound();
			}

			var ProductDTO = new ProductAdminVM
			{
				MaHh = product.MaHh,
				TenHh = product.TenHh,
				TenAlias = product.TenAlias,
				MaLoai = product.MaLoai,
				MoTaDonVi = product.MoTaDonVi,
				DonGia = product.DonGia,
				Hinh = product.Hinh,
				NgaySx = product.NgaySx,
				GiamGia = product.GiamGia,
				SoLanXem = product.SoLanXem,
				MoTa = product.MoTa,
				MaNcc = product.MaNcc,
				MaLoaiNavigation = product.MaLoaiNavigation,
				MaNccNavigation = product.MaNccNavigation
			};

			return View(ProductDTO);
		}

		// GET: HangHoas/Create
		public IActionResult Create()
		{
			ViewData["MaLoai"] = new SelectList(db.Loais, "MaLoai", "MaLoai");
			ViewData["MaNcc"] = new SelectList(db.NhaCungCaps, "MaNcc", "MaNcc");
			return View();
		}

		// POST: HangHoas/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("MaHh,TenHh,TenAlias,MaLoai,MoTaDonVi,DonGia,Hinh,NgaySx,GiamGia,SoLanXem,MoTa,MaNcc")] HangHoa hangHoa)
		{
			if (ModelState.IsValid)
			{
				db.Add(hangHoa);
				await db.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["MaLoai"] = new SelectList(db.Loais, "MaLoai", "MaLoai", hangHoa.MaLoai);
			ViewData["MaNcc"] = new SelectList(db.NhaCungCaps, "MaNcc", "MaNcc", hangHoa.MaNcc);
			return View(hangHoa);
		}

		// GET: HangHoas/Edit/5
		[Route("admin/products/update/{id}")]
		public async Task<IActionResult> Update(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await db.HangHoas.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			var ProductDTO = new ProductAdminVM
			{
				MaHh = product.MaHh,
				TenHh = product.TenHh,
				TenAlias = product.TenAlias,
				MaLoai = product.MaLoai,
				MoTaDonVi = product.MoTaDonVi,
				DonGia = product.DonGia,
				Hinh = product.Hinh,
				NgaySx = product.NgaySx,
				GiamGia = product.GiamGia,
				SoLanXem = product.SoLanXem,
				MoTa = product.MoTa,
				MaNcc = product.MaNcc,
				MaLoaiNavigation = product.MaLoaiNavigation,
				MaNccNavigation = product.MaNccNavigation
			};
			ViewData["MaLoai"] = new SelectList(db.Loais, "MaLoai", "MaLoai", product.MaLoai);
			ViewData["MaNcc"] = new SelectList(db.NhaCungCaps, "MaNcc", "MaNcc", product.MaNcc);
			return View(ProductDTO);
		}

		// POST: HangHoas/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("MaHh,TenHh,TenAlias,MaLoai,MoTaDonVi,DonGia,Hinh,NgaySx,GiamGia,SoLanXem,MoTa,MaNcc")] ProductAdminVM product)
		{
			if (id != product.MaHh)
			{
				return NotFound();
			}
			try
			{
				var existingProduct = await db.HangHoas.FirstOrDefaultAsync(h => h.MaHh == id);
				// Gán các giá trị từ ViewModel sang Entity
				existingProduct.TenHh = product.TenHh;
				existingProduct.TenAlias = product.TenAlias;
				existingProduct.MaLoai = product.MaLoai;
				existingProduct.MoTaDonVi = product.MoTaDonVi;
				existingProduct.DonGia = product.DonGia;
				if(product.Hinh != null)
				{
					existingProduct.Hinh = product.Hinh;
				}
				existingProduct.NgaySx = product.NgaySx;
				existingProduct.GiamGia = product.GiamGia;
				existingProduct.SoLanXem = product.SoLanXem;
				existingProduct.MoTa = product.MoTa;
				existingProduct.MaNcc = product.MaNcc;

				// Cập nhật và lưu thay đổi
				db.HangHoas.Update(existingProduct);
				await db.SaveChangesAsync();

				return Redirect("/admin/products");
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!HangHoaExists(product.MaHh))
				{
					return NotFound();
				}
				else
				{
					TempData["Message"] = "Model state is invalid";
					return Redirect("/admin");
				}
			}
		}

		// GET: HangHoas/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var hangHoa = await db.HangHoas
				.Include(h => h.MaLoaiNavigation)
				.Include(h => h.MaNccNavigation)
				.FirstOrDefaultAsync(m => m.MaHh == id);
			if (hangHoa == null)
			{
				return NotFound();
			}

			return View(hangHoa);
		}

		// POST: HangHoas/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var hangHoa = await db.HangHoas.FindAsync(id);
			if (hangHoa != null)
			{
				db.HangHoas.Remove(hangHoa);
			}

			await db.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool HangHoaExists(int id)
		{
			return db.HangHoas.Any(e => e.MaHh == id);
		}
	}
}
