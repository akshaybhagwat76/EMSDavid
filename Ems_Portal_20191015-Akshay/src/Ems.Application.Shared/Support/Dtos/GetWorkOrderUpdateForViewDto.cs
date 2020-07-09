namespace Ems.Support.Dtos
{
    public class GetWorkOrderUpdateForViewDto
    {
		public WorkOrderUpdateDto WorkOrderUpdate { get; set; }

		public string WorkOrderSubject { get; set;}

        public string ItemTypeType { get; set; }

		public string WorkOrderActionAction { get; set;}
    }
}