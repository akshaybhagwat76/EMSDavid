using Ems.Billing;
using Ems.Telematics;
using Ems.Organizations;
using Ems.Quotations;
using Ems.Support;
using Ems.Assets;
using Ems.Vendors;
using Ems.Customers;
using System;
using System.Linq;
using Abp.Organizations;
using Ems.Authorization.Roles;
using Ems.MultiTenancy;

namespace Ems.EntityHistory
{
    public static class EntityHistoryHelper
    {
        public const string EntityHistoryConfigurationName = "EntityHistory";

        public static readonly Type[] HostSideTrackedTypes =
        {
            typeof(CustomerInvoiceDetail),
            typeof(CustomerInvoice),
            typeof(Estimate),
            typeof(Address),
            typeof(WorkOrderUpdate),
            typeof(QuotationDetail),
            typeof(WorkOrder),
            typeof(Rfq),
            typeof(IncidentUpdate),
            typeof(Incident),
            typeof(SupportItem),
            typeof(BillingEventDetail),
            typeof(BillingEvent),
            typeof(VendorCharge),
            typeof(BillingRule),
            typeof(UsageMetric),
            typeof(LeaseItem),
            typeof(AssetOwnership),
            typeof(Asset),
            typeof(LeaseAgreement),
            typeof(Contact),
            typeof(Quotation),
            typeof(SupportContract),
            typeof(AssetOwner),
            typeof(Vendor),
            typeof(VendorChargeDetail),
            typeof(CustomerGroupMembership),
            typeof(CustomerGroup),
            typeof(Customer),
            typeof(CustomerType),
            typeof(OrganizationUnit), typeof(Role), typeof(Tenant)
        };

        public static readonly Type[] TenantSideTrackedTypes =
        {
            typeof(CustomerInvoiceDetail),
            typeof(CustomerInvoice),
            typeof(Estimate),
            typeof(Address),
            typeof(WorkOrderUpdate),
            typeof(QuotationDetail),
            typeof(WorkOrder),
            typeof(Rfq),
            typeof(IncidentUpdate),
            typeof(Incident),
            typeof(SupportItem),
            typeof(BillingEventDetail),
            typeof(BillingEvent),
            typeof(VendorCharge),
            typeof(BillingRule),
            typeof(UsageMetric),
            typeof(LeaseItem),
            typeof(AssetOwnership),
            typeof(Asset),
            typeof(LeaseAgreement),
            typeof(Contact),
            typeof(Quotation),
            typeof(SupportContract),
            typeof(AssetOwner),
            typeof(Vendor),
            typeof(VendorChargeDetail),
            typeof(CustomerGroupMembership),
            typeof(CustomerGroup),
            typeof(Customer),
            typeof(CustomerType),
            typeof(OrganizationUnit), typeof(Role)
        };

        public static readonly Type[] TrackedTypes =
            HostSideTrackedTypes
                .Concat(TenantSideTrackedTypes)
                .GroupBy(type => type.FullName)
                .Select(types => types.First())
                .ToArray();
    }
}
