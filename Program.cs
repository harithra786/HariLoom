using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using nova_attire.Helpers.Middlewares;
using hariloom.Helpers.DbContexts;
using hariloom.Interfaces;
using hariloom.Repository;
using hariloom.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Increase Kestrel request body size limit to 100 MB
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100 MB
});

// Increase request/upload size limit to 100 MB
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<appDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySQLConnectionString"),
        new MySqlServerVersion(new Version(8, 0, 36))
    ));

builder.Services.AddScoped<IApiResponseRepository, ApiResponseRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IProductsManagementRepository, ProductsManagementRepository>();
builder.Services.AddScoped<IMstUserRepository, MstUserRepository>();
builder.Services.AddScoped<IOrderManagementRepository, OrderManagementRepository>();
builder.Services.AddScoped<IWebsiteUserRepository, WebsiteUserRepository>();

builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

app.UseHttpsRedirection();

// Configure MIME types for static files
var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
provider.Mappings[".mp4"] = "video/mp4";
provider.Mappings[".webm"] = "video/webm";
provider.Mappings[".ogv"] = "video/ogg";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<SecureAccessMiddleware>();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=ProductMainCategory}/{action=ProductMainCategory}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Website}/{action=Home}/{id?}");

app.Run();