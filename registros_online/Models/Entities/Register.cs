using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace registros_online.Models
{
    public class Register
    {
        public virtual int id { get; set; }
        [Required(ErrorMessage = "Ingrese la fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public virtual DateTime fecha { get; set; }
        public virtual string descripcion { get; set; }
        public virtual string movimiento { get; set; }

        public virtual int tipo { get; set; }


        [Required(ErrorMessage = "Ingrese el precio")]
        public virtual double precio {
            get {
                return _precio;
                    //Math.Round(_precio, 2);
            }
            set {  _precio = Math.Round(value, 2); }
        }

         private double _precio;

        public virtual double _total {get;set;}
        public virtual double total { get; set; }
        public virtual bool seleccionado { get; set; }
        public virtual int id_usuario { get; set; }

        public virtual string anio
        { get
            {
                return this.fecha.Year.ToString();
            }

            set { }
           
        }

        public virtual string formattedDate
        {
            get
            {
                return this.fecha.ToString("dd/MM/yyyy");
            }

            set { }

        }

        public Register()
        {
            this.id = 0;
        }
    }
}

