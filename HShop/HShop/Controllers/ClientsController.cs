using AutoMapper;
using HShop.Data;
using HShop.Helpers;
using HShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace HShop.Controllers
{
    public class ClientsController : Controller
    {
        private const string ToastMessage = "ToastMessage";
        private const string ToastType = "ToastType";
        private const string ToastTypeSuccess = "success";
        private const string ToastTypeError = "error";
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;

        public ClientsController(Hshop2023Context db, IMapper mapper)
        {
            this.db = db;
            this._mapper = mapper;
        }

        [HttpGet]
        [Route("/admin/clients/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(ClientVM client, IFormFile? image)
        {
            var clientExist = await db.KhachHangs.FirstOrDefaultAsync(k => k.MaKh == client.MaKH);
            if (clientExist != null)
            {
                TempData[ToastMessage] = "Người dùng đã tồn tại (username đã tồn tại)";
                TempData[ToastType] = ToastTypeError;
                return RedirectToAction("Create");
            }

            if (!ModelState.IsValid)
            {
                TempData[ToastMessage] = "Dữ liệu không hợp lệ";
                TempData[ToastType] = ToastTypeError;
                return RedirectToAction("Create");
            }
            try
            {
                var clientInfo = _mapper.Map<KhachHang>(client);
                clientInfo.RandomKey = MyUtil.GenerateRandomKey();
                clientInfo.MatKhau = client.MatKhau.ToMd5Hash(clientInfo.RandomKey);
                clientInfo.HieuLuc = true; // sẽ xử lý khi dùng mail để active

                if (image != null)
                {
                    clientInfo.Hinh = MyUtil.UpLoadHinh(image, "KhachHang");
                }

                await db.KhachHangs.AddAsync(clientInfo);
                await db.SaveChangesAsync();
                TempData[ToastMessage] = "Tạo người dùng thành công";
                TempData[ToastType] = ToastTypeSuccess;
                return Redirect("/admin/clients");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData[ToastMessage] = "Lỗi hệ thống khi thêm khách hàng";
                TempData[ToastType] = ToastTypeError;
                return RedirectToAction("Create");
            }

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
            IFormFile? Hinh)
        {
            if (ModelState.IsValid)
            {
                // Xử lý lưu ảnh
                if (Hinh != null && Hinh.Length > 0)
                {
                    Client.Hinh = MyUtil.UpLoadHinh(Hinh, "KhachHang");
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
                    if (Client.Hinh != null)
                    {
                        ExistsClient.Hinh = Client.Hinh;
                    }
                    db.KhachHangs.Update(ExistsClient);
                    await db.SaveChangesAsync();
                    TempData[ToastMessage] = "Cập nhật thông tin người dùng thành công";
                    TempData[ToastType] = ToastTypeSuccess;
                    return Redirect("/admin/clients");
                }
                TempData[ToastMessage] = "Không tìm thấy người dùng";
                TempData[ToastType] = ToastTypeError;
                return NotFound();
            }
            TempData[ToastMessage] = "Cập nhật thông tin dùng thất bại";
            TempData[ToastType] = ToastTypeError;
            return Redirect("/admin/clients/update/" + Client.MaKH);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var Client = await db.KhachHangs.SingleOrDefaultAsync(k => k.MaKh == id);
            if (Client != null)
            {
                var HoaDons = await db.HoaDons.Where(h => h.MaKh == id).ToListAsync();
                foreach (var h in HoaDons)
                {
                    var Cthhs = await db.ChiTietHds.Where(h => h.MaHd == h.MaHd).ToListAsync();
                    db.ChiTietHds.RemoveRange(Cthhs);
                    db.Remove(h);
                    await db.SaveChangesAsync();
                }
                db.KhachHangs.Remove(Client);
                await db.SaveChangesAsync();
                TempData[ToastMessage] = "Xóa người dùng thành công";
                TempData[ToastType] = ToastTypeSuccess;
                return Redirect("/admin/clients");
            }
            TempData[ToastMessage] = "Không tồn tại người dùng";
            TempData[ToastType] = ToastTypeError;
            return NotFound();

        }
    }
}
