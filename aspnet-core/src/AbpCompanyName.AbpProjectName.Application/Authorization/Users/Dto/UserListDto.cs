using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;

namespace AbpCompanyName.AbpProjectName.Authorization.Users.Dto
{
    public class UserListDto : EntityDto<long>
    {
        private Random r = new Random();
        public OrderStatus Status => (OrderStatus)this.r.Next(0, 4);
        public OrderStatus Status1 => (OrderStatus)this.r.Next(0, 4);
        public OrderStatus Status2 => (OrderStatus)this.r.Next(0, 4);

        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public bool IsActive { get; set; }

        public string FullName { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }

        public string[] DisplayRoleNames { get; set; }

        public string[] RoleNames { get; set; }
    }
}