using registros_online.Models;
using System.Web.Mvc;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using registros_online.Infrastructure;
using System.Collections;

namespace registros_online.Controllers
{

    public class HomeController : GenericController
    {
        dbContext dbContext;
        string id_usuario;

        public HomeController()
        {
            dbContext = new dbContext();
        }
        
        //    [HandleError(View = "Error")]
      
        [Authorize]
        public ActionResult Index()
        {
            int id_usuario = idUsuario();
            ViewBag.usuario = HttpContext.User.Identity.Name.TrimEnd();
            //ViewBag.usuario = HttpContext.User.Identity.Name;
            ViewBag.titulo = "Lista de egresos";
            ViewBag.tipo = 2;
            return View();

        }

       // [HandleError(View = "Error")]
        [Authorize]
        public ActionResult Ingresos()
        {
            int id_usuario = idUsuario();
            ViewBag.usuario = HttpContext.User.Identity.Name.TrimEnd();
            ViewBag.titulo = "Lista de ingresos";
            ViewBag.tipo = 1;
            return View("~/Views/home/Index.cshtml");

        }

        public ActionResult ingresosAñoMes()
        {
            ViewBag.usuario = HttpContext.User.Identity.Name.TrimEnd();
            ViewBag.tipo = 1;
            ViewBag.titulo = "Ingresos por mes";
            if (HttpContext.User.Identity.Name == "")
                return RedirectToAction("Index", "home");
            else
                return View("~/Views/totales/totalesAñoMes.cshtml");
        }

        public ActionResult egresosAñoMes()
        {
            ViewBag.usuario = HttpContext.User.Identity.Name;
            ViewBag.tipo = 2;
            ViewBag.titulo = "Egresos por mes";
            if (HttpContext.User.Identity.Name == "")
                return RedirectToAction("Index", "home");
            else
                return View("~/Views/totales/totalesAñoMes.cshtml");
        }

        public ActionResult crearRegistro()
        {
            return View("~/Views/home/agregarRegistro.cshtml", new Register());
        }

