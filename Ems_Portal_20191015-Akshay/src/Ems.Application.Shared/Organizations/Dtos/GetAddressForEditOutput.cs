using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Organizations.Dtos
{
    public class GetAddressForEditOutput
    {
		public CreateOrEditAddressDto Address { get; set; }

		public string CustomerName { get; set;}

		public string AssetOwnerName { get; set;}

		public string VendorName { get; set;}


    }
}