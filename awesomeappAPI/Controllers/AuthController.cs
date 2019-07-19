using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace awesomeappAPI.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        [HttpGet]
        [Route("tenants")]
        public IEnumerable<string> Tenants()
        {
            return FakeDatabaseService.Tenants;
        }
    }
}
