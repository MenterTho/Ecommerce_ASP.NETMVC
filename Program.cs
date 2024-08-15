using Ecommerce.Data;
using Ecommerce.Helpers;
using Ecommerce.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình dịch vụ và dịch vụ cần thiết
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// 2. Cấu hình ghi log và kiểm tra kết nối cơ sở dữ liệu
ConfigureLoggingAndDatabase(app);

// 3. Cấu hình middleware (phần xử lý HTTP request)
ConfigureMiddleware(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Thêm dịch vụ cho Controllers và Views
    services.AddControllersWithViews();
    services.AddScoped<HangHoaService>();
    services.AddScoped<CartService>();

    // Cấu hình AutoMapper để ánh xạ các đối tượng
    services.AddAutoMapper(typeof(AutoMapperProfile));

    // Cấu hình DbContext với SQL Server
    services.AddDbContext<Hshop2023Context>(options =>
        options.UseSqlServer(configuration.GetConnectionString("HShop")));

    // Cấu hình session
    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian session hết hạn sau 10 phút
        options.Cookie.HttpOnly = true; // Chỉ cho phép truy cập cookie từ phía server
        options.Cookie.IsEssential = true; // Đánh dấu cookie này là cần thiết cho ứng dụng
    });

    // Cấu hình xác thực bằng cookie
    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Register/DangNhap"; // Đường dẫn tới trang đăng nhập
                options.AccessDeniedPath = "/AccessDenied"; // Đường dẫn tới trang lỗi truy cập bị từ chối
            });
}

void ConfigureLoggingAndDatabase(WebApplication app)
{
    // Cấu hình ghi log
    var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger<Program>();

    // Kiểm tra kết nối cơ sở dữ liệu và ghi log
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<Hshop2023Context>();
        try
        {
            if (dbContext.Database.CanConnect())
            {
                logger.LogInformation("Kết nối cơ sở dữ liệu thành công."); // Ghi log khi kết nối thành công
            }
            else
            {
                logger.LogError("Kết nối cơ sở dữ liệu không thành công."); // Ghi log khi kết nối thất bại
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kết nối cơ sở dữ liệu không thành công với lỗi."); // Ghi log lỗi khi kết nối cơ sở dữ liệu gặp sự cố
        }
    }
}

void ConfigureMiddleware(WebApplication app)
{
    // Cấu hình pipeline xử lý request HTTP
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error"); // Cấu hình trang lỗi khi xảy ra ngoại lệ trong môi trường không phải phát triển
        app.UseHsts(); // Cấu hình HSTS (HTTP Strict Transport Security)
    }

    app.UseHttpsRedirection(); // Chuyển hướng tất cả các request từ HTTP sang HTTPS
    app.UseStaticFiles(); // Cung cấp các file tĩnh từ thư mục wwwroot
    app.UseSession(); // Cấu hình middleware session để quản lý dữ liệu session

    app.UseRouting(); // Cấu hình middleware routing để xử lý các request dựa trên route

    app.UseAuthentication(); // Cấu hình middleware xác thực
    app.UseAuthorization(); // Cấu hình middleware phân quyền

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"); // Cấu hình route mặc định cho ứng dụng
}