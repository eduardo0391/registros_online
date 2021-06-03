using System;
using System.ComponentModel.DataAnnotations;

namespace registros_online.Models
    {
        public class UpdateUserViewModel
        {
            [StringLength(40, ErrorMessage = "El nombre puede tener una longitud máxima de 20 caracteres")]
            [Required(ErrorMessage = "Debe ingresar el nombre")]
            public virtual string nombre { get; set; }

            [System.ComponentModel.DataAnnotations.Key]
            [Required(ErrorMessage = "Debe ingresar el usuario")]
            public virtual string user { get; set; }

            [Required(ErrorMessage = "Debe ingresar el email")]
            [EmailAddress(ErrorMessage = "Email inválido")]
            public virtual string email { get; set; }

            //[DataType(DataType.Password)]
            //[Required(ErrorMessage = "Debe ingresar la contraseña")]
            //public virtual string pass { get; set; }

        }
    }
