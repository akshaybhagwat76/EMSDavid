using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class GetWorkOrderForEditOutput
    {
		public CreateOrEditWorkOrderDto WorkOrder { get; set; }

		public string WorkOrderPriorityPriority { get; set;}

		public string WorkOrderTypeType { get; set;}

		public string VendorName { get; set;}

		public string IncidentDescription { get; set;}

		public string SupportItemDescription { get; set;}

		public string UserName { get; set;}

		public string CustomerName { get; set;}

		public string AssetOwnershipAssetDisplayName { get; set;}

		public string WorkOrderStatusStatus { get; set;}

        public string TenantType { get; set; }
    }
}