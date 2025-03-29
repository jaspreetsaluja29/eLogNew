using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Read BasePath from appsettings.json
var basePath = builder.Configuration.GetValue<string>("BasePath");

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DatabaseHelper>();

// Configure DataProtection to persist keys
var keysDirectory = Path.Combine(builder.Environment.ContentRootPath, "keys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysDirectory))
    .SetApplicationName("eLog");

// Configure Authentication with more robust settings
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Cookie.Name = "eLog.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        // Handle session expiry gracefully
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/Notification") &&
                    context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                }
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();

// Enhanced Session Configuration
builder.Services.AddDistributedMemoryCache(); // For development
                                              // For production, consider using a distributed cache:
/* Uncomment for SQL Server cache:
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = "dbo";
    options.TableName = "SessionCache";
});
*/

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120); // Increase timeout for longer operations
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "eLog.Session";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure it's sent over HTTPS
    options.Cookie.SameSite = SameSiteMode.None; // Allow cross-origin cookies
    options.Cookie.HttpOnly = true;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configure Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Apply the base path if it's set in the configuration
if (!string.IsNullOrEmpty(basePath))
{
    app.UsePathBase(basePath);
}

// Middleware order is important
app.UseHttpsRedirection();
app.UseStaticFiles();
// Add session before authentication to ensure session is initialized
app.UseSession();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

// Default route (non-area controllers)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.UseCors("AllowAll");
app.Run();