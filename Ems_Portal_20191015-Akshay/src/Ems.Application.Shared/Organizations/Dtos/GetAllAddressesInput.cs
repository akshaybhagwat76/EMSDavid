using Abp.Application.Services.Dto;
using System;

namespace Ems.Organizations.Dtos
{
    public class GetAllAddressesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string AddressEntryNameFilter { get; set; }

		public int IsHeadOfficeFilter { get; set; }

		public string AddressLine1Filter { get; set; }

		public string AddressLine2Filter { get; set; }

		public string PostalCodeFilter { get; set; }

		public string CityFilter { get; set; }

		public string StateFilter { get; set; }

		public string CountryFilter { get; set; }

		public string AddressLoc8GUIDFilter { get; set; }

		public int IsDefaultForBillingFilter { get; set; }

		public int IsDefaultForShippingFilter { get; set; }

		public string CustomerNameFilter { get; set; }

		public string AssetOwnerNameFilter { get; set; }

		public string VendorNameFilter { get; set; }

        public int? CustomerId { get; set; }

        public int? AssetOwnerId { get; set; }

        public int? VendorId { get; set; }

    }
}