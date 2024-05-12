using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Motel.Utility.Checking
{
    public class Getter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Getter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetLoginId()
        {
            var user = _httpContextAccessor.HttpContext.User;

            return user.FindFirstValue(ClaimTypes.NameIdentifier);
            //return user != null ? user.FindFirstValue(ClaimTypes.NameIdentifier) : string.Empty;
        }
    }
}
