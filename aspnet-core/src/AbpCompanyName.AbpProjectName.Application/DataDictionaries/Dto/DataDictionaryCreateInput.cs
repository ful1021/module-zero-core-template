using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.DataDictionaries.Dto
{
    public class DataDictionaryCreateInput
    {
        [Required]
        [StringLength(DataDictionary.NameMaxLength)]
        public string Name { get; set; }

        public int? ParentId { get; set; }
    }
}