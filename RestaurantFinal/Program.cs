using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SavorHub.Data.Data;
using SavorHub.Data.Repository;
using SavorHub.Data.Repository.IRepository;
using SavorHub.Models;
using SavorHub.Utilities;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure the connection string from appsettings.json to use the database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Stripe settings from appsettings.json
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Add the UnitOfWork to the services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add user authentication with Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add email sender for user registration
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// For shopping cart authorization
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

app.UseHttpsRedirection();

// -----------------------------------------------------------------------
// Content Security Policy
// Fixes: NetworkError in hCaptcha hsw.js caused by the browser blocking
// the XHR to https://api.hcaptcha.com/checksiteconfig (zero transferSize,
// empty nextHopProtocol) when no connect-src / frame-src rules are present.
//
// This policy must also allow the CDN assets referenced in _Layout.cshtml
// (Bootstrap, jQuery, DataTables, TinyMCE, Toastr, SweetAlert2, icons)
// and, in Development, local WebSockets used by tooling.
// -----------------------------------------------------------------------
app.Use(async (context, next) =>
{
    var isDev = app.Environment.IsDevelopment();

    var csp =
        "default-src 'self'; " +
        "object-src 'none'; " +
        "base-uri 'self'; " +
        // 'unsafe-inline' required for the anti-flash theme script and TempData toastr blocks in _Layout.cshtml
        "script-src 'self' 'unsafe-inline' " +
            "https://code.jquery.com " +
            "https://cdn.jsdelivr.net " +
            "https://cdn.datatables.net " +
            "https://cdn.tiny.cloud " +
            "https://cdnjs.cloudflare.com; " +
        // 'unsafe-inline' for inline style attributes; fonts.googleapis.com for Bootswatch/Bootstrap Icons Google Fonts imports
        "style-src 'self' 'unsafe-inline' " +
            "https://cdnjs.cloudflare.com " +
            "https://cdn.jsdelivr.net " +
            "https://cdn.datatables.net " +
            "https://cdn.tiny.cloud " +
            "https://fonts.googleapis.com; " +
        "img-src 'self' data: https:; " +
        // fonts.gstatic.com serves the actual Google Font binary files
        "font-src 'self' data: https://fonts.gstatic.com https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
        "frame-src 'self' https://*.stripe.com https://newassets.hcaptcha.com; " +
        "connect-src 'self' " +
            "https://*.hcaptcha.com " +
            "https://*.stripe.com " +
            "https://*.stripe.network " +
            "https://code.jquery.com " +
            "https://cdn.jsdelivr.net " +
            "https://cdn.datatables.net " +
            "https://cdn.tiny.cloud " +
            "https://cdnjs.cloudflare.com" +
            (isDev ? " ws://localhost:* http://localhost:*" : "") +
            ";";

    context.Response.Headers["Content-Security-Policy"] = csp;
    await next();
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets(); // static assets like CSS, JS, images, from wwwroot folder

app.MapRazorPages()
   .WithStaticAssets();

// Controllers for MenuItems and Order APIs
app.MapControllers();

app.Run();
