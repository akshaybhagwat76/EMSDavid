using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class GetSupportContractForEditOutput
    {
		public CreateOrEditSupportContractDto SupportContract { get; set; }

		public string VendorName { get; set;}

		public string AssetOwnerName { get; set;}


    }
}