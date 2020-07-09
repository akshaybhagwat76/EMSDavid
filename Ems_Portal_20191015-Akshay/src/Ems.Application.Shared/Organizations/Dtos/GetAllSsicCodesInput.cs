using Abp.Application.Services.Dto;
using System;

namespace Ems.Organizations.Dtos
{
    public class GetAllSsicCodesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string CodeFilter { get; set; }

		public string SSICFilter { get; set; }



    }
}