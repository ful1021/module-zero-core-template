using AbpCompanyName.AbpProjectName.Enumerations;

namespace AbpCompanyName.AbpProjectName.CustomColumns
{
    [EnumData]
    public enum CustomColumnValueType
    {
        不限 = 0,
        单行字符串 = 1,
        多行字符串 = 2,
        数字 = 3,
        日期 = 4
    }
}