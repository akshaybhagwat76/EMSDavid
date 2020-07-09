using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class GetAssetOwnerForEditOutput
    {
		public CreateOrEditAssetOwnerDto AssetOwner { get; set; }

		public string CurrencyCode { get; set;}

		public string SsicCodeCode { get; set;}


    }
}