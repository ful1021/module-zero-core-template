using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace AbpCompanyName.AbpProjectName.BizLogs.Dto
{
    /// <summary>
    /// 业务日志   获取所有 输入参数
    /// </summary>
    public class BizLogGetAllInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BizLogGetAllInput()
        {
            Sorting = "CreationTime DESC";
        }
        
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
        /// 创建时间
        /// </summary>       
        public DateTime[] CreationTime { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>       
        public long? CreatorUserId { get; set; }
        
        /// <summary>
        /// A JSON formatted string to extend the containing object.
        /// </summary>       
        public string ExtensionData { get; set; }
        
        /// <summary>
        /// 
        /// </summary>       
        public Guid? Id { get; set; }
        
    }
}