using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using webApiTest;

namespace registros_online
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                //These headers are handling the "pre-flight" OPTIONS call sent by the browser
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow‌​-Credentials", "true");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);

                FormsIdentity formsIdentity = new FormsIdentity(ticket);

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(formsIdentity);

                //    var user = this.UserService.GetUserByEmail(ticket.Name);
                registros_online.Models.dbContext dbContext = new registros_online.Models.dbContext();
                var user = dbContext.getUsuarioById(int.Parse(ticket.UserData));
                string[] arrayOfRoles = new string[1];
                if (user.IsSuperUser)
                {
                    arrayOfRoles[0] = "Admin";
                    foreach (var role in arrayOfRoles)
                   
                        claimsIdentity.AddClaim(
                            new Claim(ClaimTypes.Role, role));
                }

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.Current.User = claimsPrincipal;
            }
        }
    }
}
