using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace InnovationInteractiveMapApp.Classes
{
    public class CSVParser
    {
        //добавить список стран из GeoJSON-файла карты
        static public void AddCountries(string mapPath, string databasePath)
        {
            //список добавляемых названий стран
            List<string> countryNames = new List<string>();

            //считать названия всех стран, которые будут на карте
            try
            {
                using (StreamReader sr = new StreamReader(mapPath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //строка с названием страны содержит "ADMIN"
                        if (line.Contains("ADMIN"))
                        {
                            //название добавляемой страны
                            string countryName = "";

                            //нужно найти, где начинается название страны
                            int nameLocation = line.IndexOf("ADMIN\": \"");

                            if (nameLocation > 0)
                            {
                                //сдвиг на слово 'ADMIN": '
                                nameLocation = nameLocation + 9;

                                //получение всех символов названия страны
                                char currentSymbol = line[nameLocation];
                                while (currentSymbol != '\"')
                                {
                                    countryName = countryName + currentSymbol;
                                    nameLocation++;
                                    currentSymbol = line[nameLocation];
                                }
                            }

                            //добавление очередной страны
                            countryNames.Add(countryName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }

            //добавить названия стран в CSV-базу
            try
            {
                //открыть на запись
                using (StreamWriter sw = new StreamWriter(databasePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine("country,");
                    for (int i = 0; i < countryNames.Count(); i++)
                        sw.WriteLine(countryNames[i] + ",");
                }
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }
        }

        //парсинг файла с количеством патентов (получен из WIPO)
        static public void ParsePatentApplicationsWIPO(string filePath, string databasePath)
        {
            //словарь страна-значение
            Dictionary<string, int> data = new Dictionary<string, int>();

            //открыть CSV-файл с данными для парсинга
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //удалить кавычки из строки
                        line = line.Replace("\"", "");
                        //разбить очередную строку по запятым
                        string[] dataParts = line.Split(',');
                        //если количество полученных частей - 6, то найдена строка с данными
                        if (dataParts.Length == 6)
                        {
                            //первая часть - название страны
                            //последняя - значение
                            data.Add(dataParts[0], Convert.ToInt32(dataParts[4]));
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                string exception = ex.ToString();
            }

            string a = "";
            List<CountryData> countryData = GetCountryData(databasePath);
            WriteCountryData(countryData, databasePath);
        }

        //перенести новую информацию в данные о странах
        static private List<CountryData> AddCountryInfo(Dictionary<string, int> newInfo, List<CountryData> countryData)
        {
            //перебор стран из списка
            for (int i = 0; i < countryData.Count(); i++)
            {
                //если текущая страна упоминается в словаре, то информация переносится
                //в работе
            }

            return countryData;
        }

        //получить информацию о странах из database.csv
        static private List<CountryData> GetCountryData(string databasePath)
        {
            List<CountryData> data = new List<CountryData>();

            //прочитать всю информацию из файла данных
            try
            {
                using (StreamReader sr = new StreamReader(databasePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] dataParts = line.Split(',');
                        //не заголовок
                        if (dataParts[0] != "country")
                        {
                            CountryData countryData = new CountryData();
                            countryData.Name = dataParts[0];
                            //проверка, были ли уже внесены данные
                            if (dataParts[1] != "")
                                countryData.Total_Patent_Applications = Convert.ToInt32(dataParts[1]);
                            else 
                                countryData.Total_Patent_Applications = -1;

                            //добавление страны с её данными в список
                            data.Add(countryData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }

            return data;
        }

        //записать информацию о странах в database.csv
        static private void WriteCountryData(List<CountryData> data, string databasePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(databasePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine("country,total patent applications,");
                    for (int i = 0; i < data.Count(); i++)
                    {
                        //строка данных об одной стране
                        string dataStr = data[i].Name + ",";

                        if (data[1].Total_Patent_Applications != -1)
                            dataStr = dataStr + data[i].Total_Patent_Applications;
                        dataStr = dataStr + ",";

                        sw.WriteLine(dataStr);
                    }
                }
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }
        }

    }
}