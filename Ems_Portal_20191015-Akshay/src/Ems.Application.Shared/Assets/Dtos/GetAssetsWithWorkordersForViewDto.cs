using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAssetsWithWorkordersForViewDto
    {
        public AssetDto Asset { get; set; }

        public int Workorders { get; set; }
    }
}
