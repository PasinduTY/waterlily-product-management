
using Microsoft.EntityFrameworkCore;
using WaterlilyProductManagement.Data;
using WaterlilyProductManagement.Interfaces;
using WaterlilyProductManagement.Repositories;
using WaterlilyProductManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC services.
builder.Services.AddControllersWithViews();

// Register the database context.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register memory caching.
builder.Services.AddMemoryCache();

// Register repositories.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// Register services.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// Serve static assets such as CSS and JavaScript.
app.MapStaticAssets();

// Configure the default MVC route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
