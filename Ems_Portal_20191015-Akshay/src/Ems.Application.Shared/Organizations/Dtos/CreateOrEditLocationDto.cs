using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ems.Organizations.Dtos
{
    public class CreateOrEditLocationDto : EntityDto<int?>
    {
        public string LocationName { get; set; }

        public long? UserId { get; set; }
    }
}
