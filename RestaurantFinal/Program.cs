using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Data;
using Restaurant.Data.Repository;
using Restaurant.Data.Repository.IRepository;
using Restaurant.Models;
using Restaurant.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure the connection string from appsettings.json to use the database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add the UnitOfWork to the services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add user authentication with Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// Add email sender for user registration
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// For shopping cart authorization /////////////////////////////////////////////////////////////////////////////////////////////
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use Controllers for MenuItems and OrderList
app.MapControllers();

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets(); // static assets like CSS, JS, images, from wwwroot folder

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
