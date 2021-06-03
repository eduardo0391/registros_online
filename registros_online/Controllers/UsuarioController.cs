using System;
using System.Web;
using System.Web.Mvc;
//using Microsoft.Owin.Security;
using registros_online.Models;
using System.Web.Security;
using registros_online.Infrastructure;
using System.Collections.Generic;
using System.Globalization;

namespace registros_online.Controllers
{
    public class UsuarioController : GenericController
    {
        dbContext dbContext;

        public UsuarioController()
        {
            dbContext = new dbContext();
        }
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.titulo = "Iniciar Sesión";
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel vuser)
        {
            User aux;
            if (vuser != null)
            {
                aux = dbContext.getUsuarioByUserMail(vuser);
                if (aux != null)
                {
                    if (dbContext.DesEncriptar(aux.password.Replace(" ", "")) == vuser.pass)
                    {
                        if (!aux.IsConfirm)
                            ModelState.AddModelError("error", "Su cuenta necesita ser activada, por favor revise su email");
                        else
                        {
                            setCooking(aux);
                            return RedirectToAction("Index", "home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("error", "Contraseña incorrecta.");
                    }

                }
                else
                {
                    ModelState.AddModelError("error", "El usuario ingresado no existe.");
                }
            }
            return View(vuser);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "usuario");
        }


        public ActionResult Registrar()
        {
            ViewBag.titulo = "Nuevo usuario? Registrate";
            return View("/Views/usuario/nuevoUsuario.cshtml");
        }

        public ActionResult verificarCuenta(string codigo)
        {
            User usuario = dbContext.getUsuarioxCodigo(codigo);
            if (usuario != null)
            {
                if ((DateTime.Now - usuario.shipmentDate.Value).Days >= 2)
                {
                    ViewBag.body = "El ticket se encuentra vencido";
                    return View("~/Views/usuario/messageUsuario.cshtml");
                }
                else
                {
                    ResetPasswordViewModel reset = new ResetPasswordViewModel();
                    ViewBag.Titulo = "Ingrese su nueva clave";
                    reset.ResetCode = codigo;
                    return View("actualizarClave", reset);

                }

            }
            else
            {
                ViewBag.body = "No existe el número de ticket";
                return View("~/Views/usuario/messageUsuario.cshtml");
            }
        }

        public ActionResult nuevoUsuario(UserViewModel vuser)
        {
            if (!ModelState.IsValid || vuser == null)
            {

                return View("/Views/usuario/nuevoUsuario.cshtml", vuser);
            }
            else

            {
                UsuarioEmailViewModel usuario = dbContext.getUsuarioByUserMail(vuser.user, vuser.email);

                if (usuario.email == 0 & usuario.usuario == 0)
                {

                    var user = new User()
                    {
                        user = vuser.user,
                        creationDate = DateTime.Now,
                        email = vuser.email,
                        ExpirationDate = DateTime.Parse(vuser.ExpirationDate),
                        id_user = vuser.id_user,
                        name = vuser.name,
                        password = vuser.password
                    };
                    dbContext.crearUsuario(user);
                    dbContext.cargarCategorias(user.id_user);
                    dbContext.enviarMailConfirmacion(user.id_user, user.email);
                    @ViewBag.titulo = "Usuario creado correctamente";
                    @ViewBag.body = "Usuario creado correctamente. <br/><br/>Solo necesita confirmar su correo para completar su registración.";
                    return View("messageUsuario");
                }
                else
                {
                    if (usuario.email > 0 & usuario.usuario > 0)
                        ModelState.AddModelError("error", "ya existe el usuario y el email ingresado");
                    else if (usuario.usuario > 0)
                        ModelState.AddModelError("error", "ya existe el usuario ingresado");
                    else
                        ModelState.AddModelError("error", "ya existe el email ingresado");

                    return View(vuser);
                }
            }

        }

        [HttpGet]
        public JsonResult enviarMailConfirmacion(int id, string emailDestino)
        {
            try
            {
                dbContext.enviarMailConfirmacion(id, emailDestino);
                return Json("enviado correctamente", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult recuperarUsuario()
        {
            FormatEmailViewModel email = new FormatEmailViewModel();
            @ViewBag.titulo = "Recuperación de clave";
            return View(email);
        }

        public ActionResult reenvioMailActivacion()
        {
            FormatEmailViewModel email = new FormatEmailViewModel();
            @ViewBag.titulo = "Envio de mail de confirmación";
            return View(email);
        }

        public JsonResult desencriptar(int valor, int id_user)
        {
            try
            {
                string aux;
                var user = dbContext.getUsuarioById(id_user);
                if (valor == 1)
                    aux = dbContext.DesEncriptar(user.password.Replace(" ", ""));
                else
                    aux = dbContext.Encriptar(user.password.Replace(" ", ""));
                return Json(aux, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                throw new System.ArgumentException(ex.Message, "original");
            }
        }

        public JsonResult encriptarTodo()
        {
            var todo = dbContext.getUsuarios();
            foreach (var item in todo)
            {
                item.password = dbContext.Encriptar(item.password.Replace(" ", ""));
                dbContext.UpdateUsuario(item);
            }
            return Json("todo bien", JsonRequestBehavior.AllowGet);
        }

        public ActionResult mensajeCorreo()
        {
            return View();
        }
        public ActionResult envioCorrecto()
        {
            @ViewBag.titulo = "Envío correctamente";
            @ViewBag.body = "Te enviamos un e-mail para recuperar tu clave. <br /> Por favor, chequeá tu casilla de correo.";
            return View("messageUsuario");
        }

        public ActionResult codigoInexistente()
        {
            @ViewBag.titulo = "Código de recuperación";
            @ViewBag.body = "No existe el código de recuperación.";
            return View("messageUsuario");
        }

        public ActionResult Actualizado()
        {
            @ViewBag.titulo = "Actualización de clave";
            @ViewBag.body = "Se ha actualizado la clave correctamente.";
            return View("messageUsuario");
        }

        public ActionResult ticketInvalido()
        {
            @ViewBag.titulo = "Ticket inválido";
            @ViewBag.body = "La solicitud está vencida, vuelva a solicitar el cambio de clave.";
            return View("messageUsuario");
        }

        [HttpPost]
        public ActionResult recuperarUsuario(FormatEmailViewModel vemail)
        {
            User usuario = dbContext.getUsuarioByEmail(vemail.email);
            @ViewBag.titulo = "Recuperación de clave";
            if (usuario == null)
            {
                ModelState.AddModelError("error", "El email ingresado no existe");
                return View(vemail);
            }
            else
            {
                try
                {

                    string resetCode = Guid.NewGuid().ToString();
                    usuario.codActivation = resetCode;
                    usuario.shipmentDate = DateTime.Now;
                    dbContext.UpdateUsuario(usuario);
                    dbContext.SendAsync(usuario.email, resetCode);

                    return RedirectToAction("envioCorrecto");
                }
                catch (Exception ex)
                {
                    ExceptionLogging.SendErrorToText(ex);
                    throw new System.ArgumentException(ex.Message, "original");

                }


            }
        }


        [HttpPost]
        public ActionResult enviarMailActivacion(FormatEmailViewModel vemail)
        {
            try
            {
                User usuario = dbContext.getUsuarioByEmail(vemail.email);
                @ViewBag.titulo = "Recuperación de clave";
                if (usuario == null)
                {
                    ModelState.AddModelError("error", "El email ingresado no existe");
                    return View(vemail);
                }
                else
                {

                    string resetCode = Guid.NewGuid().ToString();
                    usuario.codActivation = resetCode;
                    usuario.shipmentDate = DateTime.Now;
                    dbContext.UpdateUsuario(usuario);
                    dbContext.enviarMailConfirmacion(usuario.id_user, usuario.email);
                    @ViewBag.titulo = "Mail de confirmación enviado";
                    @ViewBag.body = "Mail de confirmación enviado correctamente. <br/><br/>Por favor verificar su correo para completar su registración.";
                    return View("messageUsuario");

                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                throw new System.ArgumentException(ex.Message, "original");
            }


        }


        [HttpPost]
        public ActionResult actualizar(ResetPasswordViewModel nuevoPassword)
        {
            User usuario = dbContext.getUsuarioxCodigo(nuevoPassword.ResetCode);
            usuario.password = dbContext.Encriptar(nuevoPassword.NewPassword);
            dbContext.UpdateUsuario(usuario);
            return RedirectToAction("Actualizado");
        }

        public ActionResult confirmarEmail(int id)
        {

            User usuario = dbContext.getUsuarioById(id);
            if (usuario != null)
            {
                usuario.IsConfirm = true;
                dbContext.UpdateUsuario(usuario);
                @ViewBag.titulo = "¡Le damos la bienvenida a Mis registros online!";
                @ViewBag.body = "Email confirmado exitosamente. Ya puedes iniciar sesión.";

            }
            else
            {
                @ViewBag.titulo = "ERROR!";
                @ViewBag.body = "Algo salió mal.";

            }
            return View("messageUsuario");
        }

        [MyAuthorize]
        [HttpPost]
        public JsonResult actualizarUsuario(UpdateUserViewModel userUpdate)
        {
            dynamic resultado = null;
            int id_usuario = idUsuario();
            UsuarioEmailViewModel comprobarUsuario = dbContext.getUsuarioActualizar(userUpdate.user, userUpdate.email, id_usuario);
            if (comprobarUsuario.email == 0 & comprobarUsuario.usuario == 0)
            {
                User usuario = dbContext.getUsuarioById(id_usuario);
                usuario.user = userUpdate.user;
                usuario.name = userUpdate.nombre;
                dbContext.UpdateUsuario(usuario);
                setCooking(usuario);
                if (usuario.email.TrimEnd() == userUpdate.email.TrimEnd())
                    resultado = new { key = "ok", descripcion = "emailIgual" };
                else
                    resultado = new { key = "ok", descripcion = "emailDistinto" };
            }
            else
            {
                string descripcion = "";
                if (comprobarUsuario.email > 0 & comprobarUsuario.usuario > 0)
                    descripcion = "ya existe el usuario y el email ingresado";
                else if (comprobarUsuario.usuario > 0)
                    descripcion = "ya existe el usuario ingresado";
                else
                    descripcion = "ya existe el email ingresado";
                resultado = new { key = "error", descripcion = descripcion };

            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }


        [MyAuthorizeAttribute]
        public JsonResult actualizarEmail(PasswordViewModel contra)
        {
            dynamic resultado = null;
            int id_usuario = idUsuario();

            User usuario = dbContext.getUsuarioById(idUsuario());
            if ( usuario.password.TrimEnd() != dbContext.Encriptar(contra.pass))
                resultado = new { key = "error", descripcion = "No se pudo actualizar el mail. Contraseña incorrecta" };
            else
            {
                usuario.email = contra.nuevoEmail;
                dbContext.UpdateUsuario(usuario);
                resultado = new { key = "ok", descripcion = "todo ok" };
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [MyAuthorize]
        public ActionResult actualizarUsuario()
        {
            ViewBag.titulo = "Actualizar usuario";
            ViewBag.usuario = HttpContext.User.Identity.Name;
            User usuario = dbContext.getUsuarioById(idUsuario());

            if (usuario != null)
            {
                UpdateUserViewModel usuarioUpdate = new UpdateUserViewModel();
                usuarioUpdate.email = usuario.email.TrimEnd();
                usuarioUpdate.nombre = usuario.name.TrimEnd();
                usuarioUpdate.user = usuario.user.TrimEnd();
                return View("/Views/usuario/usuarioUpdate/IndexUsuario.cshtml", usuarioUpdate);
            }
            else
                return RedirectToAction("Index", "home");
        }

        public JsonResult CheckPermission(int usuario)
        {
            try
            {
                var aux = dbContext.GetPermission(usuario);
                return Json(aux, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                throw new System.ArgumentException(ex.Message, "original");
            }
        }

        public ActionResult SuccessfulPayment()
        {
            User usuario = dbContext.getUsuarioById(idUsuario());
            if (usuario != null)
            {
                @ViewBag.titulo = "¡Gracias por utilizar Mis registros online!";
                @ViewBag.body = "Su cuenta se activará a la brevedad";
                var msj = "El usuario email: " + usuario.email + " y id= " + usuario.id_user + "  ha pagado la licencia.";
                dbContext.enviarMail("eduardo0391@gmail.com", "pago de " + usuario.email, msj);
            }
            else
            {
                @ViewBag.titulo = "Error";
                @ViewBag.body = "Algo salió mal.";

            }
            return View("messageUsuario");

        }
        public JsonResult SetLicense(int id_user)
        {

            try
            {
                User usuario = dbContext.getUsuarioById(id_user);
                usuario.ExpirationDate = DateTime.Now.AddYears(1);
                dbContext.UpdateUsuario(usuario);
                return Json(new { key = "ok", descripcion ="La nueva fecha de expiración del usuario " + usuario.email + " es el " + usuario.ExpirationDate}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                throw new System.ArgumentException(ex.Message, "original");
            }
        }
    }
}