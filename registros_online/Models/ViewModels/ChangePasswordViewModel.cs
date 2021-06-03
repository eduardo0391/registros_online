using System.ComponentModel.DataAnnotations;

namespace registros_online.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Debe ingresar su contraseña actual", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe ingresar la nueva contraseña", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        [Required(ErrorMessage = "Debe ingresar la confirmación de la contraseña", AllowEmptyStrings = false)]
        public string ConfirmPassword { get; set; }
    }
}