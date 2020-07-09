using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAssetsWithWorkordersInput : PagedAndSortedResultRequestDto
    {
        public int Days { get; set; }
    }
}
