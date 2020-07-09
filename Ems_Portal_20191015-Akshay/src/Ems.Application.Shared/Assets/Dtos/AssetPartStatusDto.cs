
using System;
using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class AssetPartStatusDto : EntityDto
    {
		public string Status { get; set; }

		public string Description { get; set; }



    }
}