using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms.Design;
using AbpCompanyName.AbpProjectName.Helper;
using CodeSmith.Engine;

namespace AbpCompanyName.AbpProjectName
{
    /// <summary>
    /// 模板父类
    /// </summary>
    public class BaseCodeTemplate : CodeTemplate
    {
        #region 设置公共属性

        private string _outputDirectory = "";

        /// <summary>
        /// 输出根目录路径
        /// </summary>
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        [Optional]
        [NotChecked]
        [Category("01.输出目录")]
        [Description("输出文件的根目录")]
        [DefaultValue("")]
        public string OutputDirectory
        {
            get
            {
                return _outputDirectory.Trim();
            }
            set
            {
                _outputDirectory = value;
                if (_outputDirectory.EndsWith("\\") == false)
                {
                    _outputDirectory += "\\";
                }
            }
        }

        private string _creator = "付亮";

        /// <summary>
        /// 创建人
        /// </summary>
        [Category("其它选项")]
        public string Creator
        {
            get { return _creator.Trim(); }
            set { _creator = value; }
        }

        #endregion 设置公共属性

        /// <summary>
        /// 初始化模板
        /// </summary>
        public virtual ClassNames Init()
        {
            string currentAssemblyPath = GetCurrentAssemblyDirectory();

            string dllFolder = Path.GetFullPath(Path.Combine(currentAssemblyPath, "../../../AbpCompanyName.AbpProjectName.Web.Core/bin/Debug/net461/"));

            string nameSpaceName = "AbpCompanyName.AbpProjectName";
            string entityName = Tool.TryToString(GetProperty("EntityName"));
            string vueSpaWebPageName = Tool.TryToString(GetProperty("VueSpaWebPageName"));
            var names = GetAssemblyFileNames(dllFolder, nameSpaceName, entityName, vueSpaWebPageName);
            return names;
        }

        /// <summary>
        /// 获取当前程序集根目录
        /// </summary>
        /// <returns></returns>
        public string GetCurrentAssemblyDirectory()
        {
            return Path.GetDirectoryName(CodeTemplateInfo.AssemblyDependencies.FirstOrDefault(a => a.Contains("AbpCompanyName.AbpProjectName.CodeSmith")));
        }

        /// <summary>
        /// 获取 AssemblyFile 各命名
        /// </summary>
        public static ClassNames GetAssemblyFileNames(string dllFolder, string nameSpaceName, string entityName, string vueSpaWebPageName = "")
        {
            var companyName = nameSpaceName.Split('.').FirstOrDefault();
            var projectTemplateName = nameSpaceName.Split('.').LastOrDefault();

            var applicationAssemblyName = companyName + "." + projectTemplateName + ".Application";
            var coreAssemblyName = companyName + "." + projectTemplateName + ".Core";

            var applicationDllFile = Path.Combine(dllFolder, applicationAssemblyName + ".dll");
            var coreDllFile = Path.Combine(dllFolder, coreAssemblyName + ".dll");

            Type assType = Tool.GetAssemblyType(coreDllFile, entityName);

            var entityDirectoryName = assType.Namespace.Replace(companyName + "." + projectTemplateName, "").Replace(".", "\\");
            var aplicationDirectoryPath = applicationAssemblyName + "\\" + entityDirectoryName;
            var coreDirectoryPath = coreAssemblyName + "\\" + entityDirectoryName;

            var entityProps = Tool.GetProperties(assType);
            var dtoColumns = Tool.GetDtoProperties(entityProps);
            var getAllInputColumns = Tool.GetAllInputColumns(entityProps);
            var entityDateTimeProps = Tool.GetDateTimeProperties(getAllInputColumns);
            var entityEnumsProps = Tool.GetEnumProperties(entityProps);

            var pkName = "Id";
            return new ClassNames()
            {
                PkName = pkName,
                PkType = Tool.GetPropertyType(assType, pkName),

                EntityName = entityName,
                EntityConstsName = entityName + "Consts",
                EntityNamespace = assType.Namespace,
                EntitySummary = Tool.GetClassSummary(assType),
                ApplicationDirectoryPath = aplicationDirectoryPath,
                CoreDirectoryPath = coreDirectoryPath,
                EntityProps = entityProps,
                DtoProps = dtoColumns,
                GetAllInputProps = getAllInputColumns,
                EntityDateTimeProps = entityDateTimeProps,
                EntityEnumsProps = entityEnumsProps,

                DomainIRepositoryName = "I" + entityName + "Repository",
                EntityFrameworkCoreRepositoryName = "EfCore" + entityName + "Repository",

                AppServiceName = entityName + "AppService",
                MgmtAppServiceName = entityName + "MgmtAppService",
                BaseAppServiceName = entityName + "BaseAppService",
                RepositoryName = Tool.ToFirstLetterCamel(entityName) + "Repository",
                DtoName = entityName + "Dto",
                QueryDtoName = entityName + "QueryDto",
                GetAllInputName = entityName + "GetAllInput",
                CreateOrUpdateInputName = entityName + "Dto",//"CreateOrUpdateInput",
                CreateInputName = entityName + "CreateInput",
                UpdateInputName = entityName + "Dto",// "UpdateInput",

                ApplicationDllFile = applicationDllFile,
                CoreDllFile = coreDllFile,

                CompanyName = companyName,
                ProjectTemplateName = projectTemplateName,

                ApplicationAssemblyName = applicationAssemblyName,
                CoreAssemblyName = coreAssemblyName,

                WebControllerName = entityName + "Controller",

                VueWebPageName = string.IsNullOrWhiteSpace(vueSpaWebPageName) ? Tool.ToFirstLetterCamel(entityName) : vueSpaWebPageName
            };
        }

