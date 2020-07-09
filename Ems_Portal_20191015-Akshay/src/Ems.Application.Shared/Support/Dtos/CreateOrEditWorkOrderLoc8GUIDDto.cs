
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditWorkOrderLoc8GUIDDto : EntityDto<int?>
    {

		public string Loc8GUID { get; set; }
		
		
		[Required]
		public string Subject { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		
		public string Location { get; set; }
		
		
		public DateTime? StartDate { get; set; }
		
		
		public string EndDate { get; set; }
		
		
		public DateTime DateRaised { get; set; }
		
		
		public string Remark { get; set; }
		
		
		 public int WorkOrderTypeId { get; set; }
		 
		 		 public int VendorId { get; set; }
		 
		 		 public int? SupportContractId { get; set; }
		 
		 		 public int? IncidentId { get; set; }
		 
		 		 public int? SupportItemId { get; set; }
		 
		 		 public long UserId { get; set; }
		 
		 		 public int? CustomerId { get; set; }
		 
		 		 public int? AssetOwnerId { get; set; }
		 
		 		 public int? RfqId { get; set; }
		 
		 
    }
}