
using System;
using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class WorkOrderPriorityDto : EntityDto
    {
		public string Priority { get; set; }

		public int PriorityLevel { get; set; }



    }
}