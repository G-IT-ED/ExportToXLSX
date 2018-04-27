using System;
using System.Collections.Generic;
using DevExpress.Spreadsheet;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraPrinting;
using RetroToFileExporter.Core.Interfaces;
using RSDU.Database.Mappers.Helpers;
using RSDU.Domain;
using RSDU.Messaging;

namespace RetroToFileExporter.Core.Models
{
    class DataExporter : IDataExporter
    {
        public DataExporter(IAction action, RetroKadr kadr, List<ParamData> archives, List<QueryConditionsDates> conditionsDate)
        {
            Action = action;
            Kadr = kadr;
            Archives = archives;
            ConditionsDate = conditionsDate;
        }

        public override string ToString()
        {
            String s = String.Format("DataExporter INFO: [Action:{0} ({1}); Retro:{2};]", Action.GuidAction, Action.NameTrigger, Kadr.Id);

            if (ConditionsDate != null && ConditionsDate.Count > 0)
            {
                s += Environment.NewLine;
                s += "Periods:" + Environment.NewLine;
                foreach (QueryConditionsDates c in ConditionsDate)
                    s += c.ToString();
            }

            return s;
        }

        public IAction Action { get; set; }
        public RetroKadr Kadr { get; set; }
        public List<ParamData> Archives { get; set; }
        public List<QueryConditionsDates> ConditionsDate { get; set; }

        /// <summary>
        /// Запись эксель файла
        /// </summary>
        /// <param name="infoRows">Список с данными из экспортируемых строк</param>
        /// <param name="savingFileName"></param>
        /// <param name="format">Формат файла (Xls, Xlsx)</param>
        /// <param name="kadrRows"></param>
        /// <returns></returns>
        private bool TryWriteExcellFile(object[,] infoRows, List<KardExportData> kadrRows, string savingFileName, string format)
        {
#if DEBUG
            Log.Info("Запись эксель файла " + Action.KadrId);
#endif
            using (Workbook workbook = new Workbook())
            {
                SetSheetInfo(workbook,infoRows);
                foreach (var kadr in kadrRows)
                {
                    SetSheetKadr(workbook, kadr);
                }

                //Устанавливаем параметры экспорта в csv
                if (format == ExportFormat.Csv.ToString())
                {
                    workbook.Options.Export.Csv.ValueSeparator = ';';
                    workbook.Options.Export.Csv.Encoding = Encoding.GetEncoding(1251);
                    workbook.Options.Export.Csv.FormulaExportMode = DevExpress.XtraSpreadsheet.Export.FormulaExportMode.CalculatedValue;
                    workbook.Options.Export.Csv.TextQualifier = '"';
                }

                //Сохраняем файл в выбранном формате
                workbook.SaveDocument(savingFileName, GetDocumentFormat(format));
                workbook.Dispose();
            }
#if DEBUG
            Log.Info("Записан эксель файла " + Action.KadrId);
#endif
            return true;
        }

        private void SetSheetKadr(Workbook workbook, KardExportData data)
        {
            Worksheet kadrSheet;

            //Определяем с какого листа начинать запись
            if (!Action.Info && workbook.Worksheets.Count == 1)
                kadrSheet = workbook.Worksheets[0];
            else
                kadrSheet = workbook.Worksheets.Add();
            
            kadrSheet.Name = data.Interval;

            var arraySize = GetArraySize(data);
            var x = arraySize.X;
            var y = arraySize.Y;

            string columns = GetExcelColumnName(y + 1);
            int rows = GetXCount(x);

            kadrSheet[columns+ rows].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
            kadrSheet[columns + rows].Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            
            for (int i = 0; i < data.Data.GetLength(0); i++)
            {
                for (int j = 0; j < data.Data.GetLength(1); j++)
                {
                    var val = data.Data[i, j];
                    if (val is DateTime)
                    {
                        kadrSheet.Cells[i, j].NumberFormat = "dd.MM.yyyy HH:mm";
                        kadrSheet.Cells[i, j].SetValue((DateTime)val);
                    }
                    if (val is int)
                        kadrSheet.Cells[i, j].SetValue((int)val);

                    if (val is Double)
                        kadrSheet.Cells[i, j].SetValue((Double)val);

                    if (!(val is int) && !(val is Double) && !(val is DateTime))
                        kadrSheet.Cells[i, j].SetValue(val);
                    
                    if (i < x && j < y)
                        kadrSheet.Cells[i, j].Borders.SetOutsideBorders(Color.Black, BorderLineStyle.Thin);
                }
            }
            kadrSheet.Columns.AutoFit(0,y);
        }

