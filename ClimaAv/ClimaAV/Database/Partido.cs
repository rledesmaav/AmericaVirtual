//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClimaAV.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Partido
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Partido()
        {
            this.Localidad = new HashSet<Localidad>();
        }
    
        public System.Guid ORID { get; set; }
        public string CODE { get; set; }
        public string Descripcion { get; set; }
        public System.Guid IdProvincia { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Localidad> Localidad { get; set; }
        public virtual Provincia Provincia { get; set; }
    }
}
