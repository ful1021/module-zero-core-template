using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.GeneralTree.Dto;

namespace Abp.GeneralTree
{
    public class GeneralTreeAppService<TEntity> : AppServiceBase<TEntity, int>
        where TEntity : TreeEntity<TEntity, int>
    {
        private readonly ITreeManager<TEntity, int> _generalTreeManager;

        public GeneralTreeAppService(IRepository<TEntity> repository, ITreeManager<TEntity, int> generalTreeManager)
            : base(repository)
        {
            _generalTreeManager = generalTreeManager;
        }

        public async Task Move(GeneralTreeMoveInput input)
        {
            await _generalTreeManager.MoveOrderAsync(input.Id, input.NewParentId, input.Order);
        }

        public async Task Delete(EntityDto input)
        {
            await _generalTreeManager.DeleteAsync(input.Id);
        }
    }
}