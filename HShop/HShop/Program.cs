using HShop.Data;
using HShop.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace HShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<Hshop2023Context>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("HShop"));
            });

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/KhachHang/DangNhap";
                options.AccessDeniedPath = "/AccessDenied"; // Đã đăng nhập rồi 
            });

            // Đăng ký paypalClient dạng Singleton() -  Chỉ 1 instance duy nhất trên toàn ứng dụng
            builder.Services.AddSingleton(x => new PaypalClient(
                builder.Configuration["PaypalOption:AppID"],
                builder.Configuration["PaypalOption:AppSecret"],
                builder.Configuration["PaypalOption:Mode"]
            ));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
