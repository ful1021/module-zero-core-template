using System.Reflection;

namespace AbpCompanyName.AbpProjectName.Helper
{
    public class ClassNames
    {
        /// <summary>
        /// 主键名称
        /// </summary>
        public string PkName { get; set; }

        /// <summary>
        /// 主键类型
        /// </summary>
        public string PkType { get; set; }

        /// <summary>
        /// AppService 名称
        /// </summary>
        public string AppServiceName { get; set; }

        /// <summary>
        /// 管理后端 AppService 名称
        /// </summary>
        public string MgmtAppServiceName { get; set; }

        /// <summary>
        /// 父类 AppService 名称
        /// </summary>
        public string BaseAppServiceName { get; set; }

        /// <summary>
        /// 仓储名称
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// GetAllInput 类名
        /// </summary>
        public string GetAllInputName { get; set; }

        /// <summary>
        /// Dto 类名
        /// </summary>
        public string DtoName { get; set; }

        /// <summary>
        /// QueryDto 类名
        /// </summary>
        public string QueryDtoName { get; set; }

        /// <summary>
        /// CreateOrUpdateInput 类名
        /// </summary>
        public string CreateOrUpdateInputName { get; set; }

        /// <summary>
        /// CreateInput 类名
        /// </summary>
        public string CreateInputName { get; set; }

        /// <summary>
        /// UpdateInput 类名
        /// </summary>
        public string UpdateInputName { get; set; }

        /// <summary>
        /// Application Dll 文件完整路径名 D:\\code\Boss.Scm.Application.dll
        /// </summary>
        public string ApplicationDllFile { get; set; }

        /// <summary>
        /// Core Dll 文件完整路径名 D:\\code\Boss.Scm.Core.dll
        /// </summary>
        public string CoreDllFile { get; set; }

        /// <summary>
        /// 项目公司名称 Boss
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 项目模板名称 Scm
        /// </summary>
        public string ProjectTemplateName { get; set; }

        /// <summary>
        /// Application 程序集名称，Boss.Scm.Application
        /// </summary>
        public string ApplicationAssemblyName { get; set; }

        /// <summary>
        /// Core 程序集名称，Boss.Scm.Core
        /// </summary>
        public string CoreAssemblyName { get; set; }

        /// <summary>
        /// 实体类 名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 实体 Consts 名称
        /// </summary>
        public string EntityConstsName { get; set; }

        /// <summary>
        /// 实体类 注释
        /// </summary>
        public string EntitySummary { get; set; }

        /// <summary>
        /// 实体类 命名空间： Boss.Scm.Orders
        /// </summary>
        public string EntityNamespace { get; set; }

        /// <summary>
        /// EntityFrameworkCore层 仓储名称
        /// </summary>
        public string EntityFrameworkCoreRepositoryName { get; set; }

        /// <summary>
        /// Domain层 接口仓储名称
        /// </summary>
        public string DomainIRepositoryName { get; set; }

        /// <summary>
        /// Application 类文件 从解决方案根目录开始的 路径
        /// </summary>
        public string ApplicationDirectoryPath { get; set; }

        /// <summary>
        /// Core 类文件 从解决方案根目录开始的 路径
        /// </summary>
        public string CoreDirectoryPath { get; set; }

        /// <summary>
        /// Vue 页面名称
        /// </summary>
        public string VueWebPageName { get; set; }

        /// <summary>
        /// Web Controller 名称
        /// </summary>
        public string WebControllerName { get; set; }

        /// <summary>
        /// 实体 所有 属性
        /// </summary>
        public PropertyInfo[] EntityProps { get; set; }

        /// <summary>
        /// 实体 Dto 属性
        /// </summary>
        public PropertyInfo[] DtoProps { get; set; }

        /// <summary>
        /// 实体 GetAllInput 输入参数 属性
        /// </summary>
        public PropertyInfo[] GetAllInputProps { get; set; }

        /// <summary>
        /// 实体 时间类型 属性
        /// </summary>
        public PropertyInfo[] EntityDateTimeProps { get; set; }

        /// <summary>
        /// 实体 枚举类型 属性
        /// </summary>
        public PropertyInfo[] EntityEnumsProps { get; set; }
    }
}