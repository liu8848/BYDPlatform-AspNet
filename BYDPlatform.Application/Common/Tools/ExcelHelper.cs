using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BYDPlatform.Domain.Attributes;
using BYDPlatform.Domain.Constant;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace BYDPlatform.Application.Common.Tools;

public static class ExcelHelper
{


    public static MemoryStream ExportDataTable(string fileName, DataTable dt,
        string headerRowText="",string sheetName="Sheet1")
    {
        //创建工作簿和sheet
        IWorkbook workbook = new HSSFWorkbook();
        using (Stream writeFile = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
        {
            if (writeFile.Length > 0 && string.IsNullOrEmpty(sheetName))
            {
                workbook=WorkbookFactory.Create(writeFile);
            }
        }

        ISheet sheet = null;
        ICellStyle dateStyle = workbook.CreateCellStyle();
        IDataFormat format = workbook.CreateDataFormat();
        dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
        int[] arrColWidth = new int[dt.Columns.Count];
        foreach (DataColumn item in dt.Columns)
        {
            arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(Convert.ToString(item.ColumnName)).Length;
        }
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                int intTemp = Encoding.GetEncoding(936).GetBytes(Convert.ToString(dt.Rows[i][j]) ?? string.Empty).Length;
                if (intTemp > arrColWidth[j])
                {
                    arrColWidth[j] = intTemp;
                }
            }
        }

        int rowIndex = 0;
        foreach (DataRow row in dt.Rows)
        {
            #region 新建表，填充表头，填充列头，样式
            if (rowIndex == 0)
            {
                if (workbook.GetSheetIndex(sheetName) >= 0)
                {
                    workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName));
                }

                sheet = workbook.CreateSheet(sheetName);

