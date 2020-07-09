
using System;
using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class WorkOrderStatusDto : EntityDto
    {
		public string Status { get; set; }

		public string Description { get; set; }



    }
}