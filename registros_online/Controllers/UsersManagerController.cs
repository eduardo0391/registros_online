using registros_online.Infrastructure;
using registros_online.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace registros_online.Controllers
{
    public class UsersManagerController : GenericController
    {
        dbContext dbContext;

        public UsersManagerController()
        {
            dbContext = new dbContext();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var x = dbContext.getUsuarioById(idUsuario());
            if (x.IsSuperUser)
                return View();
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public JsonResult GetUsers()
        {
            try
            {
                var dbResult = dbContext.getUsuarios();
                List<UserViewModel> users = new List<UserViewModel>();
                foreach (var x in dbResult)
                    users.Add(new UserViewModel()
                    {
                        user = x.user,
                        codActivation = x.codActivation,
                        confirmPassword = x.confirmPassword,
                        creationDate = x.creationDate?.ToString("dd/MM/yyyy"),
                        IsConfirm = x.IsConfirm,
                        email = x.email,
                        ExpirationDate = x.ExpirationDate?.ToString("dd/MM/yyyy"),
                        id_user = x.id_user,
                        name = x.name,
                        password = dbContext.DesEncriptar(x.password),
                        remember = x.remember,
                        shipmentDate = x.shipmentDate?.ToString("dd/MM/yyyy"),
                        IsSuperUser = x.IsSuperUser
                    });
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        [MyAuthorize]
        [HttpPost]
        public JsonResult UpdateUser(UserViewModel userUpdate)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                User user = dbContext.getUsuarioById(userUpdate.id_user.Value);
                if (userUpdate.user != null)
                    user.user = userUpdate.user;
                if (userUpdate.name != null)
                    user.name = userUpdate.name;
                if (userUpdate.shipmentDate != null)
                    user.shipmentDate = DateTime.ParseExact(userUpdate.shipmentDate, "dd/MM/yyyy", provider);
                if (userUpdate.codActivation != null)
                    user.codActivation = userUpdate.codActivation;
                if (userUpdate.creationDate != null)
                    user.creationDate = DateTime.ParseExact(userUpdate.creationDate, "dd/MM/yyyy", provider);
                if (userUpdate.ExpirationDate != null)
                    user.ExpirationDate = DateTime.ParseExact(userUpdate.ExpirationDate, "dd/MM/yyyy", provider);
                user.IsConfirm = userUpdate.IsConfirm;
                user.IsSuperUser = userUpdate.IsSuperUser;
                dbContext.UpdateUsuario(user);

                return Json(new { key = "ok", descripcion = "Usuario actualizado correctamente" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                throw new System.ArgumentException(ex.Message, "original");
            }
        }
        //// GET: UserManager/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: UserManager/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: UserManager/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: UserManager/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: UserManager/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: UserManager/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: UserManager/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
