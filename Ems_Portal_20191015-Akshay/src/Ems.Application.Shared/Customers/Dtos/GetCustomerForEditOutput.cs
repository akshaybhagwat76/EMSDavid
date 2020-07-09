using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Customers.Dtos
{
    public class GetCustomerForEditOutput
    {
		public CreateOrEditCustomerDto Customer { get; set; }

		public string CustomerTypeType { get; set;}

		public string CurrencyCode { get; set;}


    }
}