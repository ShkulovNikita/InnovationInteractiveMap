using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using InnovationInteractiveMapApp.Classes;
using System.Web.Script.Serialization;

namespace InnovationInteractiveMapApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //пути до файлов
            string mapPath = Server.MapPath("~/Map/countries.geojson");
            string databasePath = Server.MapPath("~/Database/database.csv");

            //первичное наполнение csv-файла названиями стран
            CSVParser.AddCountries(mapPath, databasePath);

            string patentApplicationsWIPOPath = Server.MapPath("~/DataFiles/Total patent applications.csv");
            CSVParser.ParsePatentApplicationsWIPO(patentApplicationsWIPOPath, databasePath, mapPath);

            return View();
        }

        public JsonResult ReturnFile()
        {
            string text = "";
            return Json(text);
        }

        /*public string ReturnFile()
        {
            //прочитать файл
            string text;

            using(StreamReader sr = new StreamReader(Server.MapPath("~/Map/countries.geojson")))
            {
                text = sr.ReadToEnd();
            }

            return text;
        }*/



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}