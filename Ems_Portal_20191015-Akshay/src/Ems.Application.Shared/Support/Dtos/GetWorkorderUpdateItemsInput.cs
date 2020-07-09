using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetWorkorderUpdateItemsInput : PagedAndSortedResultRequestDto
    {
        public int Days { get; set; }
    }
}
