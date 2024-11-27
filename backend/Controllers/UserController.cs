using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public string Get(string token)
        {
            using (var db = new Testappcontext())
            {
                if (db.IsAdmin(token))
                {
                    return "You are an admin";
                }
                return "You are not an admin";
            }
        }
    }
}
