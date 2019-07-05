﻿using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace AbpCompanyName.AbpProjectName.ExtendColumns.Dto
{
    /// <summary>
    ///  新增输入参数
    /// </summary>
    public class ExtendColumnDto : EntityDto<int>
    {
        /// <summary>
        /// 对应列内容的字段名
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 支持扩展列的表
        /// </summary>
        public AbpCompanyName.AbpProjectName.ExtendColumns.ExtendTableName TableName { get; set; }

        /// <summary>
        /// 列头显示文字
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 列宽
        /// </summary>
        public int Width { get; set; }

    }    
}
