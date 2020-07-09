using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class GetLeaseAgreementForEditOutput
    {
		public CreateOrEditLeaseAgreementDto LeaseAgreement { get; set; }

		public string ContactContactName { get; set;}

		public string AssetOwnerName { get; set;}

		public string CustomerName { get; set;}


    }
}