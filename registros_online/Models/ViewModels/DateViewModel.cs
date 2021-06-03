using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace registros_online.Models
{
    public class DateViewModel
    {
        public string anio { get; set; }
        public string mes { get; set; }

        public DateViewModel()
        {
            this.anio = DateTime.Now.Year.ToString();
            this.mes = DateTime.Now.Month.ToString();
        }

        public DateViewModel(string vAnio, string vMes)
        {
            this.anio = vAnio;
            this.mes = vMes;
        }

    }
}
