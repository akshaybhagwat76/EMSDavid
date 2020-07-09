using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditWorkOrderActionDto : EntityDto<int?>
    {
        [Required]
        public string Action { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
