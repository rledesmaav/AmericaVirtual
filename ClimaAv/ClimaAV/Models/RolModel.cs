using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClimaAV.Models
{
    public class RolModel
    {
        public long RolID { get; set; }
        [Display(Name = "Descripcion")]
        public string Description { get; set; }
        public List<RuleModel> Rules { get; set; }
    }
    public class RolIndex
    {
        public long RolID { get; set; }
        [Display(Name = "Descripcion")]
        public string Description { get; set; }
    }
    public class RolVM
    {
        public long RolID { get; set; }
        [Display(Name = "Descripcion")]
        public string Description { get; set; }
        public bool Selected { get; set; }
    }
}