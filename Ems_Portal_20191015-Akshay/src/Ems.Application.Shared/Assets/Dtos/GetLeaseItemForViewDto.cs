namespace Ems.Assets.Dtos
{
    public class GetLeaseItemForViewDto
    {
		public LeaseItemDto LeaseItem { get; set; }

		public string AssetClassClass { get; set;}

		public string AssetReference { get; set;}

		public string LeaseAgreementTitle { get; set;}

        public string DepositUom { get; set;}

        public string RentalUom { get; set;}
    }
}