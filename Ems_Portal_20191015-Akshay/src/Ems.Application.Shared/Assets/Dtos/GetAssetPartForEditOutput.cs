using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class GetAssetPartForEditOutput
    {
		public CreateOrEditAssetPartDto AssetPart { get; set; }

		public string AssetPartTypeType { get; set;}

		public string AssetPartName { get; set;}

		public string AssetPartStatusStatus { get; set;}

		public string UsageMetricMetric { get; set;}

		public string AttachmentFilename { get; set;}

		public string AssetReference { get; set;}

		public string ItemTypeType { get; set;}


    }
}