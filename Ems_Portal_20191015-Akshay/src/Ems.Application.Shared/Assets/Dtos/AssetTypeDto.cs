
using System;
using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class AssetTypeDto : EntityDto
    {
		public string Code { get; set; }

		public string Type { get; set; }

		public string Description { get; set; }

		public int Sort { get; set; }



    }
}