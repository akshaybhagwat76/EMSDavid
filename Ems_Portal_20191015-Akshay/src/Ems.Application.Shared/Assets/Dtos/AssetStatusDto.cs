
using System;
using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class AssetStatusDto : EntityDto
    {
		public string Status { get; set; }

		public string Description { get; set; }



    }
}