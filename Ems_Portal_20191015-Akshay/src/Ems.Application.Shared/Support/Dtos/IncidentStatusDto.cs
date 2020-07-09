
using System;
using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class IncidentStatusDto : EntityDto
    {
		public string Status { get; set; }

		public string Description { get; set; }



    }
}