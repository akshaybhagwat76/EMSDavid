using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Telematics.Dtos
{
    public class GetUsageMetricRecordForEditOutput
    {
		public CreateOrEditUsageMetricRecordDto UsageMetricRecord { get; set; }

		public string UsageMetricMetric { get; set;}


    }
}