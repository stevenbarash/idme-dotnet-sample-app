using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using idme_dotnet_sample_app.Models;
using System.Security.Claims;

namespace idme_dotnet_sample_app.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            var userClaims = new UserClaims
            {
                GivenName = User.FindFirst(ClaimTypes.GivenName)?.Value,
                Surname = User.FindFirst(ClaimTypes.Surname)?.Value,
                SocialSecurity = User.FindFirst("social_security_number")?.Value,
                IdentityDocumentNumber = User.FindFirst("identity_document_number")?.Value,
                MobilePhone = User.FindFirst(ClaimTypes.MobilePhone)?.Value,
                PostalCode = User.FindFirst(ClaimTypes.PostalCode)?.Value,
                StateOrProvince = User.FindFirst(ClaimTypes.StateOrProvince)?.Value,
                Uuid = User.FindFirst("uuid")?.Value
            };
            
            return View(userClaims);
        }
    }
}