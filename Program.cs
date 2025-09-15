using Microsoft.EntityFrameworkCore;
using OnlineStoreMVC.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Kết nối SQL Server
builder.Services.AddDbContext<OnlineStoreDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineStoreDB")));

// 2. Thêm MVC Controllers + Views
builder.Services.AddControllersWithViews();

// 3. Thêm Session (để lưu user + role)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Đăng ký IHttpContextAccessor để dùng trong Razor View
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// 5. Pipeline xử lý request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 6. Sử dụng Session
app.UseSession();

// 7. Định tuyến MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();