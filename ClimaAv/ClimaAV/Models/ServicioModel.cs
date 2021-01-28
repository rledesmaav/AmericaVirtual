using System;
using System.ComponentModel.DataAnnotations;

namespace ClimaAV.Models
{
    public class ServicioModel
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

    public class ServicioListaModel
    {
        public Guid ORID { get; set; }
        public string CODE { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
    }

    public class ServicioComboModel
    {
        public Guid ORID { get; set; }
        public string Descripcion { get; set; }
    }
}