using System;
using System.ComponentModel.DataAnnotations;

namespace ClimaAV.Models
{
    public class EspecialidadModel
    {
        public Guid ORID { get; set; }

        [Display(Name = "Código")]

        [Required(ErrorMessage = "{0} es obligatorio")]
        public string CODE { get; set; }

        [Display(Name = "Descripción")]

        [Required(ErrorMessage = "{0} es obligatorio")]
        public string Descripcion { get; set; }

        [Display(Name = "Precio de venta")]
        public string MontoM { get; set; }
        public decimal Monto { get; set; }


    }

    public class EspecialidadListaModel
    {
        public Guid ORID { get; set; }
        public string CODE { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
    }

    public class EspecialidadVM
    {
        public Guid ORID { get; set; }

        public string CODE { get; set; }

        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        public bool Selected { get; set; }
    }
	
	public class EspecialidadComboModel
    {
        public Guid ORID { get; set; }
        public string Descripcion { get; set; }
    }
	
}