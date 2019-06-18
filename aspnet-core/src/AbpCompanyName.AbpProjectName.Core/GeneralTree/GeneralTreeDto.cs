using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.GeneralTree;

namespace AbpCompanyName.AbpProjectName.GeneralTree
{
    public class GeneralTreeDto<TTree, TPrimaryKey> : AuditedEntityDto<TPrimaryKey>, IGeneralTreeDto<TTree, TPrimaryKey>
        where TPrimaryKey : struct
    {
        public TPrimaryKey? ParentId { get; set; }
        public ICollection<TTree> Children { get; set; }
    }
}