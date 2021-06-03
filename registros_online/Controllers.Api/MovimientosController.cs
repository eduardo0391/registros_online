using registros_online.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace registros_online.Controllers.Api
{
    [RoutePrefix("api/Movimientos")]
    public class MovimientosController : DefaultController
    {
        dbContext dbContext = new dbContext();

        [Route("getMeses")]
        public IHttpActionResult getMeses(int id, string anio, string tipo)
        {
            if (String.IsNullOrEmpty(anio))
            {
                anio = dbContext.getUltimoAnio(tipo, id);
            }
            var dbResult = dbContext.getMes(anio, id, tipo);
       
            return Json(dbResult);
        }

        [Route("getAnios")]
        public IHttpActionResult getAnios(string tipo, int id)
        {
            var dbResult = dbContext.getAños(id, tipo);
            return Json(dbResult);
        }


         [Route("getGastos")]

         public IHttpActionResult getGastos(int id_usuario, string tipo, string anio, string mes)
         {
             IList x = dbContext.GetRegisters(anio, mes, "",  id_usuario, tipo, "fecha","todo",0).registros.OrderByDescending(y=>y.fecha).ToList();

             if (x.Count == 0)
             {
                 return NotFound();
             }
             return Json(x);
         }

        [AllowAnonymous]
        [Route("guardarRegistro")]
        public IHttpActionResult guardarRegistro(RegisterViewModel vregistro)
        {
            if (!ModelState.IsValid)
            {
                List<string> x = parsearError(ModelState);
                return Json(x);
            }
            try
            {
                if (vregistro.id == 0)
                    dbContext.CreateRegistro(cargarDatos(vregistro));
                else
                    dbContext.UpdatePrueba(cargarDatos(vregistro));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.Message);
                return Json(ModelState);
            }
            dynamic aux = new { key = "ok", descripcion = "Cargado Correctamente" };

            return Json(aux);
        }

         public Register cargarDatos(RegisterViewModel aux)
         {
             Register nuevo = new Register();
             //nuevo.anio = aux.anio;
             nuevo.tipo = aux.tipo;
             nuevo.descripcion = aux.descripcion;
             nuevo.movimiento = aux.movimiento;
             //Remove(10) para remover la hora, minutos y segundos
             if (aux.fecha.Length==10)
                 nuevo.fecha = DateTime.ParseExact(aux.fecha, "yyyy-MM-dd", null);
             else
                 nuevo.fecha = DateTime.ParseExact(aux.fecha.Remove(10), "yyyy-MM-dd", null);
             nuevo.id = aux.id;
             aux.precio.Replace(".", "b");

             nuevo.precio = double.Parse(aux.precio.Replace(".", ""), NumberStyles.AllowDecimalPoint, new CultureInfo("sv-SE"));

             nuevo.id_usuario = aux.id_usuario;
             nuevo.total = 0;
             return nuevo;
          }

        public void Delete(int id)
        {
            var Pru = dbContext.GetRegistroById(id);
            if (Pru != null)
                dbContext.DeletePrueba(Pru);

        }

        [Route("getEstadisticasMovimientos")]
        public IHttpActionResult getEstadisticasMovimientos(string fechas, string tipo, int id_usuario)
        {
        
            var dbResult = dbContext.getEstadisticaProducto(fechas, id_usuario, tipo);
            return Json(dbResult);
        }

        public List<string> parsearError(ModelStateDictionary modelState)
        {
            List<string> errorList = (from item in ModelState
                                      where item.Value.Errors.Any()
                                      select item.Value.Errors[0].ErrorMessage).ToList();
            return errorList;
        }

    }
}
