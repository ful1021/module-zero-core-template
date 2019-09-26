using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using AbpCompanyName.AbpProjectName.Authorization.Permissions;
using AbpCompanyName.AbpProjectName.Authorization.Roles;
using AbpCompanyName.AbpProjectName.Authorization.Users.Dto;
using Microsoft.AspNetCore.Identity;

namespace AbpCompanyName.AbpProjectName.Authorization.Users
{
    [AbpAuthorize(PermissionNames.System_Users)]
    public class UserAppService : AppServiceBase<User, long>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly LogInManager _logInManager;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IPasswordHasher<User> passwordHasher,
            LogInManager logInManager)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _passwordHasher = passwordHasher;
            _logInManager = logInManager;
        }

        protected IQueryable<User> CreateFilteredQuery(UserListInput input)
        {
            return Repository.GetAllIncluding(x => x.Roles)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        [AbpAuthorize(PermissionNames.System_Users_List)]
        public async Task<PagedResultDto<UserListDto>> PagedList(UserListInput input)
        {
            var query = CreateFilteredQuery(input);
            return await base.ToPagedList(query, input, MapToList);
        }

        protected UserListDto MapToList(User user)
        {
            var roles = _roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => new { r.NormalizedName, r.DisplayName }).ToList();
            var userDto = ObjectMapper.Map<UserListDto>(user);
            userDto.DisplayRoleNames = roles.Select(r => r.DisplayName).Distinct().ToArray();
            userDto.RoleNames = roles.Select(r => r.NormalizedName).Distinct().ToArray();
            return userDto;
        }

        #region 增删改

        [AbpAuthorize(PermissionNames.System_Users_Create)]
        public async Task<UserDto> Create(UserCreateInput input)
        {
            var user = ObjectMapper.Map<User>(input);
            user.SetNormalizedNames();

            user.Surname = user.Name;
            user.TenantId = AbpSession.TenantId;
            user.IsEmailConfirmed = true;

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            CheckErrors(await _userManager.CreateAsync(user, input.Password));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            return MapToEntityDto(user);
        }

        [AbpAuthorize(PermissionNames.System_Users_Edit)]
        public async Task<UserDto> Update(UserUpdateInput input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);

            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            return MapToEntityDto(user);
        }

        #region 编辑用户权限

        [AbpAuthorize(PermissionNames.System_Users_ChangePermissions)]
        public async Task<UserGetPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            var grantedPermissions = await _userManager.GetGrantedPermissionsAsync(user);

            return new UserGetPermissionsForEditOutput
            {
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        [AbpAuthorize(PermissionNames.System_Users_ChangePermissions)]
        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.ResetAllPermissionsAsync(user);
        }

        [AbpAuthorize(PermissionNames.System_Users_ChangePermissions)]
        public async Task UpdateUserPermissions(UserUpdatePermissionsInput input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
            await _userManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        }

        #endregion 编辑用户权限

        /// <summary>
        /// 针对登陆密码输错次数过多，锁定的用户解锁
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.System_Users_Unlock)]
        public async Task UnlockUser(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            user.Unlock();
        }

        [AbpAuthorize(PermissionNames.System_Users_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }

        #endregion 增删改

        public async Task ChangeLanguage(UserChangeLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        #region 私有方法

        protected UserDto MapToEntityDto(User user)
        {
            var roles = _roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => new { r.NormalizedName, r.DisplayName }).ToList();
            var userDto = ObjectMapper.Map<UserDto>(user);
            userDto.DisplayRoleNames = roles.Select(r => r.DisplayName).Distinct().ToArray();
            userDto.RoleNames = roles.Select(r => r.NormalizedName).Distinct().ToArray();
            return userDto;
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        #endregion 私有方法

        [AbpAuthorize]
        public async Task<bool> ChangePassword(UserChangePasswordDto input)
        {
            long userId = AbpSession.UserId.Value;
            var user = await _userManager.GetUserByIdAsync(userId);
            var loginAsync = await _logInManager.LoginAsync(user.UserName, input.CurrentPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("您的‘现有密码’与记录的密码不匹配。请重试或与管理员联系以获得重置密码的帮助。");
            }
            //if (!new Regex(AccountAppService.PasswordRegex).IsMatch(input.NewPassword))
            //{
            //    throw new UserFriendlyException("Passwords must be at least 8 characters, contain a lowercase, uppercase, and number.");
            //}
            user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
            CurrentUnitOfWork.SaveChanges();
            return true;
        }

        [AbpAuthorize(PermissionNames.System_Users_ResetPassword)]
        public async Task<bool> ResetPassword(UserResetPasswordDto input)
        {
            long currentUserId = AbpSession.UserId.Value;
            var currentUser = await _userManager.GetUserByIdAsync(currentUserId);
            var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("当前账号密码不正确");
            }
            if (currentUser.IsDeleted || !currentUser.IsActive)
            {
                return false;
            }
            //var roles = await _userManager.GetRolesAsync(currentUser);
            //if (!roles.Contains(StaticRoleNames.Tenants.Admin))
            //{
            //    throw new UserFriendlyException("当前账号没有权限重置密码");
            //}

            var user = await _userManager.GetUserByIdAsync(input.UserId);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
                CurrentUnitOfWork.SaveChanges();
            }

            return true;
        }
    }
}