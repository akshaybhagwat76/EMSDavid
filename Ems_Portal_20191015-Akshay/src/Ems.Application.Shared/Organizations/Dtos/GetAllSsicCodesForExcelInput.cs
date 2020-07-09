using Abp.Application.Services.Dto;
using System;

namespace Ems.Organizations.Dtos
{
    public class GetAllSsicCodesForExcelInput
    {
		public string Filter { get; set; }

		public string CodeFilter { get; set; }

		public string SSICFilter { get; set; }



    }
}