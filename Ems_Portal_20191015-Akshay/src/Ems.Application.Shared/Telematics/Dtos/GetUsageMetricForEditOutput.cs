using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Telematics.Dtos
{
    public class GetUsageMetricForEditOutput
    {
		public CreateOrEditUsageMetricDto UsageMetric { get; set; }

		public string LeaseItemItem { get; set;}

		public string UomUnitOfMeasurement { get; set;}

        public string AssetReference { get; set; }
    }
}