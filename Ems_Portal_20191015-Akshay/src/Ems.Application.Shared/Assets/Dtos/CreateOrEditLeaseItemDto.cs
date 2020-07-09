
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class CreateOrEditLeaseItemDto : EntityDto<int?>
    {

        public DateTime? DateAllocated { get; set; }


        public decimal? AllocationPercentage { get; set; }


        public string Terms { get; set; }


        public decimal? UnitRentalRate { get; set; }


        public decimal? UnitDepositRate { get; set; }


        public DateTime? StartDate { get; set; }


        public DateTime? EndDate { get; set; }





        public int? RentalUomRefId { get; set; }


        public decimal? DepositUomRefId { get; set; }


        //[Required]
        public string Item { get; set; }


        //[Required]
        public string Description { get; set; }


        public int? AssetClassId { get; set; }

        public int? AssetId { get; set; }

        public int? LeaseAgreementId { get; set; }


    }
}