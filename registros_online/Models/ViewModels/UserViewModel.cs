using System;
using System.ComponentModel.DataAnnotations;

namespace registros_online.Models
{
    public class UserViewModel
    {
        [StringLength(20, ErrorMessage = "El nombre puede tener una longitud máxima de 20 caracteres")]
        [Required(ErrorMessage = "Debe ingresar el nombre")]
        public virtual string name { get; set; }

        public virtual int? id_user { get; set; }

        [System.ComponentModel.DataAnnotations.Key]
        [Required(ErrorMessage = "Debe ingresar el usuario")]
        public virtual string user { get; set; }

        [Required(ErrorMessage = "Debe ingresar el email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email Inválido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public virtual string email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Debe ingresar la contraseña")]
        public virtual string password { get; set; }

        public virtual string creationDate { get; set; }

        public virtual bool remember { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [System.ComponentModel.DataAnnotations.Compare("password", ErrorMessage = "La contraseña y la confirmación de la contraseña deben ser iguales.")]
        public virtual string confirmPassword { get; set; }

        public virtual string codActivation { get; set; }

        public virtual string shipmentDate { get; set; }

        public virtual string ExpirationDate { get; set; }
        //0 no. 1 si
        public virtual bool IsConfirm { get; set; }

        public virtual bool IsSuperUser { get; set; }
        //public UserViewModel()
        //{
        //    this.shipmentDate = DateTime.Now;
        //}

    }

}
