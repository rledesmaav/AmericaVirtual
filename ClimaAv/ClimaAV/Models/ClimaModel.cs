using System;
using System.ComponentModel.DataAnnotations;
using ClimaAV.Database;
using System.Web.Mvc;
using System.Collections.Generic;

namespace ClimaAV.Models
{

    public class HomeModel
    {
        public Buscador Buscador { get; set; }
        public ResultViewModel ResultViewModel { get; set; }
    }
    public class Buscador
    {   
        public Guid IdProvincia { get; set; }
        public string NombreLocalidad { get; set; }
        public string NombreProvincia { get; set; }
        public Localidad Localidad { get; set; }
        public List<Provincia> Provincia { get; set; }
        public List<Localidad> Localidades { get; set; }


    }
}