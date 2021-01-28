using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClimaAV.Models
{
    public class ResultViewModel
    {
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public string Clima { get; set; }
        public string Humedad { get; set; }
        public string ProbPreci { get; set; }
        public string Temp { get; set; }
        public string Icono { get; set; }
        public string Dia { get; set; }
        public string Viento { get; set; }
        public ClimaPorHora ClimaPorHora { get; set; }
    }

    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
        public double feels_like { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public int pressure { get; set; }
        public double humidity { get; set; }
    }

    public class Wind
    {
        public double speed { get; set; }
        public int deg { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class RootObject
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }

    public class List
    {
        public string Dia { get; set; }
        public string dt { get; set; }
        public Main main { get; set; }
        public List<Weather> weather { get; set; }
    }
    public class ClimaPorHora
    {
        public List<List> list { get; set; }
        public City city { get; set; }



    }
    public class City
    {
        public string id { get; set; }
        public string name { get; set; }
    }

}