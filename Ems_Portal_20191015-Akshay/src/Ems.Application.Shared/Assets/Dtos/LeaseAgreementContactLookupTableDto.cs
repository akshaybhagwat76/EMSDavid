using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class LeaseAgreementContactLookupTableDto
    {
		public int Id { get; set; }

		public string DisplayName { get; set; }
    }

    public class LeaseAgreementCustomerAndAssetOwnerDto
    {
        public LeaseAgreementCustomerLookupTableDto CustomerInfo { get; set; }

        public LeaseAgreementAssetOwnerLookupTableDto AssetOwnerInfo { get; set; }
    }
}