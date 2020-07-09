using Abp.Application.Services.Dto;
using Ems.Quotations.Dtos;

namespace Ems.Support.Dtos
{
    public class GetEstimateForViewDto
    {
		public EstimateDto Estimate { get; set; }

		public string WorkOrderSubject { get; set;}

		public string QuotationTitle { get; set; }

        public string EstimateStatusStatus { get; set;}

        public string CustomerName { get; set;}
    }
}