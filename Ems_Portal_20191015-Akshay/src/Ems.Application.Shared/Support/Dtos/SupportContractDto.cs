
using System;
using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class SupportContractDto : EntityDto
    {
		public string Title { get; set; }

		public string Reference { get; set; }

		public string Description { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		

		public bool IsRFQTemplate { get; set; }

		public bool IsAcknowledged { get; set; }

		public string AcknowledgedBy { get; set; }

		public DateTime AcknowledgedAt { get; set; }


		 public int? VendorId { get; set; }

		 		 public int? AssetOwnerId { get; set; }

		 
    }
}