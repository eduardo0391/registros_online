using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace registros_online.Controllers.Api
{
    public class DefaultController : ApiController
    {
        public dynamic controlErrors()
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
                description = string.Join("<br/>", alErrores.ToArray()),
                //  F_PCME_HH= model.F_PCME_HH,
                summary = errores
            };
            return aux;
        }



    }
}
