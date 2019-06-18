using System;
using Abp.Domain.Repositories;
using Abp.GeneralTree;
using AbpCompanyName.AbpProjectName.DataDictionaries.Dto;

namespace AbpCompanyName.AbpProjectName.DataDictionaries
{
    public class DataDictionaryAppService : AsyncCrudAppServiceBase<DataDictionary, DataDictionaryQueryDto, Guid>, IDataDictionaryAppService
    {
        private readonly IGeneralTreeManager<DataDictionary, Guid> _generalTreeManager;

        public DataDictionaryAppService(IRepository<DataDictionary, Guid> repository, IGeneralTreeManager<DataDictionary, Guid> generalTreeManager)
            : base(repository)
        {
            _generalTreeManager = generalTreeManager;
        }
    }
}