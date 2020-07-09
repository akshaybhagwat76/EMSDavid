
using System;
using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class AssetPartExtendedDto : AssetPartDto
    {
        public string AssetPartStatus { get; set; }
        public string AssetPartType { get; set; }
        public string AssetReference { get; set; }
        public string ItemType { get; set; }
		 
    }
}