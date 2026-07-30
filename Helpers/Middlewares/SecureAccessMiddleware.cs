using hariloom.Helpers.DbContexts;
using hariloom.Helpers.Middlewares;
using hariloom.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace nova_attire.Helpers.Middlewares
{
    public class SecureAccessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        // Admin-only route prefixes
        private static readonly string[] AdminRoutePrefixes = new[]
        {
            "/dashboard",
            "/productsmanagement",
            "/ordermanagement"
        };

        public SecureAccessMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var token = context.Request.Cookies["hariloomAuthToken"];

            bool isAdminRoute = AdminRoutePrefixes.Any(prefix => path.StartsWith(prefix));

            hariloom.Models.Entity.mstUser? authenticatedUser = null;

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var payload = TokenHelper.DecryptToken(token);

                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<appDBContext>();

                    // Validate against database
                    authenticatedUser = await dbContext.mstUser.FirstOrDefaultAsync(u =>
                        u.mstUserId == payload.UserId &&
                        u.isActive);

                    if (authenticatedUser == null)
                    {
                        context.Response.Cookies.Delete("hariloomAuthToken");
                    }
                }
                catch
                {
                    context.Response.Cookies.Delete("hariloomAuthToken");
                }
            }

            // Enforce Admin Access for Admin routes
            if (isAdminRoute)
            {
                if (authenticatedUser == null)
                {
                    var returnUrl = context.Request.Path.Value + context.Request.QueryString.Value;
                    context.Response.Redirect($"/Auth/Login?returnUrl={Uri.EscapeDataString(returnUrl)}");
                    return;
                }

                if (authenticatedUser.accessLevel != (int)accessLevelEnum.AdminUser)
                {
                    context.Response.Redirect("/Website/Home");
                    return;
                }
            }

            await _next(context);
        }
    }
}
