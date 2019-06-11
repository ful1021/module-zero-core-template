using Abp.Application.Services.Dto;

namespace AbpCompanyName.AbpProjectName.Enumerations.Dto
{
    public class EnumNameValueDto : NameValueDto<int>
    {
        /// <summary>
        /// 文本说明.
        /// </summary>
        public string Text { get; set; }

        public int Order { get; set; }
    }
}