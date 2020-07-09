using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class GetAssetClassForEditOutput
    {
		public CreateOrEditAssetClassDto AssetClass { get; set; }

		public string AssetTypeType { get; set;}


    }
}