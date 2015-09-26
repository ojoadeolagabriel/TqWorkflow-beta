using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace app.core.utility
{
    /// <summary>
    /// CsvReader Class.
    /// </summary>
    public class CsvDao
    {
        public static FileInfo SafeFileRead(string basePath, string foldername)
        {
            var fullPath = Path.Combine(basePath, foldername);
            return !Directory.Exists(fullPath) ? null : new DirectoryInfo(fullPath).GetFiles("*.csv").FirstOrDefault();
        }

        /// <summary>
        /// Safe File Write
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="foldername"></param>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <param name="tempExtension"></param>
        /// <returns></returns>
        public static bool SafeFileWrite(string basePath, string foldername, string fileName,
            string content, string tempExtension = "temp")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(tempExtension) && tempExtension.Contains("."))
                    tempExtension = tempExtension.Replace(".", "");

                if (string.IsNullOrEmpty(foldername))
                    foldername = "";

                var folderPath = Path.Combine(basePath, foldername);
                var path = Path.Combine(basePath, foldername, fileName);
                var fileExtension = Path.GetExtension(path);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                if (File.Exists(path))
                {
                    var fname = Path.GetFileNameWithoutExtension(path);
                    path = Path.Combine(basePath, foldername, string.Format("{0}_{1}{2}", fname, DateTime.Now.Ticks, fileExtension));
                }

                path = Path.ChangeExtension(path, tempExtension);

                //write new
                File.WriteAllText(path, content);

                File.Copy(path, Path.ChangeExtension(path, fileExtension));
                File.Delete(path);
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public bool AnyBadRow
        {
            get
            {
                return _badCsvRowData.Any();
            }
        }

        public bool AnyGoodRow
        {
            get
            {
                return _goodCsvRowData.Any();
            }
        }

        /// <summary>
        /// Verify Csv Row
        /// </summary>
        /// <param name="validatorFunc"></param>
        /// <param name="rowIndex"></param>
        /// <param name="respCode"></param>
        /// <returns></returns>
        public bool IsRowValid(Func<List<string>, bool> validatorFunc, int rowIndex, out string respCode)
        {
            respCode = "";
            var row = GetRow(rowIndex);
            if (row == null)
                return true;

            try
            {
                return validatorFunc(row);
            }
            catch (Exception exception)
            {
                respCode = exception.Message;
                return false;
            }
        }

        public enum CsvStorageType
        {
            Original, Good, Bad
        }

        /// <summary>
        /// Index Of Row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public int IndexOfRow(List<string> row)
        {
            if (_csvRowData == null)
                return -1;

            return _csvRowData.IndexOf(row);
        }

        /// <summary>
        /// Take n contigious elements.
        /// </summary>
        /// <param name="rowStartId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<List<string>> FetchAll(int rowStartId, int count = 0)
        {
            try
            {
                if (count == 0)
                    return _csvRowData == null ? null : _csvRowData.Skip(rowStartId).ToList();

                return _csvRowData == null ? null : _csvRowData.Skip(rowStartId).Take(count).ToList();

            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Fetch Single
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public List<string> FetchSingle(int rowId)
        {
            try
            {
                return _csvRowData == null ? null : _csvRowData.Skip(rowId).Take(1).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool UpdateRow(int startIndex, List<string> rowUpdate)
        {
            try
            {
                var row = GetRow(startIndex);
                if (row != null)
                    row = rowUpdate;

                _csvRowData[startIndex] = row;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly List<List<String>> _csvRowData = new List<List<string>>();
        /// <summary>
        /// 
        /// </summary>
        private readonly List<List<String>> _badCsvRowData = new List<List<string>>();
        /// <summary>
        /// 
        /// </summary>
        private readonly List<List<String>> _goodCsvRowData = new List<List<string>>();
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, List<String>> RowMap = new Dictionary<int, List<string>>();

        /// <summary>
        /// Copy Row To Bad Csv Data.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="optionalColumnData"></param>
        /// <returns></returns>
        public bool MarkRowAsBad(int rowIndex, string optionalColumnData = null)
        {
            try
            {
                var rowData = GetRow(rowIndex);
                if (rowData == null)
                    return false;

                var cloneRow = new List<string>();
                rowData.ForEach(cloneRow.Add);

                if (!string.IsNullOrWhiteSpace(optionalColumnData))
                    AppendColumn(rowIndex, optionalColumnData, cloneRow);

                //if (!copy)
                //    CsvRowData.Remove(rowData);
                _badCsvRowData.Add(cloneRow);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Adds selected row to bad csv collection 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="dao"></param>
        /// <param name="optionalColumnData"></param>
        /// <param name="csvDaoFunc"></param>
        /// <returns></returns>
        public static bool MarkSelectedRowRecordAsBad(List<string> row,
            CsvDao dao = null, string optionalColumnData = null, Action<CsvDao> csvDaoFunc = null)
        {
            try
            {
                if (csvDaoFunc != null && dao != null)
                {
                    csvDaoFunc(dao);
                }

                var rowData = row;
                if (rowData == null)
                    return false;

                var cloneRow = new List<string>();
                rowData.ForEach(cloneRow.Add);

                if (!string.IsNullOrWhiteSpace(optionalColumnData))
                    AppendColumnData(optionalColumnData, cloneRow);

                if (dao != null) dao._badCsvRowData.Add(cloneRow);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool MarkMultipleRowsAsBad(int startingRowIndex, string conditionalExtraColumnData = null)
        {
            try
            {
                var rows = FetchAll(startingRowIndex);
                var clonedRows = CloneRows(rows);
                if (!string.IsNullOrWhiteSpace(conditionalExtraColumnData))
                {
                    clonedRows.ForEach(c => c.Add(conditionalExtraColumnData));
                }
                _badCsvRowData.AddRange(rows);

                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public static List<List<string>> CloneRows(List<List<string>> rows)
        {
            return rows.Select(originalRow => originalRow.ToList()).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startingRowIndex"></param>
        /// <param name="conditionalExtraColumnData"></param>
        /// <returns></returns>
        public bool MarkMultipleRowsAsGood(int startingRowIndex, string conditionalExtraColumnData = null)
        {
            try
            {
                var rows = FetchAll(startingRowIndex);
                var clonedRows = CloneRows(rows);
                if (!string.IsNullOrWhiteSpace(conditionalExtraColumnData))
                {
                    clonedRows.ForEach(c => c.Add(conditionalExtraColumnData));
                }
                _goodCsvRowData.AddRange(rows);
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Copy Row To Good Data Collection
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="optionalColumnData"></param>
        /// <returns></returns>
        public bool MarkRowAsGood(int rowIndex, string optionalColumnData = null)
        {
            try
            {
                var rowData = GetRow(rowIndex);
                if (rowData == null)
                    return false;

                var cloneRow = new List<string>();
                rowData.ForEach(cloneRow.Add);

                if (!string.IsNullOrWhiteSpace(optionalColumnData))
                    AppendColumn(rowIndex, optionalColumnData, cloneRow);


                _goodCsvRowData.Add(cloneRow);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Adds selected row to good csv collection 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="dao"></param>
        /// <param name="optionalColumnData"></param>
        /// <param name="csvDaoFunc"></param>
        /// <returns></returns>
        public static bool MarkSelectedRowRecordAsGood(List<string> row, CsvDao dao = null, string optionalColumnData = null, Action<CsvDao> csvDaoFunc = null)
        {
            try
            {
                if (csvDaoFunc != null && dao != null)
                {
                    csvDaoFunc(dao);
                }

                var rowData = row;
                if (rowData == null)
                    return false;

                var cloneRow = new List<string>();
                rowData.ForEach(cloneRow.Add);

                if (!string.IsNullOrWhiteSpace(optionalColumnData))
                    AppendColumnData(optionalColumnData, cloneRow);

                if (dao != null) dao._goodCsvRowData.Add(cloneRow);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Update Row Map
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool AddRowMap(int rowIndex, String map)
        {
            try
            {
                var rowMap = GetRowMap(rowIndex);
                if (rowMap != null || string.IsNullOrWhiteSpace(map))
                    return false;

                var mapParts = map.Split(',').ToList();
                mapParts = mapParts.Select(c => c.Trim()).ToList();
                RowMap.Add(rowIndex, mapParts);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private int GetIndexOfRowMap(int rowIndex, string columnName)
        {
            try
            {
                var rowMap = GetRowMap(rowIndex);
                if (rowMap == null)
                    return -1;

                return rowMap.IndexOf(columnName);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Add Row
        /// </summary>
        /// <param name="rowData"></param>
        public void AddRow(List<String> rowData)
        {
            if (_csvRowData != null)
                _csvRowData.Add(rowData);
        }

        /// <summary>
        /// Get Row.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<string> GetRow(int index)
        {
            try
            {
                return _csvRowData[index];
            }
            catch
            {
                return null;
            }
        }

        private List<string> GetRowMap(int index)
        {
            try
            {
                return RowMap[index];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get Column.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public string GetColumn(int index, List<String> rowData)
        {
            try
            {
                return rowData[index];
            }
            catch
            {
                return null;
            }
        }

        public T GetColumn<T>(int rowIndex, int columnIndex, T defaultData = default(T))
        {
            try
            {
                var row = GetRow(rowIndex);
                if (row == null)
                    return defaultData;

                var columnData = row[columnIndex];
                return (T)Convert.ChangeType(columnData, typeof(T));
            }
            catch
            {
                return defaultData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnIndex"></param>
        /// <param name="row"></param>
        /// <param name="defaultData"></param>
        /// <param name="throwOnError"></param>
        /// <param name="errorMessage"></param>
        /// <param name="throwOnColumnNotSet"></param>
        /// <returns></returns>
        public static T GetColumnData<T>(int columnIndex, List<string> row, T defaultData = default(T), 
            bool throwOnError = false, string errorMessage = "", bool throwOnColumnNotSet = false)
        {
            try
            {
                if (row == null)
                    return defaultData;

                var columnData = row[columnIndex];

                if(string.IsNullOrWhiteSpace(columnData) && throwOnColumnNotSet)
                    throw new Exception(errorMessage);

                return (T)Convert.ChangeType(columnData, typeof(T));
            }
            catch (Exception exception)
            {
                if (!throwOnError && !throwOnColumnNotSet) 
                    return defaultData;

                if (string.IsNullOrEmpty(errorMessage))
                    throw;
                throw new Exception(errorMessage);
            }
        }

        public T GetColumnByMap<T>(int rowIndex, string columnName, T defaultData = default(T))
        {
            try
            {
                var row = GetRow(rowIndex);
                if (row == null || string.IsNullOrWhiteSpace(columnName))
                    return defaultData;

                var columnIndex = GetIndexOfRowMap(rowIndex, columnName.Trim());
                if (columnIndex == -1)
                    return defaultData;

                var columnData = row[columnIndex];
                return (T)Convert.ChangeType(columnData, typeof(T));
            }
            catch
            {
                return defaultData;
            }
        }


        /// <summary>
        /// Add Column.
        /// </summary>
        /// <param name="rowNdx"></param>
        /// <param name="columnData"></param>
        /// <returns></returns>
        public bool AppendColumn(int rowNdx, string columnData)
        {
            var row = GetRow(rowNdx);
            if (row == null)
                return false;

            row.Add(columnData);
            return true;
        }

        public bool AppendColumn(int rowNdx, string columnData, List<string> rowData)
        {
            var row = rowData;
            if (row == null)
                return false;

            row.Add(columnData);
            return true;
        }

        public static bool AppendColumnData(string columnData, List<string> rowData)
        {
            var row = rowData;
            if (row == null)
                return false;

            row.Add(columnData);
            return true;
        }

        /// <summary>
        /// Insert Column
        /// </summary>
        /// <param name="rowNdx"></param>
        /// <param name="columnIndex"></param>
        /// <param name="columnData"></param>
        /// <returns></returns>
        public bool InsertColumn(int rowNdx, int columnIndex, string columnData)
        {
            var row = GetRow(rowNdx);
            if (row == null)
                return false;

            row.Insert(columnIndex, columnData);
            return true;
        }

        /// <summary>
        /// Csv Raw Data
        /// </summary>
        public string OriginalCsvData
        {
            get
            {
                var rowBuilder = new StringBuilder();
                foreach (var row in _csvRowData)
                {
                    var columnBuilder = new StringBuilder();
                    foreach (var col in row)
                    {
                        var mCol = col;
                        if (col.Contains(","))
                        {
                            mCol = col.Insert(0, "\"");
                            mCol = mCol.Insert(mCol.Length, "\"");
                        }
                        columnBuilder.AppendFormat("{0},", mCol);
                    }

                    if (columnBuilder.Length > 0)
                        columnBuilder.Remove(columnBuilder.Length - 1, 1);
                    rowBuilder.AppendLine(columnBuilder.ToString());
                }
                return rowBuilder.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string BadCsvRawData
        {
            get
            {
                var rowBuilder = new StringBuilder();
                foreach (var row in _badCsvRowData)
                {
                    var columnBuilder = new StringBuilder();
                    foreach (var col in row)
                    {
                        var mCol = col;
                        if (col.Contains(","))
                        {
                            mCol = col.Insert(0, "\"");
                            mCol = mCol.Insert(mCol.Length, "\"");
                        }
                        columnBuilder.AppendFormat("{0},", mCol);
                    }

                    if (columnBuilder.Length > 0)
                        columnBuilder.Remove(columnBuilder.Length - 1, 1);
                    rowBuilder.AppendLine(columnBuilder.ToString());
                }

                return rowBuilder.ToString();
            }
        }

        public string GoodCsvRawData
        {
            get
            {
                var rowBuilder = new StringBuilder();
                foreach (var row in _goodCsvRowData)
                {
                    var columnBuilder = new StringBuilder();
                    foreach (var col in row)
                    {
                        var mCol = col;
                        if (col.Contains(","))
                        {
                            mCol = col.Insert(0, "\"");
                            mCol = mCol.Insert(mCol.Length, "\"");
                        }
                        columnBuilder.AppendFormat("{0},", mCol);
                    }

                    if (columnBuilder.Length > 0)
                        columnBuilder.Remove(columnBuilder.Length - 1, 1);
                    rowBuilder.AppendLine(columnBuilder.ToString());
                }
                return rowBuilder.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public bool HasMinimumRowCount(int limit)
        {
            return (_csvRowData != null && _csvRowData.Count >= limit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public bool IsColumnCountGood(int rowIndex, int limit)
        {
            var row = GetRow(rowIndex);
            if (row == null)
                return false;

            return row.Count == limit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="skipFirstRow"></param>
        /// <returns></returns>
        public static CsvDao ParseFromString(string data, bool skipFirstRow = false)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;

            var reader = new CsvDao();
            var csvRows = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var rowsToSkip = skipFirstRow ? 1 : 0;

            foreach (var csvRow in csvRows.Skip(rowsToSkip))
            {
                var columns = SplitColumns(csvRow);
                var columnList = columns.Select(column => column.Trim()).ToList();
                reader._csvRowData.Add(columnList);
            }

            return reader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="skipFirstRow"></param>
        /// <returns></returns>
        public static CsvDao LoadFromBuffer(byte[] data, bool skipFirstRow = false)
        {
            try
            {
                var payload = Encoding.ASCII.GetString(data);
                return ParseFromString(payload, skipFirstRow);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Split Columns
        /// </summary>
        /// <param name="csvRow"></param>
        /// <returns></returns>
        private static IEnumerable<string> SplitColumns(string csvRow)
        {
            var totalchars = csvRow.Length;
            var fieldBuilder = new StringBuilder();
            var list = new List<String>();
            var readingComma = false;

            for (var i = 0; i <= totalchars - 1; i++)
            {
                try
                {
                    var ch = csvRow[i];

                    if (!readingComma)
                    {
                        if (ch != '"' && ch != ',')
                        {
                            fieldBuilder.Append(ch);
                            if (i != totalchars - 1) continue;
                            var field = fieldBuilder.ToString();
                            list.Add(field);
                            fieldBuilder.Clear();
                        }
                        else
                        {
                            switch (ch)
                            {
                                case ',':
                                    var field = fieldBuilder.ToString();
                                    list.Add(field);
                                    fieldBuilder.Clear();
                                    break;
                                case '"':
                                    readingComma = true;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (ch != '"')
                        {
                            fieldBuilder.Append(ch);
                            if (i != totalchars - 1) continue;
                            var field = fieldBuilder.ToString();
                            list.Add(field);
                            fieldBuilder.Clear();
                        }
                        else
                        {
                            readingComma = false;
                            var field = fieldBuilder.ToString();
                            list.Add(field);
                            fieldBuilder.Clear();
                            ++i;
                        }
                    }
                }
                catch
                {

                }
            }

            return list;
        }
    }
}
