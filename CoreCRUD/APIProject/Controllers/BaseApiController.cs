using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIProject.Controllers
{
    public class BaseApiController : ControllerBase
    {
        //protected Guid UserID => int.Parse(FindClaim(ClaimTypes.NameIdentifier));
        private string FindClaim(string claimName)
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var claim = claimsIdentity.FindFirst(claimName);

            if (claim == null)
            {
                return null;
            }
            return claim.Value;
        }
    }
}
