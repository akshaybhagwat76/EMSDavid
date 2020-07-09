
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Quotations.Dtos
{
    public class CreateOrEditQuotationStatusDto : EntityDto<int?>
    {

		[Required]
		public string Status { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		

    }
}