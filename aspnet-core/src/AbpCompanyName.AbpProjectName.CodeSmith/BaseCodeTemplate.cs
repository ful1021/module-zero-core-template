using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AbpCompanyName.AbpProjectName.Helper;
using CodeSmith.Engine;

namespace AbpCompanyName.AbpProjectName
{
    /// <summary>
    /// 模板父类
    /// </summary>
    public class BaseCodeTemplate : CodeTemplate
    {
        public const string CompanyName = "AbpCompanyName";
        public const string ProjectTemplateName = "AbpProjectName";
        public const string SlnName = CompanyName + "." + ProjectTemplateName;
        public const string ApplicationAssemblyName = SlnName + ".Application";
        public const string CoreAssemblyName = SlnName + ".Core";

        public void OnEntityChanged(object sender, System.EventArgs args)
        {
            var isAutoSetProp = Tool.TryToBool(GetProperty("IsAutoSetProp"));
            if (!isAutoSetProp)
            {
                return;
            }
            string entityName = GetEntityName();
            Type assType = GetEntityAssemblyType(entityName);

            SetPropertyDirectory(assType, entityName);

            #region 实体类

            SetProperty("EntityNamespace", assType.Namespace);
            SetProperty("EntitySummary", Tool.GetClassSummary(assType));

            var pkName = "Id";
            SetProperty("PkName", pkName);
            SetProperty("PkType", Tool.GetPropertyType(assType, pkName));

            var entityProps = Tool.GetProperties(assType);

            SetProperty("IHasCreationTime", Tool.HasCreationTime(entityProps));
            SetProperty("ICreationAudited", Tool.IsCreationAudited(entityProps));
            SetProperty("IHasModificationTime", Tool.HasModificationTime(entityProps));
            SetProperty("IModificationAudited", Tool.IsModificationAudited(entityProps));
            SetProperty("IAudited", Tool.IsAudited(entityProps));

            #endregion 实体类

            SetProperty("CoreAssemblyName", CoreAssemblyName);

            SetProperty("IRepositoryName", "I" + entityName + "Repository");
            SetProperty("RepositoryName", entityName + "Repository");
            SetProperty("RepositoryCamelName", Tool.ToFirstLetterCamel(entityName) + "Repository");

            #region 命名 application

            SetProperty("DtoName", entityName + "Dto");
            SetProperty("QueryDtoName", entityName + "QueryDto");
            SetProperty("GetAllInputName", entityName + "GetAllInput");
            SetProperty("CreateOrUpdateInputName", entityName + "Dto");//CreateOrUpdateInput
            SetProperty("CreateInputName", entityName + "CreateInput");
            SetProperty("UpdateInputName", entityName + "Dto");// "UpdateInput"

            SetProperty("ApplicationAssemblyName", ApplicationAssemblyName);

            SetProperty("ApplicationServiceName", entityName + "AppService");
            SetProperty("IApplicationServiceName", "I" + entityName + "AppService");

            #endregion 命名 application

            SetProperty("WebControllerName", entityName + "Controller");
        }

        public void SetPropertyDirectory(Type assType, string entityName)
        {
            var baseDirectory = Tool.TryToString(GetProperty("BaseDirectory"));
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                string currentAssemblyPath = GetCurrentAssemblyDirectory();
                baseDirectory = CombinePath(currentAssemblyPath, "../../../");
            }

            var namespaces = assType.Namespace.Split('.').ToList();
            namespaces.Remove(CompanyName);
            namespaces.Remove(ProjectTemplateName);
            var classPath = string.Join("\\", namespaces);

            var directoryDto = CombinePath(baseDirectory, ApplicationAssemblyName, classPath, "Dto");
            var directoryApplication = CombinePath(baseDirectory, ApplicationAssemblyName, classPath);
            var directoryIApplication = CombinePath(baseDirectory, ApplicationAssemblyName, classPath);

            SetProperty("DirectoryDto", directoryDto);
            SetProperty("DirectoryApplication", directoryApplication);
            SetProperty("DirectoryIApplication", directoryIApplication);
        }

        public string CombinePath(params string[] paths)
        {
            return Path.GetFullPath(Path.Combine(paths));
        }

        public void OnBaseDirectoryChanged(object sender, System.EventArgs args)
        {
            string entityName = GetEntityName();
            Type assType = GetEntityAssemblyType(entityName);

            SetPropertyDirectory(assType, entityName);
        }

        public Type GetEntityAssemblyType(string entityName)
        {
            string currentAssemblyPath = GetCurrentAssemblyDirectory();

            string dllFolder = Path.GetFullPath(Path.Combine(currentAssemblyPath, "../../../AbpCompanyName.AbpProjectName.Web.Core/bin/Debug/net461/"));

            var coreDllFile = Path.Combine(dllFolder, CoreAssemblyName + ".dll");

            var assType = Tool.GetAssemblyType(coreDllFile, entityName);
            return assType;
        }

        public string GetEntityName()
        {
            return Tool.TryToString(GetProperty("EntityName"));
        }

        /// <summary>
        /// 获取实体属性
        /// </summary>
        public EntityProperty GetEntityProps()
        {
            string entityName = GetEntityName();
            Type assType = GetEntityAssemblyType(entityName);

            var entityProps = Tool.GetProperties(assType);
            var entityDateTimeProps = Tool.GetDateTimeProperties(entityProps);
            var entityEnumsProps = Tool.GetEnumProperties(entityProps);

            return new EntityProperty
            {
                EntityProps = entityProps,
                EntityDateTimeProps = entityDateTimeProps,
                EntityEnumsProps = entityEnumsProps,
            };
        }

        /// <summary>
        /// 获取 GetAll 方法 属性
        /// </summary>
        /// <param name="entityColumns"></param>
        /// <returns></returns>
        public PropertyInfo[] GetAllInputProperties(PropertyInfo[] entityColumns)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (var col in entityColumns)
            {
                if (Tool.IsAbpValueObject(col))
                {
                    continue;
                }
                list.Add(col);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取 Dto 方法 属性
        /// </summary>
        /// <param name="entityColumns"></param>
        /// <returns></returns>
        public PropertyInfo[] GetDtoProperties(PropertyInfo[] entityColumns)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (var col in entityColumns)
            {
                if (Tool.IsIn(col.Name, "Id"))
                {
                    continue;
                }
                if (Tool.IsInFullAudited(col))
                {
                    continue;
                }
                if (Tool.IsList(col))
                {
                    continue;
                }
                list.Add(col);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取当前程序集根目录
        /// </summary>
        /// <returns></returns>
        public string GetCurrentAssemblyDirectory()
        {
            return Path.GetDirectoryName(CodeTemplateInfo.AssemblyDependencies.FirstOrDefault(a => a.Contains("AbpCompanyName.AbpProjectName.CodeSmith")));
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
        /// RenderToResponse<BuildMenuTemplate>();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RenderToResponse<T>(Action<T> action = null) where T : CodeTemplate, new()
        {
            T temp = new T();
            action?.Invoke(temp);
            this.CopyPropertiesTo(temp);
            temp.Render(this.Response);
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