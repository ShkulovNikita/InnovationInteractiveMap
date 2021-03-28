using System;
using System.IO;
using System.Runtime.InteropServices;
using NPOI.HSSF.UserModel;

namespace InnovationInteractiveMapApp.Classes
{
    public class FilesHandler
    {
        //удаление старого файла перед созданием нового
        static private void DeletePreviousFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        static public void CreateExcelFile(string filePath, string databasePath)
        {
            //удалить предыдущий файл
            DeletePreviousFile(filePath);

            //создание книги Эксель
            var workbook = new HSSFWorkbook();

            //добавление листа
            var sheet = workbook.CreateSheet("Лист данных");

            //заполнение данных
            try
            {
                //чтение данных из database.csv
                using (StreamReader sr = new StreamReader(databasePath))
                {
                    string line;
                    int i = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //разбиваем данные очередной строки по запятым
                        string[] dataParts = line.Split(',');

                        //создание очередной строки
                        var row = sheet.CreateRow(i);

                        //если это заголовок
                        if (i == 0)
                        {
                            for (int j = 0; j < dataParts.Length; j++)
                            {
                                var cell = row.CreateCell(j);
                                switch (dataParts[j])
                                {
                                    case "country":
                                        cell.SetCellValue("Страна");
                                        break;
                                    case "total patent applications":
                                        cell.SetCellValue("Количество заявлений на патенты");
                                        break;
                                    case "total trademark applications":
                                        cell.SetCellValue("Количество заявлений на торговые марки");
                                        break;
                                    case "high-technology exports":
                                        cell.SetCellValue("Экспорт высоких технологий (в % от производимого экспорта)");
                                        break;
                                    case "high-technology exports (USD)":
                                        cell.SetCellValue("Экспорт высоких технологий (в долларах США)");
                                        break;
                                    case "research and development expenditure":
                                        cell.SetCellValue("Затраты на НИОКР (в % от ВВП)");
                                        break;
                                    case "payment for intellectual property":
                                        cell.SetCellValue("Оплата интеллектуальной собственности (в долларах США)");
                                        break;
                                    default:
                                        cell.SetCellValue("");
                                        break;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < dataParts.Length; j++)
                            {
                                var cell = row.CreateCell(j);
                                if (dataParts[j] != "NULL")
                                    cell.SetCellValue(dataParts[j]);
                                else
                                    cell.SetCellValue("");
                            }
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }

            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(stream);
            }
        }
    }
}