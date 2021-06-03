using registros_online.Models;
using System.Linq;
using System.Web.Http;
using System;
using System.Collections.Generic;
using System.Web.Http.ModelBinding;
namespace registros_online.Controllers.Api
{
    [RoutePrefix("api/User")]
    public class UserController : DefaultController
    {
        dbContext dbContext = new dbContext();

        [Route("CreateUser")]
        [HttpPost]
        public IHttpActionResult CreateUser(User vuser)
        {
            dynamic resultado = null;
            resultado = controlErrors();
            if (resultado.summary.Count > 0)
            {
                return Json(resultado);
            }
            UsuarioEmailViewModel usuario = dbContext.getUsuarioByUserMail(vuser.user, vuser.email);

            if (usuario.email == 0 & usuario.usuario == 0)
            {
                vuser.creationDate = DateTime.Now;
                dbContext.crearUsuario(vuser);
                dbContext.cargarCategorias(vuser.id_user);
                dbContext.enviarMailConfirmacion(vuser.id_user, vuser.email);
                resultado = new { key = "ok", description = "Usuario creado correctamente. <br/><br/>Solo necesita confirmar su correo para completar su registración" };
                return Json(resultado);
             }
            else
            {
                if (usuario.email > 0 & usuario.usuario > 0)
                    resultado = new { key = "error", description = "ya existe el usuario y el email ingresado" };
                else if (usuario.usuario > 0)
                    resultado = new { key = "error", description = "ya existe el usuario ingresado" };
                else
                    resultado = new { key = "error", description = "ya existe el email ingresado" };
                return Json(resultado);
            }
        }

        [Route("login")]
        public IHttpActionResult login(LoginViewModel usuario)
        {
            dbContext db = new dbContext();
            LoginViewModel login = new LoginViewModel();
            User aux = new User();
            login.pass = dbContext.Encriptar(usuario.pass);
            login.user = usuario.user;

            aux = db.getUsuarioByUserMail(login);
            if (aux != null)
            {
                if (aux.password.Replace(" ", "") == login.pass)
                {
                    if (!aux.IsConfirm)
                        ModelState.AddModelError("error", "Su cuenta necesita ser activada, por favor chequeá tu email");
                    else
                    {
                        return Json(aux);
                    }

                }
                else
                {
                    ModelState.AddModelError("error", "Contraseña incorrecta.");
                }

            }
            else
                ModelState.AddModelError("error", "El usuario no existe.");



            return Json(ModelState);
        }

        public List<string> parsearError(ModelStateDictionary modelState)
        {
            List<string> errorList = (from item in ModelState
                                      where item.Value.Errors.Any()
                                      select item.Value.Errors[0].ErrorMessage).ToList();
            return errorList;
        }

        // PUT: api/Default/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Default/5
        public void Delete(int id)
        {
        }

    }
}
