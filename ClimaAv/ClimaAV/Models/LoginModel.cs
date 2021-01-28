using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClimaAV.Models
{
    public class LoginModel
    {
            [Required(ErrorMessage = "Debes ingresar el Usuario")]
            [Display(Name = "Usuario")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Debes ingresar la Contraseña")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [Display(Name = "Recordarme")]
            public bool RememberMe { get; set; }
    }
    public class LoginAreaModel
    {
        [Display(Name = "Ente")]
        [Required]
        public Guid IdEnte { get; set; }

        [Display(Name = "Modalidad")]
        [Required]
        public Guid ModalidadID { get; set; }

        [Display(Name = "Area")]
        [Required]
        public Guid AreaID { get; set; }

        [Display(Name = "Puesto")]
        [Required]
        public Guid PuestoID { get; set; }

        [Display(Name = "Especialidad")]
        [Required]
        public Guid EspecialidadID { get; set; }
    }

    public class UserUpdatePassword
    {
        public long UserID { get; set; }

        [Required]
        [Display(Name = "Contraseña Actual")]
        public string PasswordOld { get; set; }

        [Required]
        [Display(Name = "Contraseña Nueva")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirmar Contraseña Nueva")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Password", ErrorMessage = "Las contraseñas ingresadas no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

}