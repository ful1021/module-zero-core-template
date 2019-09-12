using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.DataDictionaries.Dto
{
    public class DataDictionaryMoveInput
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        public int? NewParentId { get; set; }
    }
}