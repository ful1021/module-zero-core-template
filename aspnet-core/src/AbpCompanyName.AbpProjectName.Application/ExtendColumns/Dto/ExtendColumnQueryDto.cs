using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace AbpCompanyName.AbpProjectName.ExtendColumns.Dto
{
    /// <summary>
    /// 扩展列  查询Dto
    /// </summary>
    public class ExtendColumnQueryDto : ExtendColumnDto
    {
        public DateTime CreationTime { get; set; }
        public long? CreatorUserId { get; set; }
    }    
}