        public JsonResult guardarRegistro(RegisterViewModel vregistro)
        {
            int id = idUsuario();
           
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            vregistro.id_usuario = idUsuario();
            UserPermission per = dbContext.GetPermission(vregistro.id_usuario);
            if (per.Id_permission == 1)
            {
                if (vregistro.id == 0)
                    dbContext.CreateRegistro(cargarDatos(vregistro));
                else
                    dbContext.UpdatePrueba(cargarDatos(vregistro));
            }
            else
            {
                return Json(new { key = "error", IsRequiredPay = per.IsRequiredPay , description = per.Description }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { key = "ok", description = "todo ok" }, JsonRequestBehavior.AllowGet);
        }
        
        [MyAuthorize]
        public ActionResult actualizarPassword(ChangePasswordViewModel cambiarPassword)
        {
            //control de errores
            dynamic resultado;
            int id_usuario = idUsuario();
            //Controlo primero si la sesion no está expirada
 
                if (!ModelState.IsValid)
                {
                    ModelState.OrderByDescending(x => x.Key);

                    resultado = controlErrores();
                    return Json(resultado, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    User usuario = dbContext.getUsuarioById(idUsuario());
                    if ( dbContext.DesEncriptar(usuario.password.TrimEnd()) != cambiarPassword.Password)
                    {
                        List<dynamic> summary = new List<dynamic>();
                        summary.Add(new { key = "error", error = "La contraseña ingresa es incorrecta" });
                        resultado = new { key = "error", summary };
                    }
                    else
                    {
                        usuario.password = dbContext.Encriptar(cambiarPassword.NewPassword);
                        dbContext.UpdateUsuario(usuario);
                        resultado = new { key = "ok", descripcion = "todo ok" };
                    }
                }

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public Register cargarDatos(RegisterViewModel aux)
        {
            Register nuevo = new Register();
            //nuevo.anio = aux.anio;
            nuevo.tipo = aux.tipo;
            nuevo.descripcion = aux.descripcion;
            nuevo.fecha = DateTime.ParseExact(aux.fecha, "dd/MM/yyyy", null);
            nuevo.id = aux.id;
            aux.precio.Replace(".", "b");
            nuevo.movimiento = aux.movimiento;
            nuevo.precio = double.Parse(aux.precio.Replace(".", ""), NumberStyles.AllowDecimalPoint, new CultureInfo("sv-SE"));

            nuevo.id_usuario = aux.id_usuario;
            nuevo.total = 0;
            return nuevo;
        }
        public JsonResult eliminarRegistro(int id)
        {
            var Pru = dbContext.GetRegistroById(id);
            dbContext.DeletePrueba(Pru);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0)]
        public ActionResult Delete(int id)
        {

            var Pru = dbContext.GetRegistroById(id);
            dbContext.DeletePrueba(Pru);
            return PartialView("~/Views/home/listaRegistros.cshtml", new DateViewModel { anio = Pru.fecha.Year.ToString(), mes = Pru.fecha.Month.ToString() });


        }

        [MyAuthorizeAttribute]
        [HttpGet]
        public JsonResult getRegistrosxMes(string tipo)
        {

            try
            {
                var x = dbContext.GetRegistrosxMes(idUsuario(), tipo);
                return Json(x, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getRegistrosApi(string id_usuario)

        {
            IEnumerable<Register> registros = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:2020/api/");

                //HTTP GET

                var responseTask = client.GetAsync("registroOnline?id_usuario=edu");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Register>>();
                    readTask.Wait();

                    registros = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    registros = Enumerable.Empty<Register>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return Json(registros, JsonRequestBehavior.AllowGet);
        }

        [MyAuthorizeAttribute]
        [HttpGet]
        public JsonResult GetRegisters(string anio, string mes, string columna = "", string orden = "fecha", string cantidadRegistros = "10", int numeroPagina = 2, string tipo = "0")
        {

            try
            {
                dataModel query = dbContext.GetRegisters(anio, mes, columna, idUsuario(), tipo, orden, cantidadRegistros, numeroPagina);

                return Json(query, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getRegistro(string id)
        {

            var dbResult = dbContext.GetRegistroById(Int16.Parse(id.ToString()));
            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getEstadisticasProductos(string fechas, string tipo)
        {
            //Create the data source
            List<int> Scores = new List<int>() { 97, 92, 81, 60 };
            // id_usuario = HttpContext.User.Identity.Name;
            // Create the query.
            IEnumerable<int> queryHighScores =
                from score in Scores
                where score > 80
                select score;

            var dbResult = dbContext.getEstadisticaProducto(fechas, idUsuario(), tipo);

            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getAnios(string tipo)
        {
            id_usuario = HttpContext.User.Identity.Name;
            var dbResult = dbContext.getAños(idUsuario(), tipo);
            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        [MyAuthorizeAttribute]
        public JsonResult getMeses(string anio, string tipo)
        {
            if (String.IsNullOrEmpty(anio))
            {
                anio = dbContext.getUltimoAnio(tipo, idUsuario());
            }
            var dbResult = dbContext.getMes(anio, idUsuario(), tipo);
            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ExportData(string anio, string mes, string tipo, bool filtrado)
        {
            try
            {
                GridView gv = new GridView();
                int id = idUsuario();
                if (id ==0)
                    return RedirectToAction("Index", "home");
                if (filtrado) 
                gv.DataSource = dbContext.GetRegisters(anio, mes, null, id, tipo, "fecha", "todo", 0).registros
                    .Select(x => new { movimiento = x.movimiento, descripcion = x.descripcion, fecha =x.fecha.ToString("dd/MM/yyyy"), monto = x.precio });
                else
                    gv.DataSource = dbContext.GetTodo(id, tipo, "fecha").registros
                   .Select(x => new { movimiento = x.movimiento, descripcion = x.descripcion, fecha = x.fecha.ToString("dd/MM/yyyy"), monto = x.precio });
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=datos.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
               // return View();
                  return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckPermission(int usuario)
        {
            int id_usuario = idUsuario();

            var aux = dbContext.GetPermission(usuario);
            return Json(aux.Description, JsonRequestBehavior.AllowGet);
        }
    }
}
