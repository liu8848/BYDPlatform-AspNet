namespace BYDPlatform.Domain.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ExcelAttribute : Attribute
{
    public string EntityFieldName { get; set; } = "";

    public string ExcelFieldName { get; set; } = "";

    public int ColumnIndex { get; set; } = 1;
}