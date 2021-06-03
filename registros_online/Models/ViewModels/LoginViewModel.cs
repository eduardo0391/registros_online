using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace registros_online.Models
{
    public class LoginViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Debe ingresar el usuario")]
        public virtual string user { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Debe ingresar la contraseña")]
        public virtual string pass { get; set; }

        public virtual bool recordarme{ get; set; }

    }
}