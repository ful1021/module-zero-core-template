using System;
using System.ComponentModel.DataAnnotations;
using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace AbpCompanyName.AbpProjectName.BizLogs
{
    /// <summary>
    /// 业务日志
    /// </summary>
    public class BizLog : CreationAuditedEntity<Guid>, IExtendableObject
    {
        public const int BizNoMaxLength = 256;
        public const int BizTypeMaxLength = 128;
        public const int BizNameMaxLength = 512;

        public BizLog()
        {
            CreationTime = DateTime.Now;
            Id = SequentialGuidGenerator.Instance.Create();
        }

        /// <summary>
        /// 业务单号
        /// </summary>
        [MaxLength(BizNoMaxLength)]
        public string BizNo { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [MaxLength(BizTypeMaxLength)]
        public string BizType { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        [MaxLength(BizNameMaxLength)]
        public string BizName { get; set; }

        /// <summary>
        /// 业务描述
        /// </summary>
        public string BizDescription { get; set; }

        /// <summary>
        /// 业务单据数据
        /// </summary>
        public string BizData { get; set; }

        /// <summary>
        /// A JSON formatted string to extend the containing object.
        /// </summary>
        public virtual string ExtensionData { get; set; }
    }
}