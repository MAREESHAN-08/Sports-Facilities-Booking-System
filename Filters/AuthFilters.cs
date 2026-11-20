using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SportsBookingApp.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class MemberOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Session.GetInt32("MemberID") == null)
        {
            var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
        }
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AdminOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Session.GetString("Admin") == null)
            context.Result = new RedirectToActionResult("Login", "Admin", null);
    }
}