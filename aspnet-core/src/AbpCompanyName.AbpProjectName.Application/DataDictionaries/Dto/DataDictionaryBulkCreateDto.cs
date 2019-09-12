using System.Collections.Generic;

namespace AbpCompanyName.AbpProjectName.DataDictionaries.Dto
{
    public class DataDictionaryBulkCreateDto
    {
        public string Name { get; set; }

        public List<DataDictionaryBulkCreateDto> Children { get; set; }
    }
}