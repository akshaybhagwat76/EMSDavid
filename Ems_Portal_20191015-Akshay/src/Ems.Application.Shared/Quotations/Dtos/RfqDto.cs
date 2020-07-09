
using System;
using Abp.Application.Services.Dto;

namespace Ems.Quotations.Dtos
{
    public class RfqDto : EntityDto
    {
		public string Title { get; set; }

		public DateTime RequestDate { get; set; }

		public DateTime RequiredBy { get; set; }

		public string Description { get; set; }

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