
using System.ComponentModel.DataAnnotations;

namespace registros_online.Models
{
    public class PasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Debe ingresar la contraseña")]
        public string pass { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Debe ingresar el email")]
        public string nuevoEmail { get; set; }

        
    }
}