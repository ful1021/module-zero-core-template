using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using AbpCompanyName.AbpProjectName.Authorization.Permissions;
using AbpCompanyName.AbpProjectName.Authorization.Roles.Dto;
using AbpCompanyName.AbpProjectName.Authorization.Users;
using Microsoft.AspNetCore.Identity;

namespace AbpCompanyName.AbpProjectName.Authorization.Roles
{
    [AbpAuthorize(PermissionNames.System_Roles)]
    public class RoleAppService : PagedCudAppService<Role, RoleListDto, RoleDto, int, RoleListInput, RoleCreateInput, RoleUpdateInput>, IRoleAppService
    {
        private readonly IPermissionManager _permissionManager;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;

        public RoleAppService(IRepository<Role> repository, IPermissionManager permissionManager, RoleManager roleManager, UserManager userManager)
            : base(repository)
        {
            _permissionManager = permissionManager;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        #region 增删改

        [AbpAuthorize(PermissionNames.System_Roles_Create)]
        public override async Task<RoleDto> Create(RoleCreateInput input)
        {
            var role = new Role(AbpSession.TenantId, input.DisplayName) { IsDefault = input.IsDefault };
            CheckErrors(await _roleManager.CreateAsync(role));
            await CurrentUnitOfWork.SaveChangesAsync(); //It's done to get Id of the role.
            await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);

            return MapToEntityDto(role);
        }

        [AbpAuthorize(PermissionNames.System_Roles_Edit)]
        public async Task<RoleGetForEditOutput> GetForEdit(EntityDto input)
        {
            var role = await _roleManager.GetRoleByIdAsync(input.Id);
            var grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();

            return new RoleGetForEditOutput
            {
                DisplayName = role.DisplayName,
                Id = role.Id,
                IsDefault = role.IsDefault,
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        [AbpAuthorize(PermissionNames.System_Roles_Edit)]
        public override async Task<RoleDto> Update(RoleUpdateInput input)
        {
            var role = await _roleManager.GetRoleByIdAsync(input.Id);
            role.DisplayName = input.DisplayName;
            role.IsDefault = input.IsDefault;

            await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);

            return MapToEntityDto(role);
        }

        [AbpAuthorize(PermissionNames.System_Roles_Delete)]
        public override async Task Delete(EntityDto<int> input)
        {
            var role = await _roleManager.FindByIdAsync(input.Id.ToString());
            var users = await _userManager.GetUsersInRoleAsync(role.NormalizedName);

            foreach (var user in users)
            {
                CheckErrors(await _userManager.RemoveFromRoleAsync(user, role.NormalizedName));
            }

            CheckErrors(await _roleManager.DeleteAsync(role));
        }

        #endregion 增删改

        protected override IQueryable<Role> CreateFilteredQuery(RoleListInput input)
        {
            return Repository.GetAllIncluding(x => x.Permissions)
                .WhereIf(!input.Permission.IsNullOrWhiteSpace(), r => r.Permissions.Any(rp => rp.Name == input.Permission && rp.IsGranted))
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword)
                || x.DisplayName.Contains(input.Keyword));
        }

        #region 私有方法

        private void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        private async Task UpdateGrantedPermissionsAsync(Role role, List<string> grantedPermissionNames)
        {
            var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(grantedPermissionNames);
            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
        }

        #endregion 私有方法
    }
}