using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.Authorization.Users
{
    /// <summary>
    /// 用户类型
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// 后台用户
        /// </summary>
        [Display(Name = "后台用户")]
        BackEnd = 0,

        /// <summary>
        /// 前端用户
        /// </summary>
        [Display(Name = "前端用户")]
        FrontEnd = 1
    }
}