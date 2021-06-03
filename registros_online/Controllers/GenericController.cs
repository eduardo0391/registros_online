using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
using registros_online.Models;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace registros_online.Controllers
{
    public class GenericController : Controller
    {
 
        public int idUsuario()
        {
            int id = 0;
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                CustomIdentity identity = new CustomIdentity(authTicket.Name, authTicket.UserData, authTicket.UserData=="1"?true:false);
                GenericPrincipal newUser = new GenericPrincipal(identity, new string[] {"Admin" });
                id = identity.Id;
                
            }
            return id;
        }

        public void setCooking(User vuser)
        {
            string[] arrayOfRoles= new string[1];
            if (vuser.IsSuperUser)
                arrayOfRoles[0] = "Admin";
            var authTicket = new FormsAuthenticationTicket(1, //version
                                                        vuser.name, // user name
                                                        DateTime.Now,             //creation
                                                        DateTime.Now.AddMinutes(30), //Expiration
                                                        false, //Persistent
                                                        vuser.id_user.ToString(),
                                                        String.Join("|", arrayOfRoles));

            var encTicket = FormsAuthentication.Encrypt(authTicket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }

        public dynamic controlErrores()
        {
            dynamic aux;
            string[] camposConError = ModelState.Keys
                    .Where(e => ModelState[e].Errors.Any())
                    .Select(e => e)
                    .ToArray();

            IList<dynamic> errores = new List<dynamic>();
            ArrayList alErrores = new ArrayList();

            foreach (string item in camposConError)
            {
                errores.Add(new { key = item, error = ModelState[item].Errors.First().ErrorMessage });
                alErrores.Add(ModelState[item].Errors.First().ErrorMessage);
            }

            aux = new
            {
                key = "error",
                descripcion = string.Join("<br/>", alErrores.ToArray()),
                //  F_PCME_HH= model.F_PCME_HH,
                summary = errores
            };
            return aux;
        }


       
    }
}