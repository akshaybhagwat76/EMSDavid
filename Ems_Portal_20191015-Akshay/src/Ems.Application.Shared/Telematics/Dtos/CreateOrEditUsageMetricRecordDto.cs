
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Telematics.Dtos
{
    public class CreateOrEditUsageMetricRecordDto : EntityDto<int?>
    {

        public string Reference { get; set; }


        public DateTime? StartTime { get; set; }


        public DateTime? EndTime { get; set; }


        public decimal? UnitsConsumed { get; set; }


        public int UsageMetricId { get; set; }


    }
}