                #region 表头及样式
                {
                    //合并第一行表头
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dt.Columns.Count - 1));
                    var headerRow = sheet.CreateRow(0);
                    headerRow.HeightInPoints = 25;
                    //设置表头内容
                    headerRow.CreateCell(0).SetCellValue(headerRowText);
                    var headerStyle = workbook.CreateCellStyle();
                    headerStyle.Alignment = HorizontalAlignment.Center;
                    IFont font = workbook.CreateFont();
                    font.FontHeightInPoints = 20;
                    font.Boldweight = 700;
                    headerStyle.SetFont(font);
                    headerRow.GetCell(0).CellStyle = headerStyle;
                    rowIndex = 1;
                }
                #endregion

                #region 列头及样式

                if (rowIndex == 1)
                {
                    IRow headerRow = sheet.CreateRow(1);//第二行设置列名
                    ICellStyle headStyle = workbook.CreateCellStyle();
                    headStyle.Alignment = HorizontalAlignment.Center;
                    IFont font = workbook.CreateFont();
                    font.FontHeightInPoints = 10;
                    font.Boldweight = 700;
                    headStyle.SetFont(font);
                    //写入列标题
                    foreach (DataColumn column in dt.Columns)
                    {
                        headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                        headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                        //设置列宽
                        sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256 * 2);
                    }
                    rowIndex = 2;
                }

                #endregion
            }
            #endregion

            #region 填充内容

            IRow dataRow = sheet.CreateRow(rowIndex);

            foreach (DataColumn column in dt.Columns)
            {
                var newCell = dataRow.CreateCell(column.Ordinal);
                var drValue = row[column].ToString();
                                    switch (column.DataType.ToString())
                    {
                        case "System.String": //字符串类型
                            double result;
                            if (IsNumeric(drValue, out result))
                            {
                                //数字字符串
                                double.TryParse(drValue, out result);
                                newCell.SetCellValue(result);
                                break;
                            }
                            else
                            {
                                newCell.SetCellValue(drValue);
                                break;
                            }
                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                            newCell.CellStyle = dateStyle; //格式化显示
                            break;
                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue(drValue.ToString());
                            break;
                    }
            }
            #endregion
        }
        using (MemoryStream ms = new MemoryStream())
        {
            workbook.Write(ms, true);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }
    }


    public static void ExportDataTable(DataTable dt, FileStream fs,
        string sheetName, string headerText)
    {
        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = null;
        ICellStyle dateStyle = workbook.CreateCellStyle();
        IDataFormat format = workbook.CreateDataFormat();
        dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
        //取得列宽
        int[] arrColWidth = new int[dt.Columns.Count];
        foreach (DataColumn item in dt.Columns)
        {
            arrColWidth[item.Ordinal] = Encoding.GetEncoding(0).GetBytes(Convert.ToString(item.ColumnName)).Length;
        }

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                int intTemp = Encoding.GetEncoding(0).GetBytes(Convert.ToString(dt.Rows[i][j]) ?? string.Empty)
                    .Length;
                if (intTemp > arrColWidth[j])
                {
                    arrColWidth[j] = intTemp;
                }
            }
        }

        int rowIndex = 0;
        foreach (DataRow row in dt.Rows)
        {
            #region 新建表，填充表头，填充列头，样式

            if (rowIndex == 0)
            {
                if (workbook.GetSheetIndex(sheetName) >= 0)
                {
                    workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName));
                }

                sheet = workbook.CreateSheet(sheetName);

                #region 表头及样式

                {
                    //合并第一行表头
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dt.Columns.Count - 1));
                    var headerRow = sheet.CreateRow(0);
                    headerRow.HeightInPoints = 25;
                    //设置表头内容
                    headerRow.CreateCell(0).SetCellValue(headerText);
                    var headerStyle = workbook.CreateCellStyle();
                    headerStyle.Alignment = HorizontalAlignment.Center;
                    IFont font = workbook.CreateFont();
                    font.FontHeightInPoints = 20;
                    font.Boldweight = 700;
                    headerStyle.SetFont(font);
                    headerRow.GetCell(0).CellStyle = headerStyle;
                    rowIndex = 1;
                }

                #endregion

                #region 列头及样式

                if (rowIndex == 1)
                {
                    IRow headerRow = sheet.CreateRow(1); //第二行设置列名
                    ICellStyle headStyle = workbook.CreateCellStyle();
                    headStyle.Alignment = HorizontalAlignment.Center;
                    IFont font = workbook.CreateFont();
                    font.FontHeightInPoints = 10;
                    font.Boldweight = 700;
                    headStyle.SetFont(font);
                    //写入列标题
                    foreach (DataColumn column in dt.Columns)
                    {
                        headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                        headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                        //设置列宽
                        sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256 * 2);
                    }

                    rowIndex = 2;
                }

                #endregion
            }

            #endregion

            #region 填充内容

            IRow dataRow = sheet.CreateRow(rowIndex);

            foreach (DataColumn column in dt.Columns)
            {
                var newCell = dataRow.CreateCell(column.Ordinal);
                var drValue = row[column].ToString();
                switch (column.DataType.ToString())
                {
                    case "System.String": //字符串类型
                        double result;
                        if (IsNumeric(drValue, out result))
                        {
                            //数字字符串
                            double.TryParse(drValue, out result);
                            newCell.SetCellValue(result);
                            break;
                        }
                        else
                        {
                            newCell.SetCellValue(drValue);
                            break;
                        }
                    case "System.DateTime": //日期类型
                        DateTime dateV;
                        DateTime.TryParse(drValue, out dateV);
                        newCell.SetCellValue(dateV);
                        newCell.CellStyle = dateStyle; //格式化显示
                        break;
                    case "System.Boolean": //布尔型
                        bool boolV = false;
                        bool.TryParse(drValue, out boolV);
                        newCell.SetCellValue(boolV);
                        break;
                    case "System.Int16": //整型
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Byte":
                        int intV = 0;
                        int.TryParse(drValue, out intV);
                        newCell.SetCellValue(intV);
                        break;
                    case "System.Decimal": //浮点型
                    case "System.Double":
                        double doubV = 0;
                        double.TryParse(drValue, out doubV);
                        newCell.SetCellValue(doubV);
                        break;
                    case "System.DBNull": //空值处理
                        newCell.SetCellValue("");
                        break;
                    default:
                        newCell.SetCellValue(drValue.ToString());
                        break;
                }
            }
            #endregion

            rowIndex++;
        }
        workbook.Write(fs,true);
        fs.Close();
    }



    /// <summary>
    /// 将实体列表转换成DataTable
    /// </summary>
    /// <param name="entityList"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>DataTable</returns>
    public static DataTable ListToDataTable<T>(List<T> entityList)
    {
        var propertyInfos = typeof(T).GetProperties()
            .Where(p=>p.GetCustomAttributes(typeof(ExcelAttribute),false).Length>0)
            .ToList();
        var table = new DataTable();
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
    /// 将指定sheet中的数据导出到DataTable
    /// </summary>
    /// <param name="sheet">需要导出的sheet</param>
    /// <param name="headerRowIndex">表头所在行号，-1表示没有表头</param>
    /// <param name="dir">excel列名与DataTable列名所对应的字典</param>
    /// <returns></returns>
    public static DataTable ExcelToDataTable(ISheet sheet, int headerRowIndex, Dictionary<string, string> dir)
    {
        DataTable table = new DataTable();
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
                    DataColumn column = new DataColumn(Convert.ToString(i));
                    table.Columns.Add(column);
                }
            }
            //有表头，使用表头作为DataTable的列名
            else
            {
                headerRow = sheet.GetRow(headerRowIndex);
                cellCount = headerRow.LastCellNum;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    //excel某一列列名不存在，则以该列序号作为列名；如果DataTable包含重复列名，列名为重复列名+序号
                    if (headerRow.GetCell(i) == null)
                    {
                        if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                        {
                            DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                            table.Columns.Add(column);
                        }
                        else
                        {
                            DataColumn column = new DataColumn(Convert.ToString(i));
                            table.Columns.Add(column);
                        }
                    }
                    //某一列列名不为空，但是重复，列名为“重复列名+序号”
                    else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString())>0)
                    {
                        var column = new DataColumn(Convert.ToString("重复列名"+i));
                        table.Columns.Add(column);
                    }
                    //列名不重复且存在
                    else
                    {
                        var str = headerRow.GetCell(i).ToString();
                        var colName = dir
                            .FirstOrDefault(s => s.Value==str,
                                new KeyValuePair<string, string>(str,str)).Key;
                        var column = new DataColumn(colName);
                        table.Columns.Add(column);
                    }
                }
            }

            int rowCount = sheet.LastRowNum;
            for (int i = (headerRowIndex+1); i <=rowCount; i++)
            {
                try
                {
                    IRow row;
                    //excel有空行，跳过读取下一行
                    if (sheet.GetRow(i) == null)
                    {
                        continue;
                        // row = sheet.CreateRow(i);
                    }
                    else
                    {
                        row = sheet.GetRow(i);
                    }

                    DataRow dataRow = table.NewRow();
                    for (int j = row.FirstCellNum; j <= cellCount; j++)//excel列遍历
                    {
                        try
                        {
                            if (row.GetCell(j) != null)
                            {
                                switch (row.GetCell(j).CellType)
                                {
                                    case CellType.String://字符串
                                        var str = row.GetCell(j).StringCellValue;
                                        if (!string.IsNullOrEmpty(str) && str.Length > 0)
                                        {
                                            dataRow[j] = str;
                                        }
                                        else
                                        {
                                            dataRow[j] = default(string);
                                        }
                                        break;
                                    case CellType.Numeric://数字
                                        if (DateUtil.IsCellDateFormatted(row.GetCell(j)))//时间戳数字
                                        {
                                            dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                        }
                                        else
                                        {
                                            dataRow[j] = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                                        }
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
                                                {
                                                    dataRow[j] = strFormula;
                                                }
                                                else
                                                {
                                                    dataRow[j] = null;
                                                }
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
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }

                    table.Rows.Add(dataRow);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
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
    /// 将DataTable转换成List
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
                {
                    switch (propertyInfo.PropertyType.FullName)
                    {
                        case PropertyTypeConstant.STRING:
                            propertyInfo.SetValue(entity,value);
                            break;
                        case PropertyTypeConstant.INT16:
                            var int16 = Convert.ToInt16(value);
                            propertyInfo.SetValue(entity,int16);
                            break;
                        case PropertyTypeConstant.INT32:
                            var int32 = Convert.ToInt32(value);
                            propertyInfo.SetValue(entity,int32);
                            break;
                        case PropertyTypeConstant.INT64:
                            var int64 = Convert.ToInt64(value);
                            propertyInfo.SetValue(entity,int64);
                            break;
                        case PropertyTypeConstant.BOOLEAN:
                            var boolean = Convert.ToBoolean(value);
                            propertyInfo.SetValue(entity,boolean);
                            break;
                        case PropertyTypeConstant.DECIMAL:
                        case PropertyTypeConstant.DOUBLE:
                            var v = Convert.ToDecimal(value);
                            propertyInfo.SetValue(entity,v);
                            break;
                        case PropertyTypeConstant.DATETIME:
                            var dateTime = Convert.ToDateTime(value);
                            propertyInfo.SetValue(entity,dateTime);
                            break;
                        default:
                            if (propertyInfo.PropertyType.BaseType == typeof(Enum))
                            {
                                var val = Convert.ToInt32(value);
                                propertyInfo.SetValue(entity,val);
                            }
                            break;
                    }
                }
            }
            result.Add(entity);
        }
        return result;
    }
    

    /// <summary>
    /// 将Excel中数据导入成实体类
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="sheetName">sheet名称</param>
    /// <param name="headerRowIndex">表头索引</param>
    /// <typeparam name="T">导入实体类型</typeparam>
    /// <returns></returns>
    public static List<T> ImportExcelToEntityList<T>(string filePath, 
        string sheetName="Sheet1", 
        int headerRowIndex = 0) where T:class,new()
    {
        var table = new DataTable();
        using (FileStream file=new FileStream(filePath,FileMode.Open,FileAccess.Read))
        {
            if (file.Length > 0)
            {
                IWorkbook wb = WorkbookFactory.Create(file);
                ISheet sheet = wb.GetSheet(sheetName);
                table = ExcelToDataTable(sheet, headerRowIndex, new Dictionary<string, string>());
            }
        }
        List<T> result = table.DataTableToList<T>();
        table.Dispose();
        return result;
    }

    /// <summary>
    /// 从文件流中读取实体列表
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
                IWorkbook wb = WorkbookFactory.Create(fileStream);
                ISheet sheet = wb.GetSheet(sheetName);
                table = ExcelToDataTable(sheet, headerRowIndex, new Dictionary<string, string>());
            }

        List<T> result = table.DataTableToList<T>();
        table.Dispose();
        return result;
    }
    
    
    /// <summary>
    /// 判断内容是否是数字
    /// </summary>
    /// <param name="message"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool IsNumeric(String message, out double result)
    {
        Regex rex = new Regex(@"^[-]?\d+[.]?\d*$");
        result = -1;
        if (rex.IsMatch(message))
        {
            result = double.Parse(message);
            return true;
        }
        else
            return false;
    }
}

