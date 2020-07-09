namespace Ems.Support.Dtos
{
    public class GetSupportItemForViewDto
    {
		public SupportItemDto SupportItem { get; set; }

		public string AssetReference { get; set;}

		public string AssetClassClass { get; set;}

		public string UomUnitOfMeasurement { get; set;}

		public string SupportContractTitle { get; set;}

		public string ConsumableTypeType { get; set;}

		public string SupportTypeType { get; set;}


    }
}