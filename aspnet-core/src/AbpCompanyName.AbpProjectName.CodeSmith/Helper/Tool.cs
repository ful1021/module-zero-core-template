using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeSmith.Engine;

namespace AbpCompanyName.AbpProjectName.Helper
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Tool
    {
        #region 字符串扩展方法

        /// <summary>
        /// 转换为CamelCase命名法,第一个单词首字母小写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToCamel(string value) => StringUtil.ToCamelCase(value);

        /// <summary>
        /// 转换为PascalCase命名法,第一个单词首字母大写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPascal(string value) => StringUtil.ToPascalCase(value);

        /// <summary>
        /// 转换为单数形式:如Users 生成 User
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSingular(string value) => StringUtil.ToSingular(value);

        /// <summary>
        /// 转换为复数形式:如User 生成 Users
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPlural(string value) => StringUtil.ToPlural(value);

        /// <summary>
        /// 转换为CamelCase命名法,第一个单词首字母小写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToFirstLetterCamel(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            return value[0].ToString().ToLower() + value.Substring(1);
        }

        #endregion 字符串扩展方法

        #region Helper方法

        /// <summary>
        /// 判断item是否存在list中
        /// </summary>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsIn(string item, params string[] list)
        {
            return list.Any(a => item.Equals(a, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 对象 转换为DateTime类型或者null，不成功返回null
        /// </summary>
        public static DateTime? TryToDateTimeOrNull(object str)
        {
            DateTime? result = null;
            if (str != null && !string.IsNullOrWhiteSpace(str.ToString()))
            {
                DateTime tmp;
                if (DateTime.TryParse(str.ToString(), out tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 对象 转换为DateTime类型或者null，不成功返回null
        /// </summary>
        public static DateTime TryToDateTime(object str, string defaultResult = null)
        {
            var result = TryToDateTimeOrNull(str);
            if (result == null && !string.IsNullOrWhiteSpace(defaultResult))
            {
                DateTime tmp;
                if (DateTime.TryParse(defaultResult, out tmp))
                {
                    result = tmp;
                }
            }
            return result ?? DateTime.MinValue;
        }

        /// <summary>
        /// 字符串转换为 bool 类型
        /// </summary>
        public static bool TryToBool(object str, bool defaultResult = false)
        {
            var result = defaultResult;
            if (str != null && !string.IsNullOrWhiteSpace(str.ToString()))
            {
                if (bool.TryParse(str.ToString(), out bool tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 对象 转换为 decimal 类型
        /// </summary>
        public static decimal TryToDecimal(object str, decimal defaultResult = 0)
        {
            var result = defaultResult;
            if (str != null && !string.IsNullOrWhiteSpace(str.ToString()))
            {
                decimal tmp;
                if (decimal.TryParse(str.ToString(), out tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 对象 转换为 int 类型
        /// </summary>
        public static int TryToInt(object str, int defaultResult = 0)
        {
            var result = defaultResult;
            if (str != null && !string.IsNullOrWhiteSpace(str.ToString()))
            {
                if (int.TryParse(str.ToString(), out int tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 对象 转换为 string 类型
        /// </summary>
        public static string TryToString(object source)
        {
            if (source != null)
            {
                return source.ToString();
            }
            return "";
        }

        #endregion Helper方法

        #region GetSummary

        /// <summary>
        /// 获取字段注释
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public static Dictionary<PropertyInfo, string> GetPropertySummaries(PropertyInfo[] props)
        {
            Dictionary<PropertyInfo, string> dict = new Dictionary<PropertyInfo, string>();
            foreach (var item in props)
            {
                dict[item] = GetPropertySummary(item);
            }
            return dict;
        }

        /// <summary>
        /// 得到反射字段 注释
        /// </summary>
        /// <returns></returns>
        public static string GetPropertySummary(PropertyInfo item)
        {
            string name = item.Name;
            var textInfo = CSCommentReader.Create(item);
            var text = textInfo != null ? textInfo.Summary : "";
            if (name == "CreationTime")
            {
                text = "创建时间";
            }
            else if (name == "CreatorUserId")
            {
                text = "创建人";
            }
            else if (name == "LastModificationTime")
            {
                text = "上次修改时间";
            }
            else if (name == "LastModifierUserId")
            {
                text = "上次修改人";
            }

            return text;
        }

        /// <summary>
        /// 获取类注释
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetClassSummary(Type type)
        {
            if (type != null)
            {
                var info = CSCommentReader.Create(type);
                if (info != null)
                {
                    return info.Summary.Replace("\r", " ").Replace("\n", " ");
                }
            }
            return "";
        }

        #endregion GetSummary

        #region Assembly

        /// <summary>
        /// 根据dll文件，获取得到 Assembly
        /// </summary>
        /// <param name="dllFile"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string dllFile)
        {
            //byte[] filedata = System.IO.File.ReadAllBytes(dllFile);
            //Assembly assembly = Assembly.Load(filedata);
            //LoadFrom 会使文件 占用不释放
            Assembly assembly = Assembly.LoadFrom(dllFile);
            return assembly;
        }

        private static SortedList<string, Type> _typeList = new SortedList<string, Type>();

        /// <summary>
        /// 获取反射的Assembly 类型
        /// </summary>
        /// <param name="dllFile"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Type GetAssemblyType(string dllFile, string className)
        {
            string key = className;

            Type type = GetTypeFromCache(key);
            if (type == null)
            {
                Assembly assembly = GetAssembly(dllFile);
                var types = assembly.GetTypes();
                type = types.FirstOrDefault(a => a.Name == className);
                //添加到缓冲中
                _typeList.Add(key, type);
            }

            return type;
        }

        private static Type GetTypeFromCache(string key)
        {
            if (_typeList.TryGetValue(key, out Type result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        #endregion Assembly

        #region 列

        /// <summary>
        /// 反射获取PropertyInfo集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type type)
        {
            if (type == null)
            {
                return new PropertyInfo[0];
            }
            PropertyInfo[] propertyinfo = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return propertyinfo.OrderBy(a => a.Name).ToArray();
        }

        /// <summary>
        /// 反射获取PropertyInfo集合
        /// </summary>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(string dllFile, string className)
        {
            var type = GetAssemblyType(dllFile, className);
            return GetProperties(type);
        }

        /// <summary>
        /// 得到反射字段 类型
        /// </summary>
        /// <returns></returns>
        public static string GetPropertyType(Type type, string propertyName = "Id")
        {
            var props = GetProperties(type);
            var propInfo = props.FirstOrDefault(a => a.Name == propertyName);
            if (propInfo != null)
            {
                return GetCSharpType(propInfo);
            }
            return "";
        }

        /// <summary>
        /// 所有枚举列
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetEnumProperties(PropertyInfo[] props)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (var col in props)
            {
                if (col.PropertyType.IsEnum)
                {
                    list.Add(col);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 所有时间列
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetDateTimeProperties(PropertyInfo[] props)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (var col in props)
            {
                if (GetCSharpNullableType(col).Contains("DateTime"))
                {
                    list.Add(col);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 判断当前类，是否存在字段name
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="name">字段字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <param name="type">字段在C#中的类型字符串</param>
        /// <returns></returns>
        public static bool IsExistsProp(PropertyInfo[] entityColumns, string name, bool ignoreCase = false, string type = null)
        {
            var b = false;
            foreach (var col in entityColumns)
            {
                if (ignoreCase)
                {
                    if (col.Name.Trim().ToLower() == name.Trim().ToLower())
                    {
                        b = true;
                    }
                }
                else
                {
                    if (col.Name.Trim() == name.Trim())
                    {
                        b = true;
                    }
                }
                if (b && !string.IsNullOrWhiteSpace(type))
                {
                    b = GetCSharpType(col).Trim().Equals(type.Trim(), StringComparison.OrdinalIgnoreCase);
                }
                if (b)
                {
                    break;
                }
            }
            return b;
        }

        #endregion 列

        #region 类型处理

        /// <summary>
        /// 根据列得到转换为C#后，可空类型字符串 如：int?
        /// </summary>
        /// <param name="structType"></param>
        /// <returns></returns>
        public static string GetCSharpNullableType(PropertyInfo prop)
        {
            var propTypeFullName = prop.PropertyType.FullName;
            var type = GetCSharpTypeByString(propTypeFullName);
            if (type.Contains("Nullable"))
            {
                if (propTypeFullName.Contains("Int32"))
                {
                    return "int?";
                }
                else if (propTypeFullName.Contains("Int64"))
                {
                    return "long?";
                }
                else if (propTypeFullName.Contains("DateTime"))
                {
                    return "DateTime?";
                }
                else if (propTypeFullName.Contains("Boolean"))
                {
                    return "bool?";
                }
                else if (propTypeFullName.Contains("Guid"))
                {
                    return "Guid?";
                }
                else if (prop.PropertyType.IsValueType)
                {
                }
            }
            return type;
        }

        /// <summary>
        /// 根据列得到转换为C#后类型字符串
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static string GetCSharpType(PropertyInfo prop)
        {
            var type = GetCSharpTypeByString(prop.PropertyType.FullName);
            return type.Replace(prop.Name, "");
        }

        public static bool IsList(PropertyInfo prop)
        {
            var fullType = prop.PropertyType.FullName;
            return fullType.Contains("System.Collections.Generic.ICollection") || fullType.Contains("System.Collections.Generic.List");
        }

        /// <summary>
        /// 判断指定 Property 是否为 DateTime 类型
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsDataTime(PropertyInfo prop)
        {
            return GetCSharpType(prop).Contains("DateTime");
        }

        /// <summary>
        /// 判断指定 Property 是否为 int 类型
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsString(PropertyInfo prop)
        {
            return GetCSharpType(prop).Contains("string");
        }

        /// <summary>
        /// 判断指定 Property 是否为 int 类型
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsInt(PropertyInfo prop)
        {
            return GetCSharpType(prop).Contains("int");
        }

        /// <summary>
        /// 判断指定 Property 是否为 long 类型
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsLong(PropertyInfo prop)
        {
            return GetCSharpType(prop).Contains("long");
        }

        /// <summary>
        /// 判断指定 Property 是否为 数字
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsNumber(PropertyInfo prop)
        {
            return TryToInt(DefaultValue(prop), -1) == 0;
        }

        /// <summary>
        /// 根据列得到转换为C#后类型字符串
        /// </summary>
        /// <param name="structType"></param>
        /// <returns></returns>
        public static string GetCSharpTypeByString(string typeName)
        {
            switch (typeName)
            {
                case "System.AnsiString":
                case "System.AnsiStringFixedLength": return "string";
                case "System.Binary":
                case "System.Byte[]": return "byte[]";
                case "System.Boolean": return "bool";
                case "System.Byte": return "byte";
                case "System.Currency": return "decimal";
                case "System.Date":
                case "System.DateTime":
                case "System.DateTime2":
                case "System.DateTimeOffset": return "DateTime";
                case "System.Decimal": return "decimal";
                case "System.Double": return "double";
                case "System.Guid": return "Guid";
                case "System.Int16": return "short";
                case "System.Int32": return "int";
                case "System.Int64": return "long";
                case "System.Object": return "object";
                case "System.SByte": return "sbyte";
                case "System.Single": return "float";
                case "System.String":
                case "System.StringFixedLength": return "string";
                case "System.Time": return "TimeSpan";
                case "System.UInt16": return "ushort";
                case "System.UInt32": return "uint";
                case "System.UInt64": return "ulong";
                default:
                    return typeName;
            }
        }

        public static string GetCSharpAppServiceInputType(PropertyInfo prop)
        {
            var propTypeFullName = prop.PropertyType.FullName;
            var type = GetCSharpNullableType(prop);
            if (type != "string" && !IsAbpValueObject(prop) && !type.Contains("[]"))
            {
                if (!type.EndsWith("?"))
                {
                    return type + "?";
                }
            }
            return type;
        }

        public static object DefaultValue(PropertyInfo prop)
        {
            //var result = default(Type);
            var result = prop.PropertyType.IsValueType ? Activator.CreateInstance(prop.PropertyType) : null;
            return result;
        }

        #endregion 类型处理

        #region Abp扩展方法

        public const string CreatorUserId = "CreatorUserId";
        public const string CreationTime = "CreationTime";
        public const string LastModificationTime = "LastModificationTime";
        public const string LastModifierUserId = "LastModifierUserId";
        public const string DeleterUserId = "DeleterUserId";
        public const string DeletionTime = "DeletionTime";
        public const string IsDeleted = "IsDeleted";

        public static bool HasCreationTime(PropertyInfo[] props)
        {
            return IsExistsProp(props, CreationTime, true);
        }

        public static bool IsCreationAudited(PropertyInfo[] props)
        {
            return HasCreationTime(props) && IsExistsProp(props, CreatorUserId, true);
        }

        public static bool HasModificationTime(PropertyInfo[] props)
        {
            return IsExistsProp(props, LastModificationTime, true);
        }

        public static bool IsModificationAudited(PropertyInfo[] props)
        {
            return HasModificationTime(props) && IsExistsProp(props, LastModifierUserId, true);
        }

        public static bool IsAudited(PropertyInfo[] props)
        {
            return IsModificationAudited(props) && IsCreationAudited(props);
        }

        public static bool IsAbpValueObject(PropertyInfo prop)
        {
            var fullType = prop.PropertyType.BaseType;
            if (fullType != null)
            {
                if (!string.IsNullOrWhiteSpace(fullType.FullName))
                {
                    return fullType.FullName.Contains("Abp.Domain.Values.ValueObject");
                }
            }
            return false;
        }

        public static bool IsInCreationAudited(PropertyInfo prop)
        {
            return IsIn(prop.Name, CreatorUserId, CreationTime);
        }

        /// <summary>
        /// 判断指定字段是否属于 AbpAudited 列
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsInAudited(PropertyInfo prop)
        {
            return IsIn(prop.Name, CreatorUserId, CreationTime, LastModifierUserId, LastModificationTime);
        }

        public static bool IsInFullAudited(PropertyInfo prop)
        {
            return IsIn(prop.Name, CreatorUserId, CreationTime, LastModifierUserId, LastModificationTime, IsDeleted, DeletionTime, DeleterUserId);
        }

        #endregion Abp扩展方法
    }
}