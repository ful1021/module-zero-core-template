using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Web.Api.Modeling;
using Abp.Web.Api.ProxyScripting.Generators;

namespace AbpCompanyName.AbpProjectName.ProxyScripting
{
    public class VueProxyScriptGenerator : IProxyScriptGenerator, ITransientDependency
    {
        public const string Name = "vue";

        public string CreateScript(ApplicationApiDescriptionModel model)
        {
            var script = new StringBuilder();

            script.AppendLine(@"import request from '@/utils/request'
import abp from '@/utils/abp'
import { extend } from '@/utils'
");

            foreach (var module in model.Modules.Values)
            {
                AddModuleScript(script, module);
            }

            script.AppendLine();
            var index = 1;
            var moduleNames = model.Modules.Values.Select(a => GetModuleName(a)).Distinct().ToList();
            script.AppendLine("export {");
            foreach (var item in moduleNames)
            {
                script.Append($"  {item}");
                if (index++ < moduleNames.Count)
                {
                    script.AppendLine(",");
                }
                else
                {
                    script.AppendLine();
                }
            }
            script.Append("}");

            return script.ToString();
        }

        private static void AddModuleScript(StringBuilder script, ModuleApiDescriptionModel module)
        {
            script.AppendLine($"// module '{GetModuleName(module)}'");
            script.AppendLine($"const {module.Name.ToCamelCase()} = {{");
            foreach (var controller in module.Controllers.Values)
            {
                AddControllerScript(script, module, controller);
            }

            script.AppendLine("}");
        }

        private static string GetModuleName(ModuleApiDescriptionModel module)
        {
            return module.Name.ToCamelCase();
        }

        private static void AddControllerScript(StringBuilder script, ModuleApiDescriptionModel module, ControllerApiDescriptionModel controller)
        {
            script.AppendLine($"  {controller.Name.ToCamelCase()}: {{");
            foreach (var action in controller.Actions.Values)
            {
                AddActionScript(script, module, controller, action);
            }
            script.AppendLine("  },");
        }

        private static void AddActionScript(StringBuilder script, ModuleApiDescriptionModel module, ControllerApiDescriptionModel controller, ActionApiDescriptionModel action)
        {
            var parameterList = ProxyScriptingJsFuncHelper.GenerateJsFuncParameterList(action, "ajaxParams");
            script.AppendLine($"    {action.Name.ToCamelCase()}({parameterList}) {{");

            script.AppendLine("      return request(");
            script.AppendLine("        extend(");
            script.AppendLine("          true,");
            script.AppendLine("          {");

            AddAjaxCallParameters(script, controller, action);

            script.AppendLine("          },");
            script.AppendLine("          ajaxParams");
            script.AppendLine("        )");
            script.AppendLine("      )");
            script.AppendLine("    },");
        }

        public static string GenerateUrlWithParameters(ActionApiDescriptionModel action)
        {
            //TODO: Can be optimized using StringBuilder?
            var url = ReplacePathVariables(action.Url, action.Parameters);
            url = AddQueryStringParameters(url, action.Parameters);
            return url;
        }

        private static string AddQueryStringParameters(string url, IList<ParameterApiDescriptionModel> actionParameters)
        {
            var queryStringParameters = actionParameters
                .Where(p => p.BindingSourceId.IsIn(ParameterBindingSources.ModelBinding, ParameterBindingSources.Query) && p.Name == p.NameOnMethod)
                .ToArray();

            if (!queryStringParameters.Any())
            {
                return url;
            }

            var qsBuilderParams = queryStringParameters
                .Select(p => $"{{ name: '{p.Name.ToCamelCase()}', value: {ProxyScriptingJsFuncHelper.GetParamNameInJsFunc(p)} }}")
                .JoinAsString(", ");

            return url + $"' + abp.utils.buildQueryString([{qsBuilderParams}]) + '";
        }

        private static string ReplacePathVariables(string url, IList<ParameterApiDescriptionModel> actionParameters)
        {
            var pathParameters = actionParameters
                .Where(p => p.BindingSourceId == ParameterBindingSources.Path)
                .ToArray();

            if (!pathParameters.Any())
            {
                return url;
            }

            foreach (var pathParameter in pathParameters)
            {
                url = url.Replace($"{{{pathParameter.Name}}}", $"' + {ProxyScriptingJsFuncHelper.GetParamNameInJsFunc(pathParameter)} + '");
            }

            return url;
        }

        private static void AddAjaxCallParameters(StringBuilder script, ControllerApiDescriptionModel controller, ActionApiDescriptionModel action)
        {
            var httpMethod = action.HttpMethod?.ToUpperInvariant() ?? "POST";

            script.AppendLine("            url: '/" + GenerateUrlWithParameters(action) + "',");
            script.Append("            method: '" + httpMethod + "'");

            if (action.ReturnValue.Type == typeof(void))
            {
                script.AppendLine(",");
                script.Append("            dataType: null");
            }

            var headers = ProxyScriptingHelper.GenerateHeaders(action, 8);
            if (headers != null)
            {
                script.AppendLine(",");
                script.Append("            headers: " + headers);
            }

            if (httpMethod.IsIn("GET", "DELETE"))
            {
                var methodParamNames = action.Parameters.Where(p => p.BindingSourceId.IsIn(ParameterBindingSources.ModelBinding, ParameterBindingSources.Query) && p.Name != p.NameOnMethod).Select(p => p.NameOnMethod).Distinct().FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(methodParamNames))
                {
                    var paramName = ProxyScriptingJsFuncHelper.NormalizeJsVariableName(methodParamNames.ToCamelCase());
                    script.AppendLine(",");
                    script.Append("            params: " + paramName);
                }
            }
            var body = ProxyScriptingHelper.GenerateBody(action);
            if (!body.IsNullOrEmpty())
            {
                script.AppendLine(",");
                script.Append("            data: " + body);
            }
            else
            {
                var formData = ProxyScriptingHelper.GenerateFormPostData(action, 8);
                if (!formData.IsNullOrEmpty())
                {
                    script.AppendLine(",");
                    script.Append("            data: " + formData);
                }
            }

            script.AppendLine();
        }
    }
}