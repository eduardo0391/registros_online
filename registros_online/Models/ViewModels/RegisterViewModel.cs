using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace registros_online.Models
{
    public class RegisterViewModel
    {
        public virtual int id { get; set; }

       
        [Required(ErrorMessage = "Ingrese la fecha")]
        public virtual string fecha { get; set; }

        [Required(ErrorMessage = "Debe ingresar la categoría")]
        public virtual string movimiento { get; set; }

        public IList<Category> categorias { get; set; }

        public virtual string descripcion { get; set; }

        
        [Required(ErrorMessage = "Ingrese el precio")]
        public virtual string precio
        {
            get;
            set;
        }

        public virtual int tipo {
            get; set;
        }

        public virtual int id_usuario { get; set; }
        public RegisterViewModel()
        {
            this.id = 0;

        }
        public RegisterViewModel(int tipo)
        {
            this.id = 0;
            this.tipo = tipo;
            categorias = new List<Category>();
        }
    }
}
