using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using hariloom.Helpers.Middlewares;

namespace nova_attire.Helpers.Middlewares
{
    public class RestrictToAccessLevelAttribute : ActionFilterAttribute
    {
        private readonly int[] _allowedLevels;

        public RestrictToAccessLevelAttribute(params int[] allowedLevels)
        {
            _allowedLevels = allowedLevels;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Cookies["hariloomAuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                // No token - redirect to login
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            try
            {
                var payload = TokenHelper.DecryptToken(token); // assumes this returns an object with AccessLevel
                int accessLevel = payload.AccessLevel;

                if (!_allowedLevels.Contains(accessLevel))
                {
                    // Unauthorized access
                    context.Result = new RedirectToActionResult("Home", "Website", null);
                    return;
                }
            }
            catch
            {
                // Invalid token - redirect to login
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }

}
