using Ems.Quotations.Dtos;
using System.Collections.Generic;
using System.Text;


namespace Ems.Support.Dtos
{
    public class GetWorkorderUpdateItemsForViewDto
    {
        public ItemTypeDto Item { get; set; }

        public int Consumed { get; set; }
    }
}
