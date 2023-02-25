using Microsoft.AspNetCore.Mvc;

namespace ProgressAppAPI1.Controllers
{   
    [Route("[controller]")]
    [ApiController]
    public class HTTPController : ControllerBase
    {
        [HttpGet("GetIp", Name = "GetIp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> GetIpAddress()
        {
            string returnString = "HelloFromTheAPI";

            HttpContext context = HttpContext;
            if (context is not null)
            {

            }

            return Ok(returnString);
        }
    }

}
