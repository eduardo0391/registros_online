using registros_online.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace registros_online.Controllers.Api
{
    public class CategoriaController : ApiController
    {
        dbContext dbContext = new dbContext();

        public IHttpActionResult Get(int tipo, int id_usuario)
        {
            IList<Category> categorias = new List<Category>();
            if (id_usuario != 0)
            {
                categorias = dbContext.getCategorias(tipo, id_usuario);
             
            }
            return Json(categorias);
        }
        
        [Route("getEstadisticasMovimientos")]
        public IHttpActionResult getEstadisticasMovimientos(string fechas, string tipo, int id_usuario)
        {
            var dbResult = dbContext.getEstadisticaProducto(fechas, id_usuario, tipo);
            return Json(dbResult);
        }

        // DELETE: api/Default/5

        public void Delete(int id)
        {
            if (id != 0 )
            {
                var categoria = dbContext.getCategoria(id);
                if (categoria != null )
                dbContext.eliminarCategoria(categoria);
            }

        }

        public IHttpActionResult Post(CategoryViewModel categoria)
        {
            Category aux = null;
            dynamic resultado = null;
            aux = dbContext.chequearCategoria(new Category { id = categoria.id, descripcion = categoria.descripcion, id_tipo = categoria.id_tipo, id_user = categoria.id_user });
            if (aux == null)
            {
                if (aux.id == 0)
                {
                    dbContext.agregarCategoria(aux);
                    resultado = new { key = "ok", descripcion = "Agregado correctamente" };
                }
                else
                {
                    dbContext.actualizarCategoria(aux);
                    resultado = new { key = "ok", descripcion = "Actualizado correctamente" };

                }
                return Json(resultado);
            }
            else
                ModelState.AddModelError("error", "La categoría ya existe");
            return Json(ModelState);
        }
    }
}
