using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InnovationInteractiveMapApp.Classes
{
    public class CountryData
    {
        //название страны
        public string Name { get; set; }

        //количество заявок на патенты
        public int Total_Patent_Applications { get; set; }

        //количество заявок на торговые марки
        public int Total_Trademark_Applications { get; set; }
    }
}