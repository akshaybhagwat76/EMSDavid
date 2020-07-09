
using System;
using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class IncidentUpdateDto : EntityDto
    {
		public DateTime Updated { get; set; }

		public string Update { get; set; }


		 public long UserId { get; set; }

		 		 public int IncidentId { get; set; }

		 
    }
}