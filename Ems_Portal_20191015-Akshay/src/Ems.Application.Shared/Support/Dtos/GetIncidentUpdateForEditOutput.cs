using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class GetIncidentUpdateForEditOutput
    {
		public CreateOrEditIncidentUpdateDto IncidentUpdate { get; set; }

		public string UserName { get; set;}

		public string IncidentDescription { get; set;}


    }
}