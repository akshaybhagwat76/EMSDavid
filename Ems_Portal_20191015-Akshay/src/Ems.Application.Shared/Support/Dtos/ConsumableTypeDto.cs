
using System;
using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class ConsumableTypeDto : EntityDto
    {
		public string Type { get; set; }

		public string Description { get; set; }



    }
}