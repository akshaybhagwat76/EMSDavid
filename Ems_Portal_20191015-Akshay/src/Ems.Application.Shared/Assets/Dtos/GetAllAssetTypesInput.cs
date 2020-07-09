using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAllAssetTypesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}