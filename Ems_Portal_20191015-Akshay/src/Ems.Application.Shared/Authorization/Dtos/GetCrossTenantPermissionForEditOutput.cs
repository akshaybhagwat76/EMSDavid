using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Authorization.Dtos
{
    public class GetCrossTenantPermissionForEditOutput
    {
		public CreateOrEditCrossTenantPermissionDto CrossTenantPermission { get; set; }


    }
}