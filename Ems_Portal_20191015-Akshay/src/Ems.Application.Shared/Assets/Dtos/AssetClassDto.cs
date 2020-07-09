
using System;
using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class AssetClassDto : EntityDto
    {
		public string Manufacturer { get; set; }

		public string Model { get; set; }

		public string Specification { get; set; }

		public string Class { get; set; }


		 public int AssetTypeId { get; set; }

		 
    }
}