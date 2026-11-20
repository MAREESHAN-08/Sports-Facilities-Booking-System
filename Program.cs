using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using SportsBookingApp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// QuestPDF community licence (needed for PDF receipt)
QuestPDF.Settings.License = LicenseType.Community;

// Entity Framework Core DbContext
builder.Services.AddDbContext<SportsBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories (Data layer)
builder.Services.AddScoped<MemberRepository>();
builder.Services.AddScoped<FacilityRepository>();
builder.Services.AddScoped<BookingRepository>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<InquiryRepository>();

// In-memory cache for rate limiting
builder.Services.AddMemoryCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();