using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DiamondPharma.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Removed explicit URLs binding. Using default dynamic ports to avoid conflicts.

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddControllersWithViews();

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

// Disabled HTTPS redirection for local dev to avoid certificate issues
// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Default route: if pharmacy, go to Pharmacy/Pharmacy/Index; otherwise, Home/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Pharmacy}/{controller=Pharmacy}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "areas", 
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Route /Pharmacy to the catalog ordering page by default
app.MapControllerRoute(
    name: "Pharmacy",
    pattern: "Pharmacy/{controller=Pharmacy}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DiamondPharma.Data.SeedData.InitializeAsync(services);
}

// Seed catalog medicines
using (var scope = app.Services.CreateScope())
{
    DiamondPharma.Data.SeedCatalogMedicines.Initialize(scope.ServiceProvider);
}

app.Run();
