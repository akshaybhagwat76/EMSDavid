
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Quotations.Dtos
{
    public class CreateOrEditRfqDto : EntityDto<int?>
    {

		[Required]
		public string Title { get; set; }
		
		
		public DateTime RequestDate { get; set; }
		
		
		public DateTime RequiredBy { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		
		[Required]
		public string Requirements { get; set; }
		
		
		 public int RfqTypeId { get; set; }
		 
		 		 public int? AssetOwnerId { get; set; }
		 
		 		 public int? CustomerId { get; set; }
		 
		 		 public int? AssetClassId { get; set; }
		 
		 		 public int? IncidentId { get; set; }
		 
		 		 public int VendorId { get; set; }
		 
		 		 public long? UserId { get; set; }
		 
		 
    }
}