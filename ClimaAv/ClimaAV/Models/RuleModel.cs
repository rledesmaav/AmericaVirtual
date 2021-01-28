using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClimaAV.Database;
namespace ClimaAV.Models
{
    public class RuleModel
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Definicion { get; set; }
        public bool Selected { get; set; }
        public string GroupName { get; set; }
    }
}
