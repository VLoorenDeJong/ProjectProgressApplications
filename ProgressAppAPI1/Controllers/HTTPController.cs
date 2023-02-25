using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ProgressAppAPI1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HTTPController : ControllerBase
    {
        [HttpGet("GetIp", Name = "GetIp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> GetIpAddress()
        {
            HttpContext context = HttpContext;
            string? outputString = string.Empty;

            outputString = HttpContext?.Connection?.RemoteIpAddress?.ToString();

            // check if it's an IPv6 address and convert to IPv4 format if necessary
            if (!string.IsNullOrWhiteSpace(outputString))
            {
                if (outputString.Contains(":"))
                {
                    outputString = HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4().ToString();
                }
            }

            // check for the X-Forwarded-For header to get the client IP address behind the proxy
            string? xForwardedForHeader = HttpContext?.Request?.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(xForwardedForHeader))
            {
                string[] addresses = xForwardedForHeader.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (addresses.Length > 0)
                {
                    outputString = addresses[0].Trim();
                }
            }

            if (!string.IsNullOrEmpty(outputString))
            {
                return outputString;
            }
            else return "What?!?";
            
        }
    }

}
