using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Organizations.Dtos
{
    public class GetContactForEditOutput
    {
		public CreateOrEditContactDto Contact { get; set; }

		public string UserName { get; set;}

		public string VendorName { get; set;}

		public string AssetOwnerName { get; set;}

		public string CustomerName { get; set;}


    }
}