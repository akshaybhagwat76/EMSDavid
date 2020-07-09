using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Customers.Dtos
{
    public class GetCustomerTypeForEditOutput
    {
		public CreateOrEditCustomerTypeDto CustomerType { get; set; }


    }
}