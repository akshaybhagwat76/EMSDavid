
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditWorkOrderDto : EntityDto<int?>
    {

		public string Loc8GUID { get; set; }
		
		
		[Required]
		public string Subject { get; set; }
		
		
		public string Description { get; set; }
		
		
		public string Location { get; set; }
		
		
		public DateTime? StartDate { get; set; }
		
		
		public DateTime? EndDate { get; set; }
		
		
		public string Remarks { get; set; }
		
		
		public string Attachments { get; set; }
		
		
		 public int WorkOrderPriorityId { get; set; }
		 
		 		 public int WorkOrderTypeId { get; set; }
		 
		 		 public int VendorId { get; set; }
		 
		 		 public int? IncidentId { get; set; }
		 
		 		 public int? SupportItemId { get; set; }
		 
		 		 public long UserId { get; set; }
		 
		 		 public int? CustomerId { get; set; }
		 
		 		 public int? AssetOwnershipId { get; set; }
		 
		 		 public int WorkOrderStatusId { get; set; }
		 
		 
    }
}