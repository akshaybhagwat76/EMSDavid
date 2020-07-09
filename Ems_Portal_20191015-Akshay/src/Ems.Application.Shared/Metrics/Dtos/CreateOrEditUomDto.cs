
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Metrics.Dtos
{
    public class CreateOrEditUomDto : EntityDto<int?>
    {

		[Required]
		public string UnitOfMeasurement { get; set; }
		
		

    }
}