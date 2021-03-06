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

        //экспорт высоких технологий (в % от произведенного экспорта)
        public double High_Tech_Exports { get; set; }

        //экспорт высоких технологий (в долларах США)
        public double High_Tech_Exports_USD { get; set; }

        //затраты на R&D (в % от ВВП)
        public double Res_And_Dev_Expenditure { get; set; }

        //плата за интеллектуальную собственность (в долларах США)
        //https://data.worldbank.org/indicator/BM.GSR.ROYL.CD
        public double IntelPropertyPayment { get; set; }
    }
}