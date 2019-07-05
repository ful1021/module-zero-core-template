using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Runtime.Validation;

namespace AbpCompanyName.AbpProjectName.BizLogs.Dto
{
    /// <summary>
    /// 业务日志 新增输入参数
    /// </summary>
    public class BizLogCreateInput : IShouldNormalize
    {
        /// <summary>
        /// 业务单据数据
        /// </summary>
        public string BizData { get; set; }

        /// <summary>
        /// 业务描述
        /// </summary>
        public string BizDescription { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string BizName { get; set; }

        /// <summary>
        /// 业务单号
        /// </summary>
        public string BizNo { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// A JSON formatted string to extend the containing object.
        /// </summary>
        public string ExtensionData { get; set; }

        
        public void Normalize()
        {
        }
    }    
}