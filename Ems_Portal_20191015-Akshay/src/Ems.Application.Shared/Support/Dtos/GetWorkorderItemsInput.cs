using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetWorkorderItemsInput : PagedAndSortedResultRequestDto
    {
        public int Days { get; set; }
    }
}
