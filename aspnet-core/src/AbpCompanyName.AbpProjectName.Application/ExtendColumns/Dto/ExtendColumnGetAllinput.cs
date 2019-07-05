using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace AbpCompanyName.AbpProjectName.ExtendColumns.Dto
{
    /// <summary>
    ///    获取所有 输入参数
    /// </summary>
    public class ExtendColumnGetAllInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExtendColumnGetAllInput()
        {
            Sorting = "CreationTime DESC";
        }
        
        /// <summary>
        /// 创建时间
        /// </summary>       
        public DateTime[] CreationTime { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>       
        public long? CreatorUserId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>       
        public System.Collections.Generic.ICollection`1[[Abp.Events.Bus.IEventData, Abp, Version=4.7.0.0, Culture=neutral, PublicKeyToken=null]]? DomainEvents { get; set; }
        
        /// <summary>
        /// 
        /// </summary>       
        public int[] Id { get; set; }
        
        /// <summary>
        /// 对应列内容的字段名
        /// </summary>       
        public string Key { get; set; }
        
        /// <summary>
        /// 上次修改时间
        /// </summary>       
        public DateTime[] LastModificationTime { get; set; }
        
        /// <summary>
        /// 上次修改人
        /// </summary>       
        public long? LastModifierUserId { get; set; }
        
        /// <summary>
        /// 支持扩展列的表
        /// </summary>       
        public AbpCompanyName.AbpProjectName.ExtendColumns.ExtendTableName? TableName { get; set; }
        
        /// <summary>
        /// 列头显示文字
        /// </summary>       
        public string Title { get; set; }
        
        /// <summary>
        /// 列宽
        /// </summary>       
        public int[] Width { get; set; }
        
    }
}