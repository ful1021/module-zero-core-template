using Abp.Domain.Repositories;
using Abp.GeneralTree;
using AbpCompanyName.AbpProjectName.DataDictionaries.Dto;

namespace AbpCompanyName.AbpProjectName.DataDictionaries
{
    public class DataDictionaryAppService : AsyncCrudAppServiceBase<DataDictionary, DataDictionaryQueryDto, int, DataDictionaryGetAllInput, DataDictionaryDto, DataDictionaryDto>, IDataDictionaryAppService
    {
        private readonly IGeneralTreeManager<DataDictionary, int> _generalTreeManager;

        public DataDictionaryAppService(IRepository<DataDictionary> repository, IGeneralTreeManager<DataDictionary, int> generalTreeManager)
            : base(repository)
        {
            _generalTreeManager = generalTreeManager;
        }
    }
}