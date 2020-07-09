
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Organizations.Dtos
{
    public class CreateOrEditContactDto : EntityDto<int?>
    {

		public bool HeadOfficeContact { get; set; }
		
		
		public string ContactName { get; set; }
		
		
		public string PhoneOffice { get; set; }
		
		
		public string PhoneMobile { get; set; }
		
		
		public string Fax { get; set; }
		
		
		public string Address { get; set; }


		public string EmailAddress { get; set; }


        public string Position { get; set; }
		
		
		public string Department { get; set; }
		
		
		public string ContactLoc8GUID { get; set; }
		
		
		 public long? UserId { get; set; }
		 
		 		 public int? VendorId { get; set; }
		 
		 		 public int? AssetOwnerId { get; set; }
		 
		 		 public int? CustomerId { get; set; }
		 
		 
    }
}