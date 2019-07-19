using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace awesomeappAPI.Controllers
{
    [Route("api/notifications")]
    [Authorize]
    [ApiController]
    public class AwesomeController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AwesomeController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetNotifications()
        {
            var claims = httpContextAccessor.HttpContext.User.Claims;
            var tenant = claims?.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
            var username = claims?.FirstOrDefault(c => c.Type == "name")?.Value;
            var tenants = FakeDatabaseService.Tenants;

            if (tenant == tenants.ElementAt(0))
            {
                return FakeDatabaseService.Tenant1Database[username];
            }
            else
            {
                return FakeDatabaseService.Tenant2Database[username];
            }
        }
    }
}
