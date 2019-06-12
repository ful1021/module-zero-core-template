using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using AbpCompanyName.AbpProjectName.Authorization;
using AbpCompanyName.AbpProjectName.Authorization.Permissions;
using AbpCompanyName.AbpProjectName.Authorization.Roles;
using AbpCompanyName.AbpProjectName.Authorization.Roles.Cache;
using AbpCompanyName.AbpProjectName.Authorization.Users;
using AbpCompanyName.AbpProjectName.Roles.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AbpCompanyName.AbpProjectName.Roles
{
    [AbpAuthorize(AppPermissions.System_Roles)]
    public class RoleAppService : AsyncCrudAppServiceBase<Role, RoleListDto, int, PagedRoleResultRequestDto, CreateRoleDto, RoleDto>, IRoleAppService
    {
        private readonly RoleCache _roleCache;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;

        public RoleAppService(IRepository<Role> repository, RoleCache roleCache, RoleManager roleManager, UserManager userManager)
            : base(repository)
        {
            _roleCache = roleCache;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //public override async Task<RoleDto> Create(CreateRoleDto input)
        //{
        //    CheckCreatePermission();

        //    var role = ObjectMapper.Map<Role>(input);
        //    role.SetNormalizedName();

        //    CheckErrors(await _roleManager.CreateAsync(role));

        //    var grantedPermissions = PermissionManager
        //        .GetAllPermissions()
        //        .Where(p => input.GrantedPermissions.Contains(p.Name))
        //        .ToList();

        //    await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

        //    return MapToEntityDto(role);
        //}

        public async Task<ListResultDto<RoleListDto>> GetRolesAsync(GetRolesInput input)
        {
            var roles = await _roleManager
                .Roles
                .WhereIf(
                    !input.Permission.IsNullOrWhiteSpace(),
                    r => r.Permissions.Any(rp => rp.Name == input.Permission && rp.IsGranted)
                )
                .ToListAsync();

            return new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(roles));
        }

        //public override async Task<RoleDto> Update(RoleDto input)
        //{
        //    CheckUpdatePermission();

        //    var role = await _roleManager.GetRoleByIdAsync(input.Id);

        //    ObjectMapper.Map(input, role);

        //    CheckErrors(await _roleManager.UpdateAsync(role));

        //    var grantedPermissions = PermissionManager
        //        .GetAllPermissions()
        //        .Where(p => input.GrantedPermissions.Contains(p.Name))
        //        .ToList();

        //    await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

        //    return MapToEntityDto(role);
        //}

        public override async Task Delete(EntityDto<int> input)
        {
            CheckDeletePermission();

            var role = await _roleManager.FindByIdAsync(input.Id.ToString());
            var users = await _userManager.GetUsersInRoleAsync(role.NormalizedName);

            foreach (var user in users)
            {
                CheckErrors(await _userManager.RemoveFromRoleAsync(user, role.NormalizedName));
            }

            CheckErrors(await _roleManager.DeleteAsync(role));
        }

        public Task<ListResultDto<PermissionDto>> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();

            return Task.FromResult(new ListResultDto<PermissionDto>(
                ObjectMapper.Map<List<PermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList()
            ));
        }

        protected override IQueryable<Role> CreateFilteredQuery(PagedRoleResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Permissions)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword)
                || x.DisplayName.Contains(input.Keyword)
                || x.Description.Contains(input.Keyword));
        }

        protected override async Task<Role> GetEntityByIdAsync(int id)
        {
            return await Repository.GetAllIncluding(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == id);
        }

        protected override IQueryable<Role> ApplySorting(IQueryable<Role> query, PagedRoleResultRequestDto input)
        {
            return query.OrderBy(r => r.DisplayName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<GetRoleForEditOutput> GetRoleForEdit(EntityDto input)
        {
            var permissions = PermissionManager.GetAllPermissions();
            var grantedPermissions = new Permission[0];
            RoleEditDto roleEditDto;

            if (input.Id > 0) //Editing existing role?
            {
                var role = await _roleManager.GetRoleByIdAsync(input.Id);
                grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
                roleEditDto = ObjectMapper.Map<RoleEditDto>(role);
            }
            else
            {
                roleEditDto = new RoleEditDto();
            }

            return new GetRoleForEditOutput
            {
                Role = roleEditDto,
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        public async Task CreateOrUpdateRole(CreateOrUpdateRoleInput input)
        {
            if (input.Id.HasValue && input.Id > 0)
            {
                await UpdateRoleAsync(input);
            }
            else
            {
                await CreateRoleAsync(input);
            }
        }

        protected virtual async Task UpdateRoleAsync(CreateOrUpdateRoleInput input)
        {
            Debug.Assert(input.Id != null, "input.Role.Id should be set.");

            var role = await _roleManager.GetRoleByIdAsync(input.Id.Value);
            role.DisplayName = input.DisplayName;
            role.Description = input.Description;
            role.IsDefault = input.IsDefault;

            await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
        }

        protected virtual async Task CreateRoleAsync(CreateOrUpdateRoleInput input)
        {
            var role = new Role(AbpSession.TenantId, input.DisplayName) { IsDefault = input.IsDefault, Description = input.Description };
            CheckErrors(await _roleManager.CreateAsync(role));
            await CurrentUnitOfWork.SaveChangesAsync(); //It's done to get Id of the role.
            await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
        }

        private async Task UpdateGrantedPermissionsAsync(Role role, List<string> grantedPermissionNames)
        {
            var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(grantedPermissionNames);
            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
        }
    }
}