using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BYDPlatform.Domain.Attributes;
using BYDPlatform.Domain.Constant;
using FluentValidation.Results;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace BYDPlatform.Application.Common.Tools;

public static class ExcelHelper
{
    private const int MAX_COL_WIDTH = 100 * 256;


    /// <summary>
    ///     将DataTable导出为Excel（内存流）
    /// </summary>
    /// <param name="dt">数据源DataTable</param>
    /// <param name="fs">文件流</param>
    /// <param name="sheetName">表单名称</param>
    /// <param name="headerText">表头名称</param>
    /// <param name="headerIndex">表头索引，默认-1为不需要</param>
    /// <param name="dateFormat">日期时间格式化字符串</param>
    public static MemoryStream ExportDataTable(string fileName, DataTable dt,
        string sheetName = "Sheet1", int headerIndex = -1, string headerText = "",
        string dateFormat = "yyyy-mm-dd hh:mm:ss")
    {
        //创建工作簿和sheet
        IWorkbook workbook = new HSSFWorkbook();
        using (Stream writeFile = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
        {
            if (writeFile.Length > 0 && string.IsNullOrEmpty(sheetName)) workbook = WorkbookFactory.Create(writeFile);
        }

        ISheet sheet = null;
        var dateStyle = workbook.CreateCellStyle();
        var format = workbook.CreateDataFormat();
        dateStyle.DataFormat = format.GetFormat(dateFormat);
        var arrColWidth = GetContentLength(dt);

        sheet = workbook.CreateSheet(sheetName);

        //创建表头
        var rowIndex = CreateHeader(sheet, workbook, arrColWidth, dt.Columns, headerIndex, headerText);

        //填充内容
        CreateCellContent(sheet, workbook, dt, rowIndex, dateFormat);
        var ms = new MemoryStream();
        workbook.Write(ms, true);
        ms.Flush();
        ms.Position = 0;
        return ms;
    }

    
    /// <summary>
    ///     将DataTable导出为Excel（通过文件流）
    /// </summary>
    /// <param name="dt">数据源DataTable</param>
    /// <param name="fs">文件流</param>
    /// <param name="sheetName">表单名称</param>
    /// <param name="headerText">表头名称</param>
    /// <param name="headerIndex">表头索引，默认-1为不需要</param>
    /// <param name="dateFormat">日期时间格式化字符串</param>
    public static FileStream ExportDataTable(DataTable dt,
        string sheetName = "Sheet1", string headerText = "", int headerIndex = -1,
        string dateFormat = "yyyy-mm-dd hh:mm:ss")
    {
        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = null;
        var dateStyle = workbook.CreateCellStyle();
        var format = workbook.CreateDataFormat();
        dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
        //取得列宽
        var arrColWidth = GetContentLength(dt);

        if (workbook.GetSheetIndex(sheetName) >= 0) workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName));

        sheet = workbook.CreateSheet(sheetName);

        //创建表头
        var rowIndex = CreateHeader(sheet, workbook, arrColWidth, dt.Columns, headerIndex, headerText);

        //填充内容
        CreateCellContent(sheet, workbook, dt, rowIndex, dateFormat);
        var fileName = $"temp{DateTime.Now:yyyyMMddhhmmss}.xlsx";
        var fs = new FileStream($"./{fileName}", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        workbook.Write(fs);

        return fs;
    }
    