        private int GetXCount(int x)
        {
            return ((x > 65000) ? 65000 : x + 1);
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        private Point GetArraySize(KardExportData data)
        {
            var i = 0;
            var row = 0;
            var result = new Point();
            while (true)
            {
                if (data.Data[i, 0] == @"Дата/время")
                    row = i;
                if (data.Data[i, 0] == null)
                {
                    result.X = i;
                    break;
                }
                i++;
            }
            var j = 0;
            while (true)
            {
                if (data.Data[row, j] == null)
                {
                    result.Y = j;
                    break;
                }
                j++;
            }
            return result;
        }

        /// <summary>
        /// Формируем первую страницу Инфо
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="infoRows"></param>
        private void SetSheetInfo(Workbook workbook, object[,] infoRows)
        {
            if (!Action.Info) return;
            Worksheet infoSheet = workbook.Worksheets[0];
            infoSheet.Name = "Инфо";

            //Запись строк с данными
            for (int i = 0; i < infoRows.GetLength(0); i++)
            {
                for (int j = 0; j < infoRows.GetLength(1); j++)
                {
                    var val = infoRows[i, j];
                    if (val is DateTime)
                    {
                        infoSheet.Cells[i, j].NumberFormat = "dd.MM.yyyy HH:mm";
                        infoSheet.Cells[i, j].SetValue((DateTime)val);
                    }
                    if (val is Double)
                    {
                        infoSheet.Cells[i, j].SetValue((Double)val);                        
                    }
                    if (val is int)
                    {
                        infoSheet.Cells[i, j].SetValue((int)val);
                    }

                    if (!(val is int) && !(val is Double) && !(val is DateTime))
                        infoSheet.Cells[i, j].SetValue(val);
                    SetBoldStyle(val, infoSheet.Cells[i, j]);
                }
            }

            infoSheet.Columns.AutoFit(0,infoRows.GetLength(0));
        }

        private void SetBoldStyle(object infoRow, Cell cell)
        {
            if (infoRow != null && (infoRow.ToString() == "Информация" || infoRow.ToString() == "Условия запроса данных:"))
            {
                SpreadsheetFont cellFont = cell.Font;
                cellFont.Bold = true;
            }
        }

        private static readonly int _defCountColmns = 100;
        private static readonly int _countRows = 100;
        private static readonly int _defCountRows = 1000;
        /// <summary>
        /// Количество строк шапки (Объект, Название параметра...)
        /// </summary>
        private static readonly int _countHeaderRows = 7;
        /// <summary>
        /// Количество столбцов Значение и Статус и дата
        /// </summary>
        private static readonly int _countColumnsArchives = 3;

        /// <summary>
        /// Формируем версию excel
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private DocumentFormat GetDocumentFormat(string format)
        {
            switch (format.ToLowerInvariant())
            {
                case "xls":
                case ".xls":
                    return DocumentFormat.Xls;
                case "xlsx":
                case ".xlsx":
                    return DocumentFormat.Xlsx;
                default:
                    return DocumentFormat.Xls;
            }
        }

        private object[,] GetKadrRows(List<ParamData> datas, Dictionary<DateTime, DateTime> startFinish)
        {
#if DEBUG
            Log.Info("Формируем строки на экспорт начало " + Action.KadrId);
#endif
            if(datas == null || datas.First() == null || datas.First().Archive == null || datas.First().Archive.Ginfo == null|| datas.First().Archive.Ginfo.GTopt == null )
                return new string[0,0];
            Dictionary<DateTime, int> dateColumn = new Dictionary<DateTime, int>();
            Dictionary<string, int> _dicHead = new Dictionary<string, int>(); 

            var rowCount = GetRowCount(startFinish, datas.First().Archive.Ginfo.GTopt.Interval) + _defCountRows;

        var result = new object[rowCount, (datas.Count * 3) + _defCountColmns];
            result = SetHeader(result, out _dicHead);
            result = SetHeaderData(result, datas, _dicHead);

            int rowStart = SetFirstRowHeader(result, datas);
            result = SetDateColumn(result, datas.First().Archive.Ginfo.GTopt.Interval, startFinish, rowStart, out dateColumn, rowCount);

            for (int i = 0; i < datas.Count; i++)
            {
                if (Action.StateColumnHeader)
                {
                    result[rowStart - 1, 1 + (i*2)] = "Значение";
                    result[rowStart - 1, 2 + (i*2)] = "Статус";
                }
                else
                    result[rowStart - 1, 1 + (i)] = "Значение";
                result = SetData(i, datas[i].Periods, result, dateColumn);
            }
#if DEBUG
            Log.Info("Формируем строки на экспорт окончание " + Action.KadrId);
#endif
            return result;
        }

        private int fiveSecond = 5;
        private int GetRowCount(Dictionary<DateTime, DateTime> startFinish, int interval)
        {

            if (interval == 0) interval = fiveSecond;
            var result = 0;
            foreach (var key in startFinish.Keys)
            {
                bool b = true;
                var date = key;
                while (b)
                {
                    date = date.AddSeconds(interval);
                    result++;
                    if (date > startFinish[key]) b = false;
                }
            }
            return result;
        }

        private object[,] SetData(int i, List<DataArcPeriod> periods, object[,] result, Dictionary<DateTime, int> dateColumn)
        {
            foreach (DataArcPeriod t in periods)
            {
                try
                {
                    if (Action.StateColumnHeader)
                    {
                        result[dateColumn[t.Date.LocalTime], 1 + (i * 2)] = t.Value;
                        result[dateColumn[t.Date.LocalTime], 2 + (i * 2)] = t.State;
                    }
                    else
                        result[dateColumn[t.Date.LocalTime], 1 + (i)] = t.Value;
                }
                catch (Exception)
                {
                    // Игнорируется чтобы уменьшить дополнительный проход по Dictionary
                }
            }
            return result;
        }

        private object[,] SetDateColumn(object[,] result, int interval, Dictionary<DateTime, DateTime> startFinish, int rowStart, out Dictionary<DateTime, int> dateColumn, int rowCount)
        {
            if (interval == 0) interval = fiveSecond;
            dateColumn = new Dictionary<DateTime, int>();
            int row = rowStart;
            foreach (var key in startFinish.Keys)
            {
                var date = key;
                while (true)
                {
                    if (row == rowCount-1) break;
                    
                    if (!dateColumn.ContainsKey(date))
                    {
                        result[row, 0] = date;
                        dateColumn.Add(date, row);
                        row++;
                    }
                    date = date.AddSeconds(interval);
                    
                    if (date > startFinish[key]) break;
                }
            }
            return result;
        }

        

        private object[,] SetHeader(object[,] result, out Dictionary<string,int> _dicHead)
        {
            _dicHead = new Dictionary<string, int>(); 
            List<string> list = new List<string>();
            if (Action.ObjectField) list.Add("Объект");
            list.Add("Название параметра");
            if (Action.TypeParam)     list.Add("Тип параметра");
            if (Action.IdParam)       list.Add("ID параметра");
            if (Action.IdTableParam)  list.Add("ID таблицы");
            if (Action.NameArchTable) list.Add("Архивная таблица");
            list.Add("Дата/время");
            for (int i = 0; i < list.Count; i++)
            {
                result[i, 0] = list[i];
                _dicHead.Add(list[i],i);
            }
            return result;
        }

        private object[,] SetHeaderData(object[,] result, List<ParamData> datas, Dictionary<string,int> _dicHead)
        {
            var countState = Action.StateColumnHeader ? 1 : 0;
            for (int i = 0; i < datas.Count; i++)
            {
                result = SetHeaderDataParam(datas[i], result, 1 + i + (i * countState), _dicHead);
            }
            return result;
        }

        private object[,] SetHeaderDataParam(ParamData paramData, object[,] result, int columnNumber, Dictionary<string, int> _dicHead)
        {
            var header = new HeaderData(columnNumber, _dicHead);
            if (paramData.Archive == null || paramData.Archive.Measure == null)
                return result;
            var node = paramData.Archive.Measure.Node;
            if (node != null)
                result = header.Write("Объект", paramData.Archive.Measure.Node.Name, result);
            result = header.Write("Название параметра", paramData.Archive.Measure.Name, result);
            result = header.Write("Тип параметра", paramData.Archive.Measure.Unit, result);
            result = header.Write("ID параметра", paramData.Archive.Measure.IdParam, result);
            result = header.Write("ID таблицы", paramData.Archive.Measure.IdTable, result);
            result = header.Write("Архивная таблица", paramData.Archive.Retfname, result);
            return result;
        }

        private int SetFirstRowHeader(object[,] result, List<ParamData> datas)
        {
            for (var i = 0; i < datas.Max(x => x.Periods.Count) + _defCountRows; i++)
            {
                if (result[i,0] == null)
                {
                    return i;
                }
            }
            throw new Exception("Неверно обработано вычисление первой строки");
        }

        /// <summary>
        /// Получаем страницу Инфо
        /// </summary>
        /// <returns></returns>
        private object[,] GetInfoRows()
        {
#if DEBUG
            Log.Info("Страница инфо начало" + Action.KadrId);
#endif
            var result = new object[_defCountColmns, _countRows];
            result[0, 0] = "Информация";
            result[1, 0] = "Триггер";
            result[1, 1] = Action.NameTrigger;
            result[2, 0] = "Дата срабатывания";
            result[2, 1] = DateTime.Now;;
            result[4, 0] = "Кадр ретроспективы";
            if (Kadr != null) 
                result[4, 1] = Kadr.Name;
            result[5, 0] = "ID";
            result[5, 1] = Action.KadrId;
            result[7, 0] = "Условия запроса данных:";
            for (int i = 0; i < ConditionsDate.Count; i++)
            {
                var q = ConditionsDate[i];
                result[8 + i, 0] = "Условие " + i + 1;
                result[8 + i, 1] = q.QueryCondition.DirectionTime + ", " + q.QueryCondition.Interval + ", " + q.QueryCondition.IntervalCount + ", " + q.QueryCondition.Offset;
                result[8 + i, 2] = "с";
                result[8 + i, 3] = q.Start;
                result[8 + i, 4] = "по";
                result[8 + i, 5] = q.Finish;
            }
#if DEBUG
            Log.Info("Страница инфо окончание" + Action.KadrId);
#endif
            return result;
        }

        public bool Export(Dictionary<DateTime, DateTime> startFinish, DateTime dt)
        {
            Log.Info(String.Format("Выполняется экспорт:{0}{1} ",Environment.NewLine, ToString()));
            try
            {
#if DEBUG
                Log.Info("Экспорт начало " + Action.KadrId);
#endif
                var kadrRows = new List<KardExportData>();
                var infoRows = GetInfoRows();

                //Разбиваем на страницы с разными интервалами архивов c группированнами периодами
                List<ParamData> paramDatas = new List<ParamData>();
                foreach (var paramData in Archives)
                {
                    var isHaveParam = false;
                    foreach (var data in paramDatas)
                    {
                        if (data.Archive.Retfname == paramData.Archive.Retfname
                            && data.Archive.Ginfo.GTopt.DefineAlias == paramData.Archive.Ginfo.GTopt.DefineAlias)
                        {
                            isHaveParam = true;
                            break;
                        }
                    }
                    if (isHaveParam)
                    {
                        var pData = paramDatas.First(x => x.Archive.Retfname == paramData.Archive.Retfname);
                        foreach (var dataArcPeriod in paramData.Periods)
                        {
                            if (!pData.Periods.Contains(dataArcPeriod))
                                pData.Periods.Add(dataArcPeriod);
                        }
                        foreach (var data in paramDatas)
                        {
                            if (data.Archive.Retfname == pData.Archive.Retfname)
                                data.Periods = pData.Periods;
                        }
                    }
                    else
                    {
                        paramDatas.Add(paramData);
                    }
                }
                var archives = paramDatas.GroupBy(x => x.Archive.Ginfo.GTopt.DefineAlias).ToList();

                foreach (var archive in archives)
                {
                    kadrRows.Add(new KardExportData(GetKadrRows(archive.ToList(), startFinish),archive.Key,archive.ToList()));
                }

                var result =  TryWriteExcellFile(infoRows, kadrRows, GetFileNamePath(Action.FilesFolder, Action.FileName, dt), Action.VersionExcel);
#if DEBUG
                Log.Info("Экспорт закончен " + Action.KadrId);
#endif
                return result;
            }
            catch (Exception e)
            {
                Log.Error("ОШИБКА!!! Возникла ошибка при экспорте в файл! GuidAction=" + Action.GuidAction, e);
                return false;
            }
        }

        private string GetFileNamePath(string filesFolder, string fileName, DateTime dt)
        {
            filesFolder = GetTemplateName(filesFolder, dt);
            fileName = GetTemplateName(fileName, dt);

            return System.IO.Path.Combine(filesFolder, fileName);
        }

        private string GetTemplateName(string text, DateTime dt)
        {
            return text.Replace("{YY}", dt.Year.ToString().Substring(2, 2))
                       .Replace("{YYYY}", dt.Year.ToString())
                       .Replace("{MM}", (dt.Month < 10) ? "0" + dt.Month : dt.Month.ToString())
                       .Replace("{DD}", (dt.Day < 10) ? "0" + dt.Day : dt.Day.ToString())
                       .Replace("{HH}", (dt.Hour < 10) ? "0" + dt.Hour : dt.Hour.ToString())
                       .Replace("{mm}", (dt.Minute < 10) ? "0" + dt.Minute : dt.Minute.ToString())
                       .Replace("{SS}", (dt.Second < 10) ? "0" + dt.Second : dt.Second.ToString());
        }
    }

    internal class HeaderData
    {
        private int _columnNumber;
        private Dictionary<string, int> _dicHead;

        public HeaderData(int columnNumber, Dictionary<string, int> dicHead)
        {
            _columnNumber = columnNumber;
            _dicHead = dicHead;
        }

        public object[,] Write(string row, object data, object[,] result)
        {
            if (!_dicHead.ContainsKey(row)) return result;
            result[_dicHead[row], _columnNumber] = data;
            return result;
        }
    }

    public class KardExportData
    {
        public object[,] Data { get; set; }
        public string Interval { get; set; }
        public List<ParamData> Archive { get; set; }

        public KardExportData(object[,] data, string interval, List<ParamData> archive)
        {
            Data = data;
            Interval = interval;
            Archive = archive;
        }
    }
}