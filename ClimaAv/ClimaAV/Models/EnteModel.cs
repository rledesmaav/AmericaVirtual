using System;
using System.ComponentModel.DataAnnotations;
using ClimaAV.Database;
using System.Web.Mvc;
using System.Collections.Generic;

namespace ClimaAV.Models
{
    public class EnteModel
    {
        public System.Guid ORID { get; set; }

        [Required(ErrorMessage = "Debes ingresar el Ente")]
        [StringLength(100, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres", MinimumLength = 5)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debes ingresar el CODE")]
        [StringLength(4, ErrorMessage = "El campo {0} debe tener {1} caracteres", MinimumLength = 4)]
        [Remote("ExisteCODE", "Ente", HttpMethod = "POST", ErrorMessage = "El código ya existe")]
        public string CODE { get; set; }

        public string Calle { get; set; }

        public Nullable<int> Altura { get; set; }

        [Display(Name = "Localidad")]
        public Localidad Localidad { get; set; }

        public System.Guid IdLocalidad { get; set; }

        public List<EspecialidadVM> Especialidades { get; set; }

    }
}