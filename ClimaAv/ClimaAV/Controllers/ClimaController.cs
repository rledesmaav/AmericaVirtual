using ClimaAV.Database;
using ClimaAV.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ClimaAV.Controllers
{
    public class ClimaController : Controller
    {
        //KEY API openweathermap.org
        string appId = "212b49dc91ea1c61dad9b96abb1c004d";
        AmericaVirtualEntities  db = new AmericaVirtualEntities();
        public ActionResult Index()
        {
            HomeModel home = new HomeModel();
            home.ResultViewModel = new ResultViewModel();
            home.ResultViewModel.ClimaPorHora = new ClimaPorHora();
            home.Buscador = new Buscador();
            cargoCombos();
            return View(home);
        }
        [HttpPost]
        public ActionResult Index(Buscador model)
        {
            HomeModel home = new HomeModel();
            home.ResultViewModel = new ResultViewModel();
            home.Buscador = model;
            model.NombreProvincia = db.Provincia.Where(x => x.ORID == model.IdProvincia).Select(x => x.Descripcion).FirstOrDefault();
            cargoCombos();
            ViewBag.Localidad = db.Localidad.OrderBy(e => e.Descripcion).Where(x => x.Partido.IdProvincia == model.IdProvincia).ToList();
            if (model != null)
            {
                home.ResultViewModel = WeatherDetail(model);
            }
            return View(home);
        }

        private void cargoCombos()
        {
            ViewBag.Provincia = db.Provincia.OrderBy(e => e.Descripcion).ToList();
            ViewBag.Localidad = db.Localidad.OrderBy(e => e.Descripcion).Where(x => x.CODE == "1").ToList();
        }
        public ActionResult GetLocalidad(Guid IdProvincia)
        {
            var Localidad = db.Localidad.OrderBy(a => a.Descripcion)
            .Where(x => x.Partido.IdProvincia == IdProvincia).Select(a => "<option value='" + a.Descripcion + "'>" + a.Descripcion + "</option>'");
            return Content(String.Join("", Localidad));

        }

        //[HttpPost]
        //public ActionResult Index(string City)
        //{
        //    HomeModel home = new HomeModel();
        //    home.ResultViewModel = WeatherDetail(City);
        //    home.Buscador.Provincia = db.Provincia.ToList();
        //    return View(home);
        //}
        [HttpPost]
        public ResultViewModel WeatherDetail(Buscador model)
        {
            //url del servicio web clima actual
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q=" + model.NombreLocalidad + "," + model.NombreProvincia + ",AR&units=metric&cnt=1&APPID=" + appId + "&lang=es");
            //URL del servicio web clima por horas
            string url_forecast = string.Format("http://api.openweathermap.org/data/2.5/forecast?q=" + model.NombreLocalidad + "," + model.NombreProvincia + ",AR&units=metric&cnt=5&APPID=" + appId + "&lang=es");
            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);
                string jsonforecast = client.DownloadString(url_forecast);
                //Convierte el JSON a un modelo especial generado en base al JSON.  
                RootObject weatherInfo = (new JavaScriptSerializer()).Deserialize<RootObject>(json);
                ClimaPorHora climaPorHora = (new JavaScriptSerializer()).Deserialize<ClimaPorHora>(jsonforecast);

                //Pongo los datos del modelo del JSON, a un modelo propio para enviar a la vista
                ResultViewModel rslt = new ResultViewModel();
                TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

                rslt.Pais = weatherInfo.sys.country; ;
                rslt.Ciudad = textInfo.ToTitleCase(weatherInfo.name);
                rslt.Clima = textInfo.ToTitleCase(weatherInfo.weather[0].description);
                rslt.Humedad = Convert.ToString(weatherInfo.main.humidity);
                rslt.Temp = Convert.ToString(Convert.ToInt32(weatherInfo.main.temp));
                rslt.Icono = weatherInfo.weather[0].icon;
                rslt.Viento = Convert.ToString(weatherInfo.wind.speed);
                rslt.Dia = textInfo.ToTitleCase(DateTime.Now.ToString("dddd",
                        new CultureInfo("es-ES")));
                foreach (var item in climaPorHora.list)
                {
                    item.Dia = Dia(Convert.ToDouble(item.dt));
                }
                rslt.ClimaPorHora = climaPorHora;


                return rslt;
            }
        }
        public string Dia(double dt)
        {
            // Unix timestamp is seconds past epoch
            TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(dt).ToLocalTime();
            string dia = textInfo.ToTitleCase(dtDateTime.ToString("dd/MM/yyyy HH:mm tt",
                        new CultureInfo("es-ES")));
            return dia;

        }
        //{
        //     "cod": "200",
        //     "message": 0,
        //     "cnt": 40,
        //     "list": [
        //         {
        //             "dt": 1589176800,
        //             "main": {
        //                 "temp": 291.59,
        //                 "feels_like": 288.55,
        //                 "temp_min": 290.47,
        //                 "temp_max": 291.59,
        //                 "pressure": 1011,
        //                 "sea_level": 1012,
        //                 "grnd_level": 1009,
        //                 "humidity": 54,
        //                 "temp_kf": 1.12
        //             },
        //             "weather": [
        //                 {
        //                     "id": 803,
        //                     "main": "Clouds",
        //                     "description": "nubes rotas",
        //                     "icon": "04n"
        //                 }
        //             ],
        //             "clouds": {
        //                 "all": 74
        //             },
        //             "wind": {
        //                 "speed": 4.01,
        //                 "deg": 215
        //             },
        //             "sys": {
        //                 "pod": "n"
        //             },
        //             "dt_txt": "2020-05-11 06:00:00"
        //         }
        //     ],
        //     "city": {
        //         "id": 3431271,
        //         "name": "Lomas de Zamora",
        //         "coord": {
        //             "lat": -34.7609,
        //             "lon": -58.4063
        //         },
        //         "country": "AR",
        //         "timezone": -10800,
        //         "sunrise": 1589193476,
        //         "sunset": 1589230928
        //     }
        // }

    }
}