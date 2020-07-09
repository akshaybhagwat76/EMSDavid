
using System;
using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class IncidentPriorityDto : EntityDto
    {
		public string Priority { get; set; }

		public string Description { get; set; }

		public int PriorityLevel { get; set; }



    }
}