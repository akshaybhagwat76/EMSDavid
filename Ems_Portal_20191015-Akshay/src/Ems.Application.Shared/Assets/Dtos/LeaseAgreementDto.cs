
using System;
using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class LeaseAgreementDto : EntityDto
    {
		public string Reference { get; set; }

		public string Description { get; set; }

		public string Title { get; set; }

		public string Terms { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		


		 public int? ContactId { get; set; }

		 		 public int? AssetOwnerId { get; set; }

		 		 public int? CustomerId { get; set; }

		 
    }
}