using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.GeneralTree;
using AbpCompanyName.AbpProjectName.Authorization;
using AbpCompanyName.AbpProjectName.DataDictionaries.Dto;

namespace AbpCompanyName.AbpProjectName.DataDictionaries
{
    public class DataDictionaryAppService : AppServiceBase<DataDictionary, int>, IDataDictionaryAppService
    {
        private readonly ITreeManager<DataDictionary, int> _generalTreeManager;

        public DataDictionaryAppService(IRepository<DataDictionary> repository, ITreeManager<DataDictionary, int> generalTreeManager)
            : base(repository)
        {
            _generalTreeManager = generalTreeManager;
        }

        [AbpAuthorize(PermissionNames.Dict_DictData_List)]
        public async Task<ListResultDto<DataDictionaryQueryDto>> List(DataDictionaryGetAllInput input)
        {
            var query = Repository.GetAll().Select(a => new DataDictionaryQueryDto
            {
                TypeCode = a.TypeCode,
                Order = a.Order,
                IsStatic = a.IsStatic,
                IsAllowAddChildren = true,

                Id = a.Id,
                Code = a.Code,
                Name = a.Name,
                ParentId = a.ParentId,
                Level = a.Level
            });

            var entities = await AsyncQueryableExecuter.ToListAsync(query);

            var list = entities.ToTreeDtoOrder<DataDictionaryQueryDto, int>(arr => arr.OrderBy(b => b.TypeCode).ThenBy(b => b.Order).ThenBy(b => b.Id)).ToList();

            return new ListResultDto<DataDictionaryQueryDto>(list);
        }

        [AbpAuthorize(PermissionNames.Dict_DictData_Create)]
        public async Task Create(DataDictionaryCreateInput input)
        {
            var tree = new DataDictionary
            {
                Name = input.Name,
                ParentId = input.ParentId
            };
            await _generalTreeManager.CreateAsync(tree);
        }

        [AbpAuthorize(PermissionNames.Dict_DictData_Create)]
        public async Task BulkCreate(List<DataDictionaryBulkCreateDto> input)
        {
            foreach (var item in input)
            {
                var tree = MapToEntity(item);

                await _generalTreeManager.BulkCreateAsync(tree);
            }
        }

        private DataDictionary MapToEntity(DataDictionaryBulkCreateDto item)
        {
            var tree = new DataDictionary
            {
                Name = item.Name
            };
            if (item.Children != null && item.Children.Count > 0)
            {
                tree.Children = item.Children.Select(a => MapToEntity(a)).ToList();
            }
            return tree;
        }

        [AbpAuthorize(PermissionNames.Dict_DictData_Edit)]
        public async Task Update(DataDictionaryUpdateInput input)
        {
            var tree = await Repository.GetAsync(input.Id);
            tree.Name = input.Name;
            await _generalTreeManager.UpdateAsync(tree);
        }

        [AbpAuthorize(PermissionNames.Dict_DictData_Edit)]
        public async Task Move(DataDictionaryMoveInput input)
        {
            await _generalTreeManager.MoveOrderAsync(input.Id, input.NewParentId, input.Order);
        }

        [AbpAuthorize(PermissionNames.Dict_DictData_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _generalTreeManager.DeleteAsync(input.Id);
        }
    }
}