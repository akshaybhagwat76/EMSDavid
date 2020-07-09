
using System;
using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class AssetOwnerDto : EntityDto
    {
		public string Reference { get; set; }

		public string Name { get; set; }

		public string Identifier { get; set; }

		public string LogoUrl { get; set; }

		public string Website { get; set; }


		 public int? CurrencyId { get; set; }

		 		 public int? SsicCodeId { get; set; }

		 
    }
}