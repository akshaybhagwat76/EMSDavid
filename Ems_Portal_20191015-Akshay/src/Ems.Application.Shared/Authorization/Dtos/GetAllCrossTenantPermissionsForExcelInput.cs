using Abp.Application.Services.Dto;
using System;

namespace Ems.Authorization.Dtos
{
    public class GetAllCrossTenantPermissionsForExcelInput
    {
		public string Filter { get; set; }

		public int? MaxTenantRefIdFilter { get; set; }
		public int? MinTenantRefIdFilter { get; set; }

		public string EntityTypeFilter { get; set; }

		public string TenantsFilter { get; set; }

		public string TenantTypeFilter { get; set; }



    }
}