using System.Web.Mvc;
using InnovationInteractiveMapApp.Classes;

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
    }
}