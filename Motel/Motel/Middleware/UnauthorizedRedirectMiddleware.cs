using Microsoft.AspNetCore.Authorization;

namespace Motel.Middleware
{
    public class UnauthorizedRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _loginPath;

        public UnauthorizedRedirectMiddleware(RequestDelegate next, string loginPath)
        {
            _next = next;
            _loginPath = loginPath;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                // Lấy các endpoint mà người dùng yêu cầu
                var endpoint = context.GetEndpoint();

                // Nếu đường dẫn yêu cầu yêu cầu quyền RequireCustomer
                if (endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>()?.Policy == "RequireCustomer")
                {
                    // Chuyển hướng đến trang đăng nhập
                    context.Response.Redirect("/Account/Login");
                    return;
                }
            }

            await _next(context);
        }
    }
}
