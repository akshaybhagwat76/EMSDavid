using Abp.Application.Services.Dto;
using System;

namespace Ems.Organizations.Dtos
{
    public class GetAllContactsForExcelInput
    {
		public string Filter { get; set; }

		public int HeadOfficeContactFilter { get; set; }

		public string ContactNameFilter { get; set; }

		public string PhoneOfficeFilter { get; set; }

		public string PhoneMobileFilter { get; set; }

		public string FaxFilter { get; set; }

		public string AddressFilter { get; set; }

		public string EmailAddressFilter { get; set; }

		public string PositionFilter { get; set; }

		public string DepartmentFilter { get; set; }

		public string ContactLoc8GUIDFilter { get; set; }


		 public string UserNameFilter { get; set; }

		 		 public string VendorNameFilter { get; set; }

		 		 public string AssetOwnerNameFilter { get; set; }

		 		 public string CustomerNameFilter { get; set; }

		 
    }
}