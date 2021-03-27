using System;
using System.Collections.Generic;
using System.Linq;
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
                    sw.WriteLine("country,,,");
                    for (int i = 0; i < countryNames.Count(); i++)
                        sw.WriteLine(countryNames[i] + ",,,");
                }
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }
        }

        //парсинг файла с количеством патентов (получен из WIPO)
        static public void ParsePatentApplicationsWIPO(string filePath, string databasePath, string mapPath)
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

            List<CountryData> countryData = GetCountryData(databasePath);
            countryData = AddCountryPatentsInfo(data, countryData);
            WriteCountryData(countryData, databasePath);
        }

        //парсинг файла с количеством торговых марок (получен из WIPO)
        static public void ParseTrademarkApplicationsWIPO(string filePath, string databasePath, string mapPath)
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
                        //рассматриваются только итоговые, Total
                        if ((!line.Contains("Abroad"))&&(!line.Contains("Resident")))
                        {
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
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }

            List<CountryData> countryData = GetCountryData(databasePath);
            countryData = AddCountryTrademarkInfo(data, countryData);
            WriteCountryData(countryData, databasePath);
        }

        //парсинг файла с экспортом высоких технологий
        static public void ParseHighTechExportsWorldBank(string filePath, string databasePath, string mapPath)
        {
            Dictionary<string, double> data = new Dictionary<string, double>();

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
                        //разбить строку по запятым
                        string[] dataParts = line.Split(',');
                        //в строках данных по странам 66 частей
                        if ((dataParts.Length == 66) && (dataParts[0] != "Country Name")) 
                        {
                            //нужно получить последние известные данные
                            //отбор с 2016 по 2019 год
                            string value = "";

                            //2019
                            if (dataParts[63] != "")
                                value = dataParts[63];
                            //2018
                            else if (dataParts[62] != "")
                                value = dataParts[62];
                            //2017
                            else if (dataParts[61] != "")
                                value = dataParts[61];
                            //2016
                            else if (dataParts[60] != "")
                                value = dataParts[60];

                            //поменять десятичный разделитель для конвертации
                            value = value.Replace('.', ',');

                            //добавление информации о стране
                            if (value != "")
                                data.Add(dataParts[0], Math.Round(Convert.ToDouble(value), 3));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }

            List<CountryData> countryData = GetCountryData(databasePath);
            countryData = AddCountryHighTechExportsInfo(data, countryData);
            WriteCountryData(countryData, databasePath);
            EditMapJSON(countryData, mapPath);
        }

        //перенести информацию о торговых марках в данные о странах
        static private List<CountryData> AddCountryTrademarkInfo(Dictionary<string, int> newInfo, List<CountryData> countryData)
        {
            //перебор стран из списка
            for (int i = 0; i < countryData.Count(); i++)
            {
                //если текущая страна упоминается в словаре, то информация переносится
                if (newInfo.ContainsKey(countryData[i].Name))
                {
                    countryData[i].Total_Trademark_Applications = newInfo[countryData[i].Name];
                }
            }

            return countryData;
        }

        //перенести информацию о патентах в данные о странах
        static private List<CountryData> AddCountryPatentsInfo(Dictionary<string, int> newInfo, List<CountryData> countryData)
        {
            //перебор стран из списка
            for (int i = 0; i < countryData.Count(); i++)
            {
                //если текущая страна упоминается в словаре, то информация переносится
                if (newInfo.ContainsKey(countryData[i].Name))
                {
                    countryData[i].Total_Patent_Applications = newInfo[countryData[i].Name];
                }
            }

            return countryData;
        }

        //перенести информацию об экспорте высоких технологий в данные о странах
        static private List<CountryData> AddCountryHighTechExportsInfo(Dictionary<string, double> newInfo, List<CountryData> countryData)
        {
            //перебор стран из списка
            for (int i = 0; i < countryData.Count(); i++)
            {
                //если текущая страна упоминается в словаре, то информация переносится
                if (newInfo.ContainsKey(countryData[i].Name))
                {
                    countryData[i].High_Tech_Exports = newInfo[countryData[i].Name];
                }
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

                            //патенты
                            if ((dataParts[1] != "NULL")&&(dataParts[1] != ""))
                                countryData.Total_Patent_Applications = Convert.ToInt32(dataParts[1]);
                            else 
                                countryData.Total_Patent_Applications = -1;
                            //торговые марки
                            if ((dataParts[2] != "NULL") && (dataParts[2] != ""))
                                countryData.Total_Trademark_Applications = Convert.ToInt32(dataParts[2]);
                            else
                                countryData.Total_Trademark_Applications = -1;
                            //экспорт высоких технологий
                            if ((dataParts[3] != "NULL") && (dataParts[3] != ""))
                                countryData.High_Tech_Exports = Convert.ToInt32(dataParts[3]);
                            else
                                countryData.High_Tech_Exports = -1;

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
                    sw.WriteLine("country,total patent applications,total trademark applications");
                    for (int i = 0; i < data.Count(); i++)
                    {
                        //строка данных об одной стране
                        string dataStr = data[i].Name + ",";

                        //патенты
                        if (data[i].Total_Patent_Applications != -1)
                            dataStr = dataStr + data[i].Total_Patent_Applications + ",";
                        else 
                            dataStr = dataStr + "NULL,";

                        //торговые марки
                        if (data[i].Total_Trademark_Applications != -1)
                            dataStr = dataStr + data[i].Total_Trademark_Applications + ",";
                        else
                            dataStr = dataStr + "NULL,";

                        //экспорт высоких технологий
                        if (data[i].High_Tech_Exports != -1)
                        {
                            string str = data[i].High_Tech_Exports.ToString().Replace(',', '.');
                            dataStr = dataStr + str + ",";
                        }
                        else
                            dataStr = dataStr + "NULL,";

                        sw.WriteLine(dataStr);
                    }
                }
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }
        }

        //редактировать geoJSON
        static private void EditMapJSON(List<CountryData> data, string mapPath)
        {
            //текущий текст JSON'а
            string currentText = "";
            //обновленный текст JSON'а
            string newText = "";

            //прочитать JSON-файл карты
            try
            {
                using (StreamReader sr = new StreamReader(mapPath))
                {
                    currentText = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }

            //обработка текста JSON'a с добавлением показателя
            string[] dataParts = currentText.Split('\n');

            int counter = 0;
            for (int i = 0; i < dataParts.Length; i++)
            {
                //первые строки без данных о странах
                if (i < 4)
                    newText = newText + dataParts[i] + "\n";
                else
                {
                    //найти запятую, после которой можно записать значение показателя
                    int location = dataParts[i].IndexOf("ADMIN");
                    if (location > 0)
                    {
                        char currentSymbol = dataParts[i][location];
                        while (currentSymbol != ',')
                        {
                            location++;
                            currentSymbol = dataParts[i][location];
                        }
                        location++;

                        //запись значений показателей
                        string strPatent = "";
                        string strTrademark = "";
                        string strHighTechExports = "";


                        //если этот показатель не был записан ранее
                        if (!dataParts[i].Contains("total_patent_applications")) 
                        {
                            if (data[counter].Total_Patent_Applications != -1)
                                strPatent = " \"total_patent_applications\": " + data[counter].Total_Patent_Applications.ToString() + ",";
                            else strPatent = " \"total_patent_applications\": " + "null" + ",";
                        }
                        if (!dataParts[i].Contains("total_trademark_applications")) 
                        {
                            if (data[counter].Total_Trademark_Applications != -1)
                                strTrademark = " \"total_trademark_applications\": " + data[counter].Total_Trademark_Applications.ToString() + ",";
                            else strTrademark = " \"total_trademark_applications\": " + "null" + ",";
                        }
                        if(!dataParts[i].Contains("high_tech_exports"))
                        {
                            if (data[counter].High_Tech_Exports != -1)
                                strHighTechExports = " \"high_tech_exports\": " + data[counter].High_Tech_Exports.ToString().Replace(',', '.') + ",";
                            else strHighTechExports = " \"high_tech_exports\": " + "null" + ",";
                        }

                        dataParts[i] = dataParts[i].Insert(location, strPatent + strTrademark + strHighTechExports);
                        
                        counter++;
                        newText = newText + dataParts[i] + "\n";
                    }
                }
            }
            newText = newText + "]\n";
            newText = newText + "}\n";

            //запись в файл
            try
            {
                using (StreamWriter sw = new StreamWriter(mapPath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(newText);
                }
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }
        }
    }
}