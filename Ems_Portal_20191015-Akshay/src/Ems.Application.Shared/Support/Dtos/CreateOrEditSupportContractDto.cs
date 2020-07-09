
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditSupportContractDto : EntityDto<int?>
    {

        [Required]
        public string Title { get; set; }


        public string Reference { get; set; }


        [Required]
        public string Description { get; set; }


        public DateTime StartDate { get; set; }


        public DateTime EndDate { get; set; }


        public bool IsRFQTemplate { get; set; }


        public bool IsAcknowledged { get; set; }


        public string AcknowledgedBy { get; set; }


        public DateTime AcknowledgedAt { get; set; }


        public int? VendorId { get; set; }


        public int? AssetOwnerId { get; set; }

    }
}