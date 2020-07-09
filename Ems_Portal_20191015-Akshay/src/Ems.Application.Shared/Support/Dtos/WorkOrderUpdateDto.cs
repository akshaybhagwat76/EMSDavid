using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class WorkOrderUpdateDto : EntityDto
    {
        //public DateTime UpdatedAt { get; set; }

        //public string Update { get; set; }

        //public bool Complete { get; set; }

        //public long UpdatedByUserId { get; set; }

        //public int? UomId { get; set; }

        public int WorkOrderId { get; set; }

        public int? ItemTypeId { get; set; }

        public int WorkOrderActionId { get; set; }

        public decimal Number { get; set; }

        public string Comments { get; set; }
    }
}