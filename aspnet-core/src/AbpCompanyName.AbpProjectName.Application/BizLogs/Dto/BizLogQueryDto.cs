using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace AbpCompanyName.AbpProjectName.BizLogs.Dto
{
    /// <summary>
    /// 业务日志  查询Dto
    /// </summary>
    public class BizLogQueryDto : BizLogDto
    {
        public DateTime CreationTime { get; set; }
        public long? CreatorUserId { get; set; }
    }    
}
