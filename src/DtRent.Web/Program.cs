using Microsoft.EntityFrameworkCore;
using DtRent.Infrastructure.Persistence;
using DtRent.Application.ThueXe;
using DtRent.Application.TruyVan;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DtRentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DtRent")));
builder.Services.AddScoped<ThueXeDichVu>();
builder.Services.AddScoped<XeTruyVanDichVu>();
builder.Services.AddScoped<PhieuTruyVanDichVu>();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapGet("/ping-db", async (DtRentDbContext db) =>
    Results.Ok(new
    {
        xe = await db.Xes.CountAsync(),
        nhomXe = await db.NhomXes.CountAsync()
    }));
    
app.Run();
