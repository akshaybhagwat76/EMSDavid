using Abp.Application.Services.Dto;
using System;

namespace Ems.Quotations.Dtos
{
    public class GetAllItemTypesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}