using System;
using System.Collections.Generic;
using System.Text;

namespace Ems.Support.Dtos
{
    public class GetWorkorderItemsForViewDto
    {
        public SupportItemDto SupportItem { get; set; }

        public int Consumed { get; set; }

    }
}
