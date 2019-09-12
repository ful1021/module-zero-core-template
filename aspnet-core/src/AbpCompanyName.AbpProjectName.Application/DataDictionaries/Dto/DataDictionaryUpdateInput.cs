using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.DataDictionaries.Dto
{
    public class DataDictionaryUpdateInput
    {
        [Required]
        [StringLength(DataDictionary.NameMaxLength)]
        public string Name { get; set; }

        [Range(1, int.MaxValue)]
        public int Id { get; set; }
    }
}