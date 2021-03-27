using System;
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

            try
            {
                //первичное наполнение csv-файла названиями стран
                CSVParser.AddCountries(mapPath, databasePath);

                string patentApplicationsWIPOPath = Server.MapPath("~/DataFiles/Total patent applications.csv");
                CSVParser.ParsePatentApplicationsWIPO(patentApplicationsWIPOPath, databasePath, mapPath);

                string trademarkApplicationsWIPOPath = Server.MapPath("~/DataFiles/Total trademark applications.csv");
                CSVParser.ParseTrademarkApplicationsWIPO(trademarkApplicationsWIPOPath, databasePath, mapPath);

                string hightechExportsWorldBankPath = Server.MapPath("~/DataFiles/High-technology exports.csv");
                CSVParser.ParseHighTechExportsWorldBank(hightechExportsWorldBankPath, databasePath, mapPath);

                string hightechExportsUSDWorldBankPath = Server.MapPath("~/DataFiles/High-technology exports (USD).csv");
                CSVParser.ParseHighTechExportsUSDWorldBank(hightechExportsUSDWorldBankPath, databasePath, mapPath);

                string resAndDevExpendWorldBankPath = Server.MapPath("~/DataFiles/Research and development.csv");
                CSVParser.ParseResAndDevExpendWorldBank(resAndDevExpendWorldBankPath, databasePath, mapPath);

                string intelPropertyPaymentWorldBankPath = Server.MapPath("~/DataFiles/Charges for the use of intellectual property.csv");
                CSVParser.ParseIntelPropPaymentWorldBank(intelPropertyPaymentWorldBankPath, databasePath, mapPath);
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }
            

            return View();
        }
    }
}