using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class GetIncidentForEditOutput
    {
		public CreateOrEditIncidentDto Incident { get; set; }

		public string IncidentPriorityPriority { get; set;}

		public string IncidentStatusStatus { get; set;}

		public string CustomerName { get; set;}

		public string AssetReference { get; set;}

		public string SupportItemDescription { get; set;}

		public string IncidentTypeType { get; set;}

		public string UserName { get; set;}

        public string TenantType { get; set; }
    }
}