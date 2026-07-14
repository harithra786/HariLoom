using hariloom.Helpers.DbContexts;
using hariloom.Helpers.Middlewares;

namespace nova_attire.Helpers.Middlewares
{
    public class SecureAccessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public SecureAccessMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            var token = context.Request.Cookies["hariloomAuthToken"];

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<appDBContext>();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var payload = TokenHelper.DecryptToken(token);

                    // Validate against database
                    var user = dbContext.mstUser.FirstOrDefault(u => u.mstUserId == payload.UserId && u.phoneNumber == payload.PhoneNumber && u.accessLevel == payload.AccessLevel && u.isActive);

                    if (user == null)
                    {
                        //await LogIllegalRequest(context, token, "User data mismatch", dbContext);
                        context.Response.Cookies.Delete("hariloomAuthToken");
                        context.Response.Redirect("/Auth/Login");
                        return;
                    }
                }
                catch
                {
                    //await LogIllegalRequest(context, token, "Token decryption failed", dbContext);
                    context.Response.Cookies.Delete("hariloomAuthToken");
                    context.Response.Redirect("/Auth/Login");
                    return;
                }
            }

            await _next(context);
        }

        //private async Task LogIllegalRequest(HttpContext context, string token, string reason, appDBContext dbContext)
        //{
        //    var ip = context.Connection.RemoteIpAddress?.ToString();
        //    var log = new trnIllegalRequest
        //    {
        //        IPAddress = ip ?? "unknown",
        //        Reason = reason,
        //        RawToken = token,
        //        PageAccessed = context.Request.Path
        //    };

        //    dbContext.trnIllegalRequest.Add(log);
        //    await dbContext.SaveChangesAsync();
        //}
    }
}
