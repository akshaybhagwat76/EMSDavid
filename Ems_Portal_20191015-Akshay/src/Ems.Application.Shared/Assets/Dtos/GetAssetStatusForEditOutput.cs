using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class GetAssetStatusForEditOutput
    {
		public CreateOrEditAssetStatusDto AssetStatus { get; set; }


    }
}