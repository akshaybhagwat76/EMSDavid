
using System;
using Abp.Application.Services.Dto;

namespace Ems.Organizations.Dtos
{
    public class AddressDto : EntityDto
    {
		public string AddressEntryName { get; set; }

		public bool IsHeadOffice { get; set; }

		public string AddressLine1 { get; set; }

		public string AddressLine2 { get; set; }

		public string PostalCode { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Country { get; set; }

		public string AddressLoc8GUID { get; set; }

		public bool IsDefaultForBilling { get; set; }

		public bool IsDefaultForShipping { get; set; }


		 public int? CustomerId { get; set; }

		 		 public int? AssetOwnerId { get; set; }

		 		 public int? VendorId { get; set; }

		 
    }
}