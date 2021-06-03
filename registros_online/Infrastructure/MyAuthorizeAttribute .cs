using System;
using System.Web.Mvc;

namespace registros_online.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!((System.Web.HttpRequestWrapper)filterContext.HttpContext.Request).IsAuthenticated)
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        // put whatever data you want which will be sent
                        // to the client
                        timeout ="caducado",
                        message = "sorry, but you were logged out"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}