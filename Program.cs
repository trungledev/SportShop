using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        //options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>{
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});
// Disable mail
////Đăng ký dịch vụ mail
//var mailSettings = builder.Configuration.GetSection("MailSettings");
//builder.Services.Configure<MailSettings>(mailSettings);
//builder.Services.AddTransient<IEmailSender,SendMailService>();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Disable authentication with 
//builder.Services.AddAuthentication()
//    .AddFacebook(facebookOptions =>
//    {
//        facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
//        facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
//        facebookOptions.CallbackPath = "/dang-nhap-tu-facebook";
//    });
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("RoleAdmin", policy => policy.RequireClaim("Admin"));
});
// Disable cookie
//builder.Services.Configure<CookiePolicyOptions>(options =>
//{
//    // This lambda determines whether user consent for non-essential 
//    // cookies is needed for a given request.
//    options.CheckConsentNeeded = context => true;

//    options.MinimumSameSitePolicy = SameSiteMode.None;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");
app.MapRazorPages();

app.UseCookiePolicy();

app.Run();
