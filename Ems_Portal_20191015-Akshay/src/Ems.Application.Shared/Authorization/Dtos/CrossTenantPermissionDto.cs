
using System;
using Abp.Application.Services.Dto;

namespace Ems.Authorization.Dtos
{
    public class CrossTenantPermissionDto : EntityDto
    {
		public int TenantRefId { get; set; }

		public string EntityType { get; set; }

		public string Tenants { get; set; }

		public string TenantType { get; set; }



    }
}