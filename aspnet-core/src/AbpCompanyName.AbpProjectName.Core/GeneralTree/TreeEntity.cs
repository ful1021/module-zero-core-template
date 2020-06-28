using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;

//https://github.com/maliming/Abp.GeneralTree/blob/master/README.CN.md

namespace Abp.GeneralTree
{
    /// <summary>
    /// 适用于管理各种树结构实体，例如：区域，组织，类别，行业和具有父子实体的其他实体.
    /// </summary>
    public abstract class TreeEntity<TTree, TPrimaryKey> : AuditedAggregateRoot<TPrimaryKey>, IGeneralTree<TTree, TPrimaryKey>
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

        public virtual int Level { get; set; }

        /// <summary>
        /// 排序 ，越小则越向前【推荐使用 DisplayOrder】
        /// </summary>
        public virtual int Order { get; set; }

        /// <summary>
        /// 如果为 true，则不能修改或者删除
        /// </summary>
        public virtual bool IsStatic { get; set; }

        public virtual TPrimaryKey? ParentId { get; set; }

        public virtual TTree Parent { get; set; }

        public virtual ICollection<TTree> Children { get; set; }

        #region Code

        /// <summary>
        /// Length of a code unit between dots.
        /// </summary>
        public const int CodeUnitLength = 5;

        /// <summary>
        /// 获取下一个子节点Code
        /// </summary>
        /// <param name="parentCode"></param>
        /// <param name="lastChildCode"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetNextChildCode(string parentCode, string lastChildCode, int number)
        {
            if (!string.IsNullOrWhiteSpace(lastChildCode))
            {
                //Get the next code
                return CalculateNextCode(lastChildCode);
            }
            return AppendCode(parentCode, CreateCode(number));
        }

        /// <summary>
        /// Creates code for given numbers.
        /// Example: if numbers are 4,2 then returns "00004.00002";
        /// </summary>
        /// <param name="numbers">Numbers</param>
        public static string CreateCode(params int[] numbers)
        {
            if (numbers.IsNullOrEmpty())
            {
                return null;
            }

            return numbers.Select(number => number.ToString(new string('0', CodeUnitLength))).JoinAsString(".");
        }

        /// <summary>
        /// Appends a child code to a parent code.
        /// Example: if parentCode = "00001", childCode = "00042" then returns "00001.00042".
        /// </summary>
        /// <param name="parentCode">Parent code. Can be null or empty if parent is a root.</param>
        /// <param name="childCode">Child code.</param>
        public static string AppendCode(string parentCode, string childCode)
        {
            if (childCode.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(childCode), "childCode can not be null or empty.");
            }

            if (parentCode.IsNullOrEmpty())
            {
                return childCode;
            }

            return parentCode + "." + childCode;
        }

        /// <summary>
        /// Gets relative code to the parent.
        /// Example: if code = "00019.00055.00001" and parentCode = "00019" then returns "00055.00001".
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parentCode">The parent code.</param>
        public static string GetRelativeCode(string code, string parentCode)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            if (parentCode.IsNullOrEmpty())
            {
                return code;
            }

            if (code.Length == parentCode.Length)
            {
                return null;
            }

            return code.Substring(parentCode.Length + 1);
        }

        /// <summary>
        /// Calculates next code for given code.
        /// Example: if code = "00019.00055.00001" returns "00019.00055.00002".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string CalculateNextCode(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var parentCode = GetParentCode(code);
            var lastUnitCode = GetLastUnitCode(code);

            return AppendCode(parentCode, CreateCode(Convert.ToInt32(lastUnitCode) + 1));
        }

        /// <summary>
        /// Gets the last unit code.
        /// Example: if code = "00019.00055.00001" returns "00001".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string GetLastUnitCode(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var splittedCode = code.Split('.');
            return splittedCode[splittedCode.Length - 1];
        }

        /// <summary>
        /// Gets parent code.
        /// Example: if code = "00019.00055.00001" returns "00019.00055".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string GetParentCode(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            var splittedCode = code.Split('.');
            if (splittedCode.Length == 1)
            {
                return null;
            }

            return splittedCode.Take(splittedCode.Length - 1).JoinAsString(".");
        }

        #endregion Code
    }
}