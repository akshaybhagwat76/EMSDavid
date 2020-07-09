
using System;
using Abp.Application.Services.Dto;

namespace Ems.Vendors.Dtos
{
    public class VendorDto : EntityDto
    {
		public string Reference { get; set; }

		public string Name { get; set; }

		public string Identifier { get; set; }

		public string LogoUrl { get; set; }

		public string Website { get; set; }

		public string VendorLoc8GUID { get; set; }


		 public int? SsicCodeId { get; set; }

		 		 public int CurrencyId { get; set; }

		 
    }
}