    public static FileStream ExportDataTableWithErrorMsg<T>(
        DataTable dt,List<List<ValidationFailure>>fl,
        string sheetName="Sheet1",string headerText="",int headerIndex=-1,
        string dateFormat="yyyy-mm-dd hh:mm:ss"
        )
    {
        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet;
        var dataStyle = workbook.CreateCellStyle();
        var format = workbook.CreateDataFormat();
        dataStyle.DataFormat = format.GetFormat(dateFormat);
        //取得列宽
        var arrColWidth = GetContentLength(dt);

        if (workbook.GetSheetIndex(sheetName) >= 0) workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName));

        sheet = workbook.CreateSheet(sheetName);
        
        //创建表头
        var rowIndex = CreateHeader(sheet, workbook, arrColWidth, dt.Columns, headerIndex, headerText);
        
        //填充内容
        CreateContentWithErrorMsg<T>(sheet,workbook,dt,fl,rowIndex);
        
        var fileName = $"temp{DateTime.Now:yyyyMMddhhmmss}.xlsx";
        var fs = new FileStream($"./{fileName}", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        workbook.Write(fs);

        return fs;
    }

    /// <summary>
    ///     根据DataTable的Column集合创建表头
    /// </summary>
    /// <param name="sheet">创建表头的表单</param>
    /// <param name="workbook">表单所属工作簿</param>
    /// <param name="cellStyle">单元格样式</param>
    /// <param name="arrColWidth">列宽集合</param>
    /// <param name="columns">列集合</param>
    /// <param name="headerIndex">表头索引,不需要表头时设置为-1</param>
    /// <param name="headerText">表头标题内容</param>
    /// <returns name="rowIndex">返回创建表头后的下一行</returns>
    public static int CreateHeader(ISheet sheet, IWorkbook workbook, int[] arrColWidth,
        DataColumnCollection columns, int headerIndex = -1, string headerText = "")
    {
        var rowIndex = headerIndex;
        //需要表头是合并第一行作为表头
        if (headerIndex >= 0)
        {
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, columns.Count - 1));
            var headerRow = sheet.CreateRow(0);
            headerRow.HeightInPoints = 25;
            //设置表头内容
            headerRow.CreateCell(0).SetCellValue(headerText);
            var headerStyle = workbook.CreateCellStyle();
            headerStyle.Alignment = HorizontalAlignment.Center;
            //设置字体
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 20;
            font.IsBold = true;
            headerStyle.SetFont(font);
            headerRow.GetCell(0).CellStyle = headerStyle;
        }

        rowIndex++;

        //创建列头
        var columnRow = sheet.CreateRow(rowIndex);
        var columnStyle = workbook.CreateCellStyle();
        columnStyle.Alignment = HorizontalAlignment.Center;
        var font1 = workbook.CreateFont();
        font1.FontHeightInPoints = 10;
        font1.IsBold = true;
        //写入列标题
        foreach (DataColumn column in columns)
        {
            columnRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
            columnRow.GetCell(column.Ordinal).CellStyle = columnStyle;
            // sheet.SetColumnWidth(column.Ordinal,((arrColWidth[column.Ordinal]+1)*256*2)>256?256:(arrColWidth[column.Ordinal]+1)*256*2);

            var tmp = (arrColWidth[column.Ordinal] + 1) * 256 * 2;
            var width = tmp > MAX_COL_WIDTH ? MAX_COL_WIDTH : tmp;
            sheet.SetColumnWidth(column.Ordinal, width);
        }

        rowIndex++;

        return rowIndex;
    }

    /// <summary>
    ///     填充Excel内容
    /// </summary>
    /// <param name="sheet">填充表单</param>
    /// <param name="workbook">填充工作簿</param>
    /// <param name="dt">DataTable</param>
    /// <param name="rowIndex">填充行号</param>
    /// <param name="dateFormat">日期时间格式化字符串</param>
    public static void CreateCellContent(ISheet sheet, IWorkbook workbook, DataTable dt,
        int rowIndex, string dateFormat = "yyyy-mm-dd hh:mm:ss")
    {
        foreach (DataRow row in dt.Rows)
        {
            var dataRow = sheet.CreateRow(rowIndex);

            foreach (DataColumn column in dt.Columns)
            {
                var newCell = dataRow.CreateCell(column.Ordinal);
                var drValue = row[column].ToString();
                switch (column.DataType.ToString())
                {
                    case PropertyTypeConstant.STRING:
                        double result;
                        if (IsNumeric(drValue, out result))
                        {
                            double.TryParse(drValue, out result);
                            newCell.SetCellValue(result);
                            break;
                        }

                        newCell.SetCellValue(drValue);
                        break;
                    case PropertyTypeConstant.DATETIME:
                        DateTime dateV;
                        DateTime.TryParse(drValue, out dateV);
                        newCell.SetCellValue(dateV);
                        newCell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat(dateFormat);
                        break;
                    case PropertyTypeConstant.BOOLEAN:
                        var boolV = false;
                        bool.TryParse(drValue, out boolV);
                        newCell.SetCellValue(boolV);
                        break;
                    case PropertyTypeConstant.INT16:
                    case PropertyTypeConstant.INT32:
                    case PropertyTypeConstant.INT64:
                    case PropertyTypeConstant.BYTE:
                        var intV = 0;
                        int.TryParse(drValue, out intV);
                        newCell.SetCellValue(intV);
                        break;
                    case PropertyTypeConstant.DECIMAL:
                    case PropertyTypeConstant.DOUBLE:
                        double doubV = 0;
                        double.TryParse(drValue, out doubV);
                        newCell.SetCellValue(doubV);
                        break;
                    case PropertyTypeConstant.DBNULL:
                        newCell.SetCellValue("");
                        break;
                    default:
                        newCell.SetCellValue(drValue);
                        break;
                }
            }
            rowIndex++;
        }
    }

    public static void CreateContentWithErrorMsg<T>(ISheet sheet, IWorkbook workbook, DataTable dt,
        List<List<ValidationFailure>> failureList,int rowIndex,string dateFormat="yyyy-mm-dd hh:mm:ss")
    {
        
        var mapping = GetPropertyMapping<T>();

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            var row = dt.Rows[i];
            var failureMsg = failureList[i];
            
            #region 填充每行具体信息
            var dataRow = sheet.CreateRow(rowIndex);
            foreach (DataColumn column in dt.Columns)
            {
                var newCell = dataRow.CreateCell(column.Ordinal);
                var drValue = row[column].ToString();
                switch (column.DataType.ToString())
                {
                    case PropertyTypeConstant.STRING:
                        double result;
                        if (IsNumeric(drValue, out result))
                        {
                            double.TryParse(drValue, out result);
                            newCell.SetCellValue(result);
                            break;
                        }

                        newCell.SetCellValue(drValue);
                        break;
                    case PropertyTypeConstant.DATETIME:
                        DateTime dateV;
                        DateTime.TryParse(drValue, out dateV);
                        newCell.SetCellValue(dateV);
                        newCell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat(dateFormat);
                        break;
                    case PropertyTypeConstant.BOOLEAN:
                        var boolV = false;
                        bool.TryParse(drValue, out boolV);
                        newCell.SetCellValue(boolV);
                        break;
                    case PropertyTypeConstant.INT16:
                    case PropertyTypeConstant.INT32:
                    case PropertyTypeConstant.INT64:
                    case PropertyTypeConstant.BYTE:
                        var intV = 0;
                        int.TryParse(drValue, out intV);
                        newCell.SetCellValue(intV);
                        break;
                    case PropertyTypeConstant.DECIMAL:
                    case PropertyTypeConstant.DOUBLE:
                        double doubV = 0;
                        double.TryParse(drValue, out doubV);
                        newCell.SetCellValue(doubV);
                        break;
                    case PropertyTypeConstant.DBNULL:
                        newCell.SetCellValue("");
                        break;
                    default:
                        newCell.SetCellValue(drValue);
                        break;
                }
            }
            #endregion

            #region 填充错误信息

            foreach (var failure in failureMsg)
            {
                var propertyName = failure.PropertyName;
                var msg = failure.ErrorMessage;
                var columnName = mapping.GetValueOrDefault(propertyName);
                if (string.IsNullOrEmpty(columnName)) continue;
                var cell = dataRow.GetCell(dt.Columns.IndexOf(columnName));


                //设置样式
                var cellStyle = workbook.CreateCellStyle();
                cellStyle.FillForegroundColor = HSSFColor.Red.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;
                cell.CellStyle = cellStyle;
                
                //设置批注
                var drawing = sheet.CreateDrawingPatriarch();
                var comment = drawing.CreateCellComment(new XSSFClientAnchor());
                comment.String = new XSSFRichTextString(msg);
                cell.CellComment = comment;
            }
            #endregion
            
            rowIndex++;
        }
    }


    /// <summary>
    ///     将实体列表转换成DataTable
    /// </summary>
    /// <param name="entityList"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>DataTable</returns>
    public static DataTable ListToDataTable<T>(List<T> entityList)
    {
        var propertyInfos = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(ExcelAttribute), false).Length > 0)
            .ToList();
        var table = new DataTable();
        //按照注解ColumIndex属性排序
        propertyInfos.Sort((p1, p2) =>
        {
            var index1 = p1.GetCustomAttribute<ExcelAttribute>().ColumnIndex;
            var index2 = p2.GetCustomAttribute<ExcelAttribute>().ColumnIndex;
            return index1 > index2 ? 1 : 0;
        });
        foreach (var propertyInfo in propertyInfos)
        {
            var attribute = propertyInfo.GetCustomAttribute<ExcelAttribute>();
            table.Columns.Add(attribute!.ExcelFieldName);
        }

        foreach (var entity in entityList)
        {
            var dataRow = table.NewRow();

            foreach (var propertyInfo in propertyInfos)
            {
                var attribute = propertyInfo.GetCustomAttribute<ExcelAttribute>();
                dataRow[attribute!.ExcelFieldName] = propertyInfo.GetValue(entity);
            }

            table.Rows.Add(dataRow);
        }

        return table;
    }

    /// <summary>
    /// 获取实体类字段名与Excel列名映射
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>Dictionary(字段名,列名)</returns>)
    public static Dictionary<string, string> GetPropertyMapping<T>()
    {
        var propertyInfos = typeof(T).GetProperties()
            .Where(p=>p.GetCustomAttributes(typeof(ExcelAttribute),false).Length>0)
            .ToList();
        var dic = new Dictionary<string,string>();
        foreach (var property in propertyInfos)
        {
            var attribute = (ExcelAttribute)property.GetCustomAttribute(typeof(ExcelAttribute))!;
            dic.Add(attribute.EntityFieldName,attribute.ExcelFieldName);
        }

        return dic;
    }


    /// <summary>
    ///     将指定sheet中的数据导出到DataTable
    /// </summary>
    /// <param name="sheet">需要导出的sheet</param>
    /// <param name="headerRowIndex">表头所在行号，-1表示没有表头</param>
    /// <param name="dir">excel列名与DataTable列名所对应的字典</param>
    /// <returns></returns>
    public static DataTable ExcelToDataTable(ISheet sheet, int headerRowIndex, Dictionary<string, string> dir)
    {
        var table = new DataTable();
        IRow headerRow;
        int cellCount;
        try
        {
            //没有表头或不需要表头则用excel列序号作为DataTable列名
            if (headerRowIndex < 0)
            {
                headerRow = sheet.GetRow(0);
                cellCount = headerRow.LastCellNum;
                for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                {
                    var column = new DataColumn(Convert.ToString(i));
                    table.Columns.Add(column);
                }
            }
            //有表头，使用表头作为DataTable的列名
            else
            {
                headerRow = sheet.GetRow(headerRowIndex);
                cellCount = headerRow.LastCellNum;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                    //excel某一列列名不存在，则以该列序号作为列名；如果DataTable包含重复列名，列名为重复列名+序号
                    if (headerRow.GetCell(i) == null)
                    {
                        if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                        {
                            var column = new DataColumn(Convert.ToString("重复列名" + i));
                            table.Columns.Add(column);
                        }
                        else
                        {
                            var column = new DataColumn(Convert.ToString(i));
                            table.Columns.Add(column);
                        }
                    }
                    //某一列列名不为空，但是重复，列名为“重复列名+序号”
                    else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString()) > 0)
                    {
                        var column = new DataColumn(Convert.ToString("重复列名" + i));
                        table.Columns.Add(column);
                    }
                    //列名不重复且存在
                    else
                    {
                        var str = headerRow.GetCell(i).ToString();
                        var colName = dir
                            .FirstOrDefault(s => s.Value == str,
                                new KeyValuePair<string, string>(str, str)).Key;
                        var column = new DataColumn(colName);
                        table.Columns.Add(column);
                    }
            }

            var rowCount = sheet.LastRowNum;
            for (var i = headerRowIndex + 1; i <= rowCount; i++)
                try
                {
                    IRow row;
                    //excel有空行，跳过读取下一行
                    if (sheet.GetRow(i) == null)
                        continue;
                    // row = sheet.CreateRow(i);
                    row = sheet.GetRow(i);

                    var dataRow = table.NewRow();
                    for (int j = row.FirstCellNum; j <= cellCount; j++) //excel列遍历
                        try
                        {
                            if (row.GetCell(j) != null)
                                switch (row.GetCell(j).CellType)
                                {
                                    case CellType.String: //字符串
                                        var str = row.GetCell(j).StringCellValue;
                                        if (!string.IsNullOrEmpty(str) && str.Length > 0)
                                            dataRow[j] = str;
                                        else
                                            dataRow[j] = default(string);
                                        break;
                                    case CellType.Numeric: //数字
                                        if (DateUtil.IsCellDateFormatted(row.GetCell(j))) //时间戳数字
                                            dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                        else
                                            dataRow[j] = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                                        break;
                                    case CellType.Boolean:
                                        dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                        break;
                                    case CellType.Error:
                                        dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                        break;
                                    case CellType.Formula:
                                        switch (row.GetCell(j).CachedFormulaResultType)
                                        {
                                            case CellType.String:
                                                var strFormula = row.GetCell(j).StringCellValue;
                                                if (!string.IsNullOrEmpty(strFormula) && strFormula.Length > 0)
                                                    dataRow[j] = strFormula;
                                                else
                                                    dataRow[j] = null;
                                                break;
                                            case CellType.Numeric:
                                                dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                                break;
                                            case CellType.Boolean:
                                                dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                                break;
                                            case CellType.Error:
                                                dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                                break;
                                            default:
                                                dataRow[j] = "";
                                                break;
                                        }

                                        break;
                                    default:
                                        dataRow[j] = "";
                                        break;
                                }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }

                    table.Rows.Add(dataRow);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return table;
    }


    /// <summary>
    ///     将DataTable转换成List
    /// </summary>
    /// <param name="dt">Datable</param>
    /// <typeparam name="TResult">转换结果泛型</typeparam>
    /// <returns></returns>
    public static List<TResult> DataTableToList<TResult>(this DataTable dt) where TResult : class, new()
    {
        //获取实体类中具有ExcelAttribute的注解属性
        var propertyInfos = typeof(TResult).GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(ExcelAttribute), false).Length > 0)
            .ToList();

        var result = new List<TResult>();

        foreach (DataRow row in dt.Rows)
        {
            var entity = new TResult();
            foreach (var propertyInfo in propertyInfos)
            {
                var attribute = propertyInfo.GetCustomAttribute<ExcelAttribute>();
                var value = row[attribute!.ExcelFieldName];
                if (value != DBNull.Value)
                    switch (propertyInfo.PropertyType.FullName)
                    {
                        case PropertyTypeConstant.STRING:
                            propertyInfo.SetValue(entity, value);
                            break;
                        case PropertyTypeConstant.INT16:
                            var int16 = Convert.ToInt16(value);
                            propertyInfo.SetValue(entity, int16);
                            break;
                        case PropertyTypeConstant.INT32:
                            var int32 = Convert.ToInt32(value);
                            propertyInfo.SetValue(entity, int32);
                            break;
                        case PropertyTypeConstant.INT64:
                            var int64 = Convert.ToInt64(value);
                            propertyInfo.SetValue(entity, int64);
                            break;
                        case PropertyTypeConstant.BOOLEAN:
                            var boolean = Convert.ToBoolean(value);
                            propertyInfo.SetValue(entity, boolean);
                            break;
                        case PropertyTypeConstant.DECIMAL:
                        case PropertyTypeConstant.DOUBLE:
                            var v = Convert.ToDecimal(value);
                            propertyInfo.SetValue(entity, v);
                            break;
                        case PropertyTypeConstant.DATETIME:
                            var dateTime = Convert.ToDateTime(value);
                            propertyInfo.SetValue(entity, dateTime);
                            break;
                        default:
                            if (propertyInfo.PropertyType.BaseType == typeof(Enum))
                            {
                                var val = Convert.ToInt32(value);
                                propertyInfo.SetValue(entity, val);
                            }

                            break;
                    }
            }

            result.Add(entity);
        }

        return result;
    }


    /// <summary>
    ///     将Excel中数据导入成实体类
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="sheetName">sheet名称</param>
    /// <param name="headerRowIndex">表头索引</param>
    /// <typeparam name="T">导入实体类型</typeparam>
    /// <returns></returns>
    public static List<T> ImportExcelToEntityList<T>(string filePath,
        string sheetName = "Sheet1",
        int headerRowIndex = 0) where T : class, new()
    {
        var table = new DataTable();
        using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            if (file.Length > 0)
            {
                var wb = WorkbookFactory.Create(file);
                var sheet = wb.GetSheet(sheetName);
                table = ExcelToDataTable(sheet, headerRowIndex, new Dictionary<string, string>());
            }
        }

        var result = table.DataTableToList<T>();
        table.Dispose();
        return result;
    }

    /// <summary>
    ///     从文件流中读取实体列表
    /// </summary>
    /// <param name="fileStream">文件流</param>
    /// <param name="sheetName">sheet名称</param>
    /// <param name="headerRowIndex">表头行号索引</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>实体类列表</returns>
    public static List<T> ImportExcelToEntityList<T>(Stream fileStream,
        string sheetName = "Sheet1",
        int headerRowIndex = 0) where T : class, new()
    {
        var table = new DataTable();

        if (fileStream.Length > 0)
        {
            var wb = WorkbookFactory.Create(fileStream);
            var sheet = wb.GetSheet(sheetName);
            table = ExcelToDataTable(sheet, headerRowIndex, new Dictionary<string, string>());
        }

        var result = table.DataTableToList<T>();
        table.Dispose();
        return result;
    }


    /// <summary>
    ///     判断内容是否是数字
    /// </summary>
    /// <param name="message"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool IsNumeric(string message, out double result)
    {
        var rex = new Regex(@"^[-]?\d+[.]?\d*$");
        result = -1;
        if (rex.IsMatch(message))
        {
            result = double.Parse(message);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     返回每一列列宽集合
    /// </summary>
    /// <param name="dataTable">数据源</param>
    /// <returns></returns>
    private static int[] GetContentLength(DataTable dataTable)
    {
        var arrColWidth = new int[dataTable.Columns.Count];
        foreach (DataColumn item in dataTable.Columns)
        {
            var length = Encoding.UTF8.GetBytes(item.ColumnName).Length;
            arrColWidth[item.Ordinal] = length > 255 ? 255 : length;
        }

        foreach (DataRow row in dataTable.Rows)
            for (var j = 0; j < dataTable.Columns.Count; j++)
            {
                var tmp = Encoding.UTF8.GetBytes(row[j].ToString() ?? string.Empty).Length;
                var length = tmp > 255 ? 255 : tmp;
                arrColWidth[j] = int.Max(arrColWidth[j], length);
            }

        return arrColWidth;
    }
}