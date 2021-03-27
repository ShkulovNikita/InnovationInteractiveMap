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
    }
}