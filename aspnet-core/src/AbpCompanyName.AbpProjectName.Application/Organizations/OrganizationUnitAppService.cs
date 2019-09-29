using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.GeneralTree;
using Abp.Linq.Extensions;
using Abp.Organizations;
using AbpCompanyName.AbpProjectName.Authorization;
using AbpCompanyName.AbpProjectName.Authorization.Roles;
using AbpCompanyName.AbpProjectName.Organizations.Dto;
using Microsoft.EntityFrameworkCore;

namespace AbpCompanyName.AbpProjectName.Organizations
{
    [AbpAuthorize(PermissionNames.System_OrganizationUnits)]
    public class OrganizationUnitAppService : AbpProjectNameAppServiceBase, IOrganizationUnitAppService
    {
        private readonly OrganizationUnitManager _organizationUnitManager;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;
        private readonly RoleManager _roleManager;

        public OrganizationUnitAppService(
            OrganizationUnitManager organizationUnitManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            RoleManager roleManager,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository)
        {
            _organizationUnitManager = organizationUnitManager;
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _roleManager = roleManager;
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
        }

        public async Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnits()
        {
            var query = from ou in _organizationUnitRepository.GetAll()
                        join uoUser in _userOrganizationUnitRepository.GetAll() on ou.Id equals uoUser.OrganizationUnitId into uoUserGrouped
                        join uoRole in _organizationUnitRoleRepository.GetAll() on ou.Id equals uoRole.OrganizationUnitId into uoRoleGrouped
                        select new OrganizationUnitDto
                        {
                            Code = ou.Code,
                            CreationTime = ou.CreationTime,
                            CreatorUserId = ou.CreatorUserId,
                            DisplayName = ou.DisplayName,
                            Id = ou.Id,
                            LastModificationTime = ou.LastModificationTime,
                            LastModifierUserId = ou.LastModifierUserId,
                            MemberCount = uoUserGrouped.Count(),
                            ParentId = ou.ParentId,
                            RoleCount = uoRoleGrouped.Count()
                        };

            var items = await query.ToListAsync();
            var list = items.ToTreeDtoOrderBy<OrganizationUnitDto, long, long>(a => a.Id).ToList();
            return new ListResultDto<OrganizationUnitDto>(list);
        }

        public async Task<ListResultDto<OrganizationUnitByDto>> GetOrganizationUnitsByUser(GetOrganizationUnitsByUserInput input)
        {
            var ids = _userOrganizationUnitRepository.GetAll()
                .Where(uou => uou.UserId == input.UserId)
                .Select(uou => uou.OrganizationUnitId);

            var list = await GetAllByIds(ids);
            return new ListResultDto<OrganizationUnitByDto>(list);
        }

        public async Task<ListResultDto<OrganizationUnitByDto>> GetOrganizationUnitsByRole(GetOrganizationUnitsByRoleInput input)
        {
            var ids = _organizationUnitRoleRepository.GetAll()
                .Where(uou => uou.RoleId == input.RoleId)
                .Select(uou => uou.OrganizationUnitId);

            var list = await GetAllByIds(ids);
            return new ListResultDto<OrganizationUnitByDto>(list);
        }

        private async Task<List<OrganizationUnitByDto>> GetAllByIds(IQueryable<long> ids)
        {
            var query = _organizationUnitRepository.GetAll()
                .Where(a => ids.Contains(a.Id))
                .Select(ou => new OrganizationUnitByDto
                {
                    Code = ou.Code,
                    DisplayName = ou.DisplayName,
                    Id = ou.Id,
                    ParentId = ou.ParentId,
                });

            var items = await query.ToListAsync();
            var list = items.ToTreeDtoOrderBy<OrganizationUnitByDto, long, long>(a => a.Id).ToList();
            return list;
        }

        public async Task<PagedResultDto<OrganizationUnitUserListDto>> GetOrganizationUnitUsers(GetOrganizationUnitUsersInput input)
        {
            var query = from ouUser in _userOrganizationUnitRepository.GetAll()
                        join ou in _organizationUnitRepository.GetAll() on ouUser.OrganizationUnitId equals ou.Id
                        join user in UserManager.Users on ouUser.UserId equals user.Id
                        where ouUser.OrganizationUnitId == input.Id
                        select new
                        {
                            ouUser,
                            user
                        };

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<OrganizationUnitUserListDto>(
                totalCount,
                items.Select(item =>
                {
                    var organizationUnitUserDto = ObjectMapper.Map<OrganizationUnitUserListDto>(item.user);
                    organizationUnitUserDto.AddedTime = item.ouUser.CreationTime;
                    return organizationUnitUserDto;
                }).ToList());
        }

        public async Task<PagedResultDto<OrganizationUnitRoleListDto>> GetOrganizationUnitRoles(GetOrganizationUnitRolesInput input)
        {
            var query = from ouRole in _organizationUnitRoleRepository.GetAll()
                        join ou in _organizationUnitRepository.GetAll() on ouRole.OrganizationUnitId equals ou.Id
                        join role in _roleManager.Roles on ouRole.RoleId equals role.Id
                        where ouRole.OrganizationUnitId == input.Id
                        select new
                        {
                            ouRole,
                            role
                        };

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<OrganizationUnitRoleListDto>(
                totalCount,
                items.Select(item =>
                {
                    var organizationUnitRoleDto = ObjectMapper.Map<OrganizationUnitRoleListDto>(item.role);
                    organizationUnitRoleDto.AddedTime = item.ouRole.CreationTime;
                    return organizationUnitRoleDto;
                }).ToList());
        }

