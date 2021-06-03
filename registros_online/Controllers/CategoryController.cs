using registros_online.Infrastructure;
using registros_online.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace registros_online.Controllers
{
    public class CategoryController : GenericController
    {
        dbContext dbContext;

        public CategoryController()
        {
            dbContext = new dbContext();
        }


        [MyAuthorizeAttribute]
        public JsonResult getCategorias(int tipo)
        {
            int id_usuario = idUsuario();
            IList<Category> categorias = new List<Category>();
            if (id_usuario != 0)
            {
                categorias = dbContext.getCategorias(tipo, id_usuario);
            }
            return Json(categorias, JsonRequestBehavior.AllowGet);
        }

        [MyAuthorizeAttribute]
        public JsonResult agregarCategoria(Category categoria)
        {

            dynamic resultado = null;
            categoria.id_user = idUsuario();

            Category aux = null;
            aux = dbContext.chequearCategoria(categoria);
            if (aux == null)
            {
                if (categoria.id == 0)
                {
                    dbContext.agregarCategoria(categoria);
                    resultado = new { key = "ok", message = "Agregado correctamente" };
                }
                else
                {
                    dbContext.actualizarCategoria(categoria);
                    resultado = new { key = "ok", message = "Actualizado correctamente" };
                }
            }
            else
                resultado = new { key = "error", message = "Ya existe la categoría " + aux.descripcion };
            return Json(resultado, JsonRequestBehavior.AllowGet);

        }

        public JsonResult eliminarCategoria(int id_categoria)
        {

            Category Pru = dbContext.getCategoria(id_categoria);
            if (Pru != null)
            {
                dbContext.eliminarCategoria(Pru);
                return Json("Eliminado correctamente", JsonRequestBehavior.AllowGet);
            }
            else
                return Json("No se pudo eliminar la categoria", JsonRequestBehavior.AllowGet);
        }
    }
}
