
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Authorization.Dtos
{
    public class CreateOrEditCrossTenantPermissionDto : EntityDto<int?>
    {

		public int TenantRefId { get; set; }
		
		
		[Required]
		public string EntityType { get; set; }
		
		
		public string Tenants { get; set; }
		
		
		[Required]
		public string TenantType { get; set; }
		
		

    }
}