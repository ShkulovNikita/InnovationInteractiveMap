using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace InnovationInteractiveMapApp.Classes
{
    public class CSVParser
    {
        public void ReadCountries()
        {
        }

        //добавить список стран из GeoJSON-файла карты
        static public void AddCountries(string path1, string path2)
        {
            //список добавляемых названий стран
            List<string> countryNames = new List<string>();

            //считать названия всех стран, которые будут на карте
            try
            {
                //расположение файла с картой
                string filePath = path1;
                using (StreamReader sr = new StreamReader(filePath))
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
                //расположение файла CSV-базы
                string filePath = path2;

                //открыть на запись
                using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.Default))
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

    }
}