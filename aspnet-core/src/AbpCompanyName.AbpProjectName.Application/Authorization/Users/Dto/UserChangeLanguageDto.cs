using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.Authorization.Users.Dto
{
    public class UserChangeLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}