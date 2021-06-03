using System.ComponentModel.DataAnnotations;

namespace registros_online.Models
{
    public class FormatEmailViewModel
    {
        [Required(ErrorMessage = "Debe ingresar el email")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public virtual string email { get; set; }
    }
}