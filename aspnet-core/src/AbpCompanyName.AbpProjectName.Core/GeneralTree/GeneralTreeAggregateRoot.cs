﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.GeneralTree;
using AbpCompanyName.AbpProjectName.Domain.Entities;

//https://github.com/maliming/Abp.GeneralTree/blob/master/README.CN.md

namespace AbpCompanyName.AbpProjectName.GeneralTree
{
    /// <summary>
    /// 适用于管理各种树结构实体，例如：区域，组织，类别，行业和具有父子实体的其他实体.
    /// </summary>
    public abstract class GeneralTreeAggregateRoot<TTree, TPrimaryKey> : AuditedAggregateRoot<TPrimaryKey>, IGeneralTree<TTree, TPrimaryKey>, IExtendableObject
        where TPrimaryKey : struct
    {
        public const int CodeMaxLength = 128;
        public const int NameMaxLength = 256;

        /// <summary>
        /// 编码
        /// </summary>
        [MaxLength(CodeMaxLength)]
        public virtual string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(NameMaxLength)]
        public virtual string Name { get; set; }

        public virtual string FullName { get; set; }

        /// <summary>
        /// 名称值类型
        /// </summary>
        public virtual StringTextType NameTextType { get; set; }

        /// <summary>
        /// 排序【越小越靠前】
        /// </summary>
        public virtual int Sort { get; set; } = 10000;

        /// <summary>
        /// A JSON formatted string to extend the containing object.
        /// </summary>
        public virtual string ExtensionData { get; set; }

        public virtual int Level { get; set; }

        public virtual TPrimaryKey? ParentId { get; set; }

        public virtual TTree Parent { get; set; }

        public virtual ICollection<TTree> Children { get; set; }
    }
}