
using System;
using Abp.Application.Services.Dto;

namespace Ems.Quotations.Dtos
{
    public class ItemTypeDto : EntityDto
    {
		public string Type { get; set; }

		public string Description { get; set; }



    }
}