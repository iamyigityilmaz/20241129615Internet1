using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HaberPortali2.Attributes
{
    public class AdminAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var claim = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "IsAdmin");

            if (claim == null || claim.Value != "True")
            {
                context.Result = new RedirectToActionResult("Denied", "Auth", null);
            }
        }
    }
}
