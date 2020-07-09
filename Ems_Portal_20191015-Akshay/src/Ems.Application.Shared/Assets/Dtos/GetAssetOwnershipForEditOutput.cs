using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class GetAssetOwnershipForEditOutput
    {
		public CreateOrEditAssetOwnershipDto AssetOwnership { get; set; }

		public string AssetReference { get; set;}

		public string AssetOwnerName { get; set;}


    }
}