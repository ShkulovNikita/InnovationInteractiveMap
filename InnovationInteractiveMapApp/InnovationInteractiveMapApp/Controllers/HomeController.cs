﻿using System.Web.Mvc;
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

            string trademarkApplicationsWIPOPath = Server.MapPath("~/DataFiles/Total trademark applications.csv");
            CSVParser.ParseTrademarkApplicationsWIPO(trademarkApplicationsWIPOPath, databasePath, mapPath);

            string hightechExportsWorldBankPath = Server.MapPath("~/DataFiles/High-technology exports.csv");
            CSVParser.ParseHighTechExportsWorldBank(hightechExportsWorldBankPath, databasePath, mapPath);

            string hightechExportsUSDWorldBankPath = Server.MapPath("~/DataFiles/High-technology exports (USD).csv");
            CSVParser.ParseHighTechExportsUSDWorldBank(hightechExportsUSDWorldBankPath, databasePath, mapPath);


            return View();
        }
    }
}