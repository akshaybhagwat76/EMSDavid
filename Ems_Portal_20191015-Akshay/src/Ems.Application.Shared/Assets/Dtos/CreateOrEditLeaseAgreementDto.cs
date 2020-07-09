
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class CreateOrEditLeaseAgreementDto : EntityDto<int?>
    {

		public string Reference { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		
		[Required]
		public string Title { get; set; }
		
		
		public string Terms { get; set; }
		
		
		public DateTime? StartDate { get; set; }
		
		
		public DateTime? EndDate { get; set; }
		
		
		
		
		
		 public int? ContactId { get; set; }
		 
		 		 public int? AssetOwnerId { get; set; }
		 
		 		 public int? CustomerId { get; set; }
		 
		 
    }
}