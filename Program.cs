using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using CNPM;
using CNPM.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
IConfiguration Configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
Utils.rootPath = builder.Environment.ContentRootPath + "\\wwwroot";
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddOptions();
builder.Services.AddRazorPages();

//local host
builder.WebHost.UseKestrel(options =>
{
    options.Listen(System.Net.IPAddress.Any, 7226, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});


//email
var mailsettings = Configuration.GetSection("MailSettings");  // đọc config
builder.Services.Configure<MailSettings>(mailsettings);               // đăng ký để Inject
builder.Services.AddTransient<IEmailSender, SendMailService>();        // Đăng ký dịch vụ Mail


//logger
ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());


//DBContext
builder.Services.AddDbContext<AppDbContext>(options => {
    // Đọc chuỗi kết nối
    string connectstring = Configuration.GetConnectionString("AppDbContext")!;
    // Sử dụng MS SQL Server
    options.UseSqlServer(connectstring).UseLoggerFactory(loggerFactory);
    });


//Identity
builder.Services.AddIdentity<TaiKhoan, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


//Google
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        // Đọc thông tin Authentication:Google từ appsettings.json
        IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");

        // Thiết lập ClientID và ClientSecret để truy cập API google
        googleOptions.ClientId = googleAuthNSection["ClientId"]!;
        googleOptions.ClientSecret = googleAuthNSection["ClientSecret"]!;
        // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
        googleOptions.CallbackPath = "/dang-nhap-tu-google";

    });


// Configure options
// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // Khóa 1 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true; // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedAccount = true; // Xác thực tài khoản
    options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại

});
builder.Services.ConfigureApplicationCookie(options => {
     options.Cookie.HttpOnly = true;
     options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = $"/login/";
    options.LogoutPath = $"/logout/";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.Configure<RouteOptions>(options => {
    options.LowercaseUrls = true;                   // url chữ thường
    options.LowercaseQueryStrings = false;          // không bắt query trong url phải in thường
});

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var _userStore = services.GetRequiredService<IUserStore<TaiKhoan>>();
    var _userManager = services.GetRequiredService<UserManager<TaiKhoan>>();
    Utils.SeedData(context, Configuration, _userStore, _userManager);
}

app.Run();