        [AbpAuthorize(PermissionNames.System_OrganizationUnits_ManageOrganizationTree)]
        public async Task<OrganizationUnitDto> CreateOrganizationUnit(CreateOrganizationUnitInput input)
        {
            var organizationUnit = new OrganizationUnit(AbpSession.TenantId, input.DisplayName, input.ParentId);

            await _organizationUnitManager.CreateAsync(organizationUnit);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<OrganizationUnitDto>(organizationUnit);
        }

        [AbpAuthorize(PermissionNames.System_OrganizationUnits_ManageOrganizationTree)]
        public async Task<OrganizationUnitDto> UpdateOrganizationUnit(UpdateOrganizationUnitInput input)
        {
            var organizationUnit = await _organizationUnitRepository.GetAsync(input.Id);

            organizationUnit.DisplayName = input.DisplayName;

            await _organizationUnitManager.UpdateAsync(organizationUnit);

            return await CreateOrganizationUnitDto(organizationUnit);
        }

        [AbpAuthorize(PermissionNames.System_OrganizationUnits_ManageOrganizationTree)]
        public async Task<OrganizationUnitDto> MoveOrganizationUnit(MoveOrganizationUnitInput input)
        {
            await _organizationUnitManager.MoveAsync(input.Id, input.NewParentId);

            return await CreateOrganizationUnitDto(
                await _organizationUnitRepository.GetAsync(input.Id)
                );
        }

        [AbpAuthorize(PermissionNames.System_OrganizationUnits_ManageOrganizationTree)]
        public async Task DeleteOrganizationUnit(EntityDto<long> input)
        {
            await _organizationUnitManager.DeleteAsync(input.Id);
        }

        [AbpAuthorize(PermissionNames.System_OrganizationUnits_ManageMembers)]
        public async Task RemoveUserFromOrganizationUnit(UserToOrganizationUnitInput input)
        {
            await UserManager.RemoveFromOrganizationUnitAsync(input.UserId, input.OrganizationUnitId);
        }

        [AbpAuthorize(PermissionNames.System_OrganizationUnits_ManageRoles)]
        public async Task RemoveRoleFromOrganizationUnit(RoleToOrganizationUnitInput input)
        {
            await _roleManager.RemoveFromOrganizationUnitAsync(input.RoleId, input.OrganizationUnitId);
        }

        [AbpAuthorize(PermissionNames.System_OrganizationUnits_ManageMembers)]
        public async Task AddUsersToOrganizationUnit(UsersToOrganizationUnitInput input)
        {
            foreach (var userId in input.UserIds)
            {
                await UserManager.AddToOrganizationUnitAsync(userId, input.OrganizationUnitId);
            }
        }

        [AbpAuthorize(PermissionNames.System_OrganizationUnits_ManageRoles)]
        public async Task AddRolesToOrganizationUnit(RolesToOrganizationUnitInput input)
        {
            foreach (var roleId in input.RoleIds)
            {
                await _roleManager.AddToOrganizationUnitAsync(roleId, input.OrganizationUnitId, AbpSession.TenantId);
            }
        }

        [AbpAuthorize(PermissionNames.System_OrganizationUnits_ManageMembers)]
        public async Task<PagedResultDto<NameValueDto>> FindUsers(FindOrganizationUnitUsersInput input)
        {
            var userIdsInOrganizationUnit = _userOrganizationUnitRepository.GetAll()
                .Where(uou => uou.OrganizationUnitId == input.OrganizationUnitId)
                .Select(uou => uou.UserId);

            var query = UserManager.Users
                .Where(u => !userIdsInOrganizationUnit.Contains(u.Id))
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter)
                );

            var userCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.Name)
                .ThenBy(u => u.Surname)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<NameValueDto>(
                userCount,
                users.Select(u =>
                    new NameValueDto(
                        u.FullName + " (" + u.EmailAddress + ")",
                        u.Id.ToString()
                    )
                ).ToList()
            );
        }

        [AbpAuthorize(PermissionNames.System_OrganizationUnits_ManageRoles)]
        public async Task<PagedResultDto<NameValueDto>> FindRoles(FindOrganizationUnitRolesInput input)
        {
            var roleIdsInOrganizationUnit = _organizationUnitRoleRepository.GetAll()
                .Where(uou => uou.OrganizationUnitId == input.OrganizationUnitId)
                .Select(uou => uou.RoleId);

            var query = _roleManager.Roles
                .Where(u => !roleIdsInOrganizationUnit.Contains(u.Id))
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.DisplayName.Contains(input.Filter) ||
                        u.Name.Contains(input.Filter)
                );

            var roleCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.DisplayName)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<NameValueDto>(
                roleCount,
                users.Select(u =>
                    new NameValueDto(
                        u.DisplayName,
                        u.Id.ToString()
                    )
                ).ToList()
            );
        }

        private async Task<OrganizationUnitDto> CreateOrganizationUnitDto(OrganizationUnit organizationUnit)
        {
            var dto = ObjectMapper.Map<OrganizationUnitDto>(organizationUnit);
            dto.MemberCount = await _userOrganizationUnitRepository.CountAsync(uou => uou.OrganizationUnitId == organizationUnit.Id);
            return dto;
        }
    }
}