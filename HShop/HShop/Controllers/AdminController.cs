using HShop.Data;
using HShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HShop.Controllers
{
    [Authorize(Roles = "1")]
    public class AdminController : Controller
    {
        private readonly Hshop2023Context db;

        public AdminController(Hshop2023Context db)
        {
            this.db = db;
        }

        [Route("/admin")]
        public async Task<IActionResult> Index()
        {
            var slDonHang = await db.HoaDons.CountAsync();
            var slHangHoa = await db.HangHoas.CountAsync();
            var result = new ThongKeVM
            {
                SlDonHang = slDonHang,
                SlHangHoa = slHangHoa
            };
            return View(result);
        }

        [Route("/admin/orders")]
        public async Task<IActionResult> Orders()
        {
            var allOrders = await db.HoaDons.OrderByDescending(h => h.NgayDat).ToListAsync();
            var result = (from i in allOrders
                          join k in db.KhachHangs on i.MaKh equals k.MaKh into khachHangGroup
                          from kh in khachHangGroup.DefaultIfEmpty()
                          join t in db.TrangThais on i.MaTrangThai equals t.MaTrangThai into trangThaiGroup
                          from tt in trangThaiGroup.DefaultIfEmpty()
                          select new InvoiceVM
                          {
                              MaHd = i.MaHd,
                              HoTen = i.HoTen ?? kh.HoTen,
                              NgayDat = i.NgayDat,
                              DiaChi = i.DiaChi,
                              CachThanhToan = i.CachThanhToan,
                              CachVanChuyen = i.CachVanChuyen,
                              TrangThai = tt.MoTa,
                              GhiChu = i.GhiChu ?? "",
                              DienThoai = i.DienThoai ?? kh.DienThoai
                          }).ToList();
            return View(result);
        }

        [Route("/admin/products")]
        public async Task<IActionResult> Products(int pageNumber = 1, int pageSize = 5, int? maLoai = null)
        {
            var HangHoas = db.HangHoas.Include(h=> h.MaLoaiNavigation).AsQueryable();
            if(maLoai.HasValue)
            {
                HangHoas = HangHoas.Where(h => h.MaLoai == maLoai.Value);
            }
            var totalProducts = await HangHoas.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var HangHoaDTO = await HangHoas.Skip((pageNumber - 1)*pageSize).Take(pageSize).Select(h => new HangHoaVM
            {
                MaHh = h.MaHh,
                TenHH = h.TenHh,
                MoTaNgan = !string.IsNullOrEmpty(h.MoTa) ? h.MoTa : "Không có mô tả",
				Hinh = !string.IsNullOrEmpty(h.Hinh) ? h.Hinh : null,
                DonGia = h.DonGia ?? 0.0,
                GiamGia = h.GiamGia,
                TenLoai = h.MaLoaiNavigation.TenLoai,
            }).ToListAsync();
            var Categories = await db.Loais.Select(l => new MenuLoaiVM
            {
                MaLoai = l.MaLoai,
                TenLoai = l.TenLoai,
                SoLuong = l.HangHoas.Count
            }).ToListAsync();

            ViewBag.Categories = Categories;
			ViewBag.CurrentPage = pageNumber;
			ViewBag.TotalPages = totalPages;
			ViewBag.PageSize = pageSize;
			ViewBag.CurrentMaLoai = maLoai;

			return View(HangHoaDTO);
        }

		[Route("/admin/clients")]
		public async Task<IActionResult> Clients(int pageNumber = 1, int pageSize = 5, int? VaiTro = null)
		{
			// Lọc theo VaiTro nếu có
			var query = db.KhachHangs.AsQueryable();

			if (VaiTro.HasValue)
			{
				query = query.Where(c => c.VaiTro == VaiTro.Value);
			}

			// Phân trang
			var pagedList = await query
				.Select(c => new ClientVM
				{
                    MaKH = c.MaKh,
					HoTen = c.HoTen,
					Hinh = c.Hinh,
                    GioiTinh = c.GioiTinh,
                    NgaySinh = c.NgaySinh,
                    DienThoai = c.DienThoai,
                    DiaChi = c.DiaChi,
                    Email = c.Email,
				})
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			// Lấy danh sách các VaiTro duy nhất
			var uniqueRoles = await db.KhachHangs
				.Select(c => c.VaiTro)
				.Distinct()
				.ToListAsync();

			ViewBag.Roles = uniqueRoles;
			ViewBag.PageNumber = pageNumber;
			ViewBag.PageSize = pageSize;
			ViewBag.TotalCount = await query.CountAsync();

			return View(pagedList);
		}

		[Route("/admin/categories")]
        public async Task<IActionResult> Categories()
        {
            var ListCategories = db.Loais.AsQueryable();
            var result = await ListCategories.Select(c => new CategoryVM
            {
                MaLoai = c.MaLoai,
                TenLoai = c.TenLoai,
                TenLoaiAlias = c.TenLoaiAlias,
                MoTa = c.MoTa,
                Hinh = c.Hinh,
            }).ToListAsync();
            return View(result);
        }

        [Route("/admin/suppliers")]
        public async Task<IActionResult> Suppliers()
        {
            var listSuppliers = db.NhaCungCaps.AsQueryable();
            var result = await listSuppliers.Select(s => new SupplierVM
            {
                MaNcc = s.MaNcc,
                TenCongTy = s.TenCongTy,
                Logo = s.Logo,
                NguoiLienLac = s.NguoiLienLac,
                Email = s.Email,
                DienThoai = s.DienThoai,
                DiaChi = s.DiaChi,
                MoTa = s.MoTa,
            }).ToListAsync();
            return View(result);
        }
    }
}
