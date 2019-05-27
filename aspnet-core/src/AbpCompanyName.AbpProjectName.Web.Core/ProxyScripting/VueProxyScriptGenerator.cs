using System.Linq;
using System.Text;
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

            script.AppendLine(@"
import request from '@/utils/request'
import abp from '@/utils/abp'
// const clonedeep = require('lodash.clonedeep')
import { clonedeep } from '@/utils'
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
            script.AppendLine("        clonedeep(");
            script.AppendLine("          {");

            AddAjaxCallParameters(script, controller, action);

            script.AppendLine("          },");
            script.AppendLine("          ajaxParams");
            script.AppendLine("        )");
            script.AppendLine("      )");
            script.AppendLine("    },");
        }

        private static void AddAjaxCallParameters(StringBuilder script, ControllerApiDescriptionModel controller, ActionApiDescriptionModel action)
        {
            var httpMethod = action.HttpMethod?.ToUpperInvariant() ?? "POST";

            script.AppendLine("            url: '/" + ProxyScriptingHelper.GenerateUrlWithParameters(action) + "',");
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

            var body = ProxyScriptingHelper.GenerateBody(action);
            if (!body.IsNullOrEmpty())
            {
                script.AppendLine(",");
                script.Append("            data: JSON.stringify(" + body + ")");
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