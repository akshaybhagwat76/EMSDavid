
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Organizations.Dtos
{
    public class CreateOrEditSsicCodeDto : EntityDto<int?>
    {

		[Required]
		public string Code { get; set; }
		
		
		[Required]
		public string SSIC { get; set; }
		
		

    }
}