        #region 生成文件

        /// <summary>
        /// 根据模板名称得到模板
        /// </summary>
        /// <param name="TemplateName"></param>
        /// <returns></returns>
        public CodeTemplate GetCodeTemplate(string TemplateName)
        {
            CodeTemplateCompiler compiler = new CodeTemplateCompiler(this.CodeTemplateInfo.DirectoryName + TemplateName);

            CodeTemplate template = null;

            compiler.CodeTemplateInfo.ToString();
            compiler.Compile();
            if (compiler.Errors.Count == 0)
            {
                template = compiler.CreateInstance();
            }
            else
            {
                System.Text.StringBuilder errorMessage = new System.Text.StringBuilder();
                for (int i = 0; i < compiler.Errors.Count; i++)
                {
                    errorMessage.Append(compiler.Errors[i].ToString()).Append("\r\n");
                }
                throw new ApplicationException(errorMessage.ToString());
            }

            //复制属性
            if (template != null)
            {
                CopyPropertiesTo(template);
            }

            return template;
        }

        /// <summary>
        /// 输出其它模块内容
        /// 需要在头部写命令注册：
        /// <%@ Register Template="BuildMenu.cst" Name="BuildMenuTemplate" MergeProperties="True" %>
        /// 调用：
        /// RenderToResponse<BuildMenuTemplate>(BuildMenu);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RenderToResponse<T>() where T : CodeTemplate, new()
        {
            T sm = new T();
            this.CopyPropertiesTo(sm);
            sm.Render(this.Response);
        }

        /// <summary>
        /// 输出其它模块内容
        /// </summary>
        /// <param name="isRender"></param>
        /// <param name="templatePath"></param>
        public void RenderOtherTemplate(bool isRender, string templatePath)
        {
            if (isRender)
            {
                CodeTemplate template = GetCodeTemplate(templatePath);
                template.Render(this.Response);
            }
        }

        /// <summary>
        /// 通用根据模板生成文件方法
        /// </summary>
        public void RenderToFile(bool isRender, string templatePath, string directory, string currclassName)
        {
            if (isRender)
            {
                //载入子模板
                CodeTemplate template = GetCodeTemplate(templatePath);
                //CopyPropertiesTo(template);

                RenderToFile(directory, currclassName, template);

                Response.WriteLine(templatePath + "代码生成完毕！");
            }
        }

        private void RenderToFile(string directory, string currclassName, CodeTemplate template)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            if (currclassName.IndexOf(".") > 0)
            {
                template.RenderToFile(Path.Combine(directory, currclassName), true);
            }
            else
            {
                template.RenderToFile(Path.Combine(directory, currclassName) + ".cs", true);
            }
        }

        #endregion 生成文件
    }
}