
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditWorkOrderUpdateDto : EntityDto<int?>
    {
        //public DateTime UpdatedAt { get; set; }

        //[Required]
        //public string Update { get; set; }

        //public bool Complete { get; set; }

        //public long UpdatedByUserId { get; set; }

        //public virtual int? UomId { get; set; }

        public int WorkOrderId { get; set; }

        public virtual decimal Number { get; set; }

        public virtual int? ItemTypeId { get; set; }

        public virtual int WorkOrderActionId { get; set; }

        public virtual string Comments { get; set; }
    }
}