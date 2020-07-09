using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class GetAssetForEditOutput
    {
		public CreateOrEditAssetDto Asset { get; set; }

		public string AssetClassClass { get; set;}

		public string AssetStatusStatus { get; set;}


    }
}