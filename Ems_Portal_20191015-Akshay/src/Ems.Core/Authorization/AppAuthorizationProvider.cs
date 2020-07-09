using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Ems.Authorization
{
    /// <summary>
    /// Application's authorization provider.
    /// Defines permissions for the application.
    /// See <see cref="AppPermissions"/> for all permission names.
    /// </summary>
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        public AppAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public AppAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {

            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var inventoryItems = pages.CreateChildPermission(AppPermissions.Pages_InventoryItems, L("InventoryItems"));
            inventoryItems.CreateChildPermission(AppPermissions.Pages_InventoryItems_Create, L("CreateNewInventoryItem"));
            inventoryItems.CreateChildPermission(AppPermissions.Pages_InventoryItems_Edit, L("EditInventoryItem"));
            inventoryItems.CreateChildPermission(AppPermissions.Pages_InventoryItems_Delete, L("DeleteInventoryItem"));



            var assetPartStatuses = pages.CreateChildPermission(AppPermissions.Pages_Main_AssetPartStatuses, L("AssetPartStatuses"));
            assetPartStatuses.CreateChildPermission(AppPermissions.Pages_Main_AssetPartStatuses_Create, L("CreateNewAssetPartStatus"));
            assetPartStatuses.CreateChildPermission(AppPermissions.Pages_Main_AssetPartStatuses_Edit, L("EditAssetPartStatus"));
            assetPartStatuses.CreateChildPermission(AppPermissions.Pages_Main_AssetPartStatuses_Delete, L("DeleteAssetPartStatus"));



            var assetParts = pages.CreateChildPermission(AppPermissions.Pages_Main_AssetParts, L("AssetParts"));
            assetParts.CreateChildPermission(AppPermissions.Pages_Main_AssetParts_Create, L("CreateNewAssetPart"));
            assetParts.CreateChildPermission(AppPermissions.Pages_Main_AssetParts_Edit, L("EditAssetPart"));
            assetParts.CreateChildPermission(AppPermissions.Pages_Main_AssetParts_Delete, L("DeleteAssetPart"));
            assetParts.CreateChildPermission(AppPermissions.Pages_Main_AssetParts_ManagePartTree, L("ManageComponentTree"));


            var assetPartTypes = pages.CreateChildPermission(AppPermissions.Pages_AssetPartTypes, L("AssetPartTypes"));
            assetPartTypes.CreateChildPermission(AppPermissions.Pages_AssetPartTypes_Create, L("CreateNewAssetPartType"));
            assetPartTypes.CreateChildPermission(AppPermissions.Pages_AssetPartTypes_Edit, L("EditAssetPartType"));
            assetPartTypes.CreateChildPermission(AppPermissions.Pages_AssetPartTypes_Delete, L("DeleteAssetPartType"));



            var agedReceivablesPeriods = pages.CreateChildPermission(AppPermissions.Pages_AgedReceivablesPeriods, L("AgedReceivablesPeriods"));
            agedReceivablesPeriods.CreateChildPermission(AppPermissions.Pages_AgedReceivablesPeriods_Create, L("CreateNewAgedReceivablesPeriod"));
            agedReceivablesPeriods.CreateChildPermission(AppPermissions.Pages_AgedReceivablesPeriods_Edit, L("EditAgedReceivablesPeriod"));
            agedReceivablesPeriods.CreateChildPermission(AppPermissions.Pages_AgedReceivablesPeriods_Delete, L("DeleteAgedReceivablesPeriod"));



            // PAGES ////////////////////

            var administration = pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));

            var xeroInvoices = administration.CreateChildPermission(AppPermissions.Pages_Administration_XeroInvoices, L("XeroInvoices"));
            xeroInvoices.CreateChildPermission(AppPermissions.Pages_Administration_XeroInvoices_Create, L("CreateNewXeroInvoice"));
            xeroInvoices.CreateChildPermission(AppPermissions.Pages_Administration_XeroInvoices_Edit, L("EditXeroInvoice"));
            xeroInvoices.CreateChildPermission(AppPermissions.Pages_Administration_XeroInvoices_Delete, L("DeleteXeroInvoice"));



            var crossTenantPermissions = administration.CreateChildPermission(AppPermissions.Pages_Administration_CrossTenantPermissions, L("CrossTenantPermissions"));
            crossTenantPermissions.CreateChildPermission(AppPermissions.Pages_Administration_CrossTenantPermissions_Create, L("CreateNewCrossTenantPermission"));
            crossTenantPermissions.CreateChildPermission(AppPermissions.Pages_Administration_CrossTenantPermissions_Edit, L("EditCrossTenantPermission"));
            crossTenantPermissions.CreateChildPermission(AppPermissions.Pages_Administration_CrossTenantPermissions_Delete, L("DeleteCrossTenantPermission"));



            var configuration = pages.CreateChildPermission(AppPermissions.Pages_Configuration, L("Configuration"));

            var main = pages.CreateChildPermission(AppPermissions.Pages_Main, L("Main"));

            pages.CreateChildPermission(AppPermissions.Pages_DemoUiComponents, L("DemoUiComponents"));


            // MAIN ////////////////////

            // Customer Invoices

            var customerInvoices = main.CreateChildPermission(AppPermissions.Pages_Main_CustomerInvoices, L("CustomerInvoices"));
            customerInvoices.CreateChildPermission(AppPermissions.Pages_Main_CustomerInvoices_Create, L("CreateNewCustomerInvoice"));
            customerInvoices.CreateChildPermission(AppPermissions.Pages_Main_CustomerInvoices_Edit, L("EditCustomerInvoice"));
            customerInvoices.CreateChildPermission(AppPermissions.Pages_Main_CustomerInvoices_Delete, L("DeleteCustomerInvoice"));
            customerInvoices.CreateChildPermission(AppPermissions.Pages_Main_CustomerInvoices_View, L("ViewCustomerInvoice"));
            customerInvoices.CreateChildPermission(AppPermissions.Pages_Main_CustomerInvoices_Submit, L("Submit"));

            customerInvoices.CreateChildPermission(AppPermissions.Pages_Main_CustomerInvoices_CreateCustomerInvoiceDetail, L("CreateNewCustomerInvoiceDetail"));
            customerInvoices.CreateChildPermission(AppPermissions.Pages_Main_CustomerInvoices_EditCustomerInvoiceDetail, L("EditCustomerInvoiceDetail"));
            customerInvoices.CreateChildPermission(AppPermissions.Pages_Main_CustomerInvoices_DeleteCustomerInvoiceDetail, L("DeleteCustomerInvoiceDetail"));

            // TODO - Sort the rest of these into neat groups

            var supportContracts = main.CreateChildPermission(AppPermissions.Pages_Main_SupportContracts, L("SupportContracts"));
            supportContracts.CreateChildPermission(AppPermissions.Pages_Main_SupportContracts_Create, L("CreateNewSupportContract"));
            supportContracts.CreateChildPermission(AppPermissions.Pages_Main_SupportContracts_Edit, L("EditSupportContract"));
            supportContracts.CreateChildPermission(AppPermissions.Pages_Main_SupportContracts_Delete, L("DeleteSupportContract"));
            supportContracts.CreateChildPermission(AppPermissions.Pages_Main_SupportContracts_View, L("ViewSupportContract"));
            supportContracts.CreateChildPermission(AppPermissions.Pages_Main_SupportContracts_SupportItemsCreate, L("CreateNewSupportItem"));
            supportContracts.CreateChildPermission(AppPermissions.Pages_Main_SupportContracts_SupportItemsEdit, L("EditSupportItem"));
            supportContracts.CreateChildPermission(AppPermissions.Pages_Main_SupportContracts_SupportItemsDelete, L("DeleteSupportItem"));


            var vendors = main.CreateChildPermission(AppPermissions.Pages_Main_Vendors, L("Vendors"));
            vendors.CreateChildPermission(AppPermissions.Pages_Main_Vendors_Profile, L("ViewVendorProfile"));
            vendors.CreateChildPermission(AppPermissions.Pages_Main_Vendors_View, L("ViewVendor"));
            vendors.CreateChildPermission(AppPermissions.Pages_Main_Vendors_Create, L("CreateNewVendor"));
            vendors.CreateChildPermission(AppPermissions.Pages_Main_Vendors_Edit, L("EditVendor"));
            vendors.CreateChildPermission(AppPermissions.Pages_Main_Vendors_Delete, L("DeleteVendor"));
            vendors.CreateChildPermission(AppPermissions.Pages_Main_Vendors_CreateAddress, L("CreateNewVendorAddress"));
            vendors.CreateChildPermission(AppPermissions.Pages_Main_Vendors_EditAddress, L("EditVendorAddress"));
            vendors.CreateChildPermission(AppPermissions.Pages_Main_Vendors_DeleteAddress, L("DeleteVendorAddress"));

            var estimates = main.CreateChildPermission(AppPermissions.Pages_Main_Estimates, L("Estimates"));
            estimates.CreateChildPermission(AppPermissions.Pages_Main_Estimates_Create, L("CreateNewEstimate"));
            estimates.CreateChildPermission(AppPermissions.Pages_Main_Estimates_Edit, L("EditEstimate"));
            estimates.CreateChildPermission(AppPermissions.Pages_Main_Estimates_Delete, L("DeleteEstimate"));
            estimates.CreateChildPermission(AppPermissions.Pages_Main_Estimates_View, L("ViewEstimate"));

            estimates.CreateChildPermission(AppPermissions.Pages_Main_Estimates_EstimateDetailsCreate, L("CreateNewEstimateDetail"));
            estimates.CreateChildPermission(AppPermissions.Pages_Main_Estimates_EstimateDetailsEdit, L("EditEstimateDetail"));
            estimates.CreateChildPermission(AppPermissions.Pages_Main_Estimates_EstimateDetailsDelete, L("DeleteEstimateDetail"));

            var attachments = main.CreateChildPermission(AppPermissions.Pages_Main_Attachments, L("Attachments"));
            attachments.CreateChildPermission(AppPermissions.Pages_Main_Attachments_Admin, L("AccessToAllAttachments"));
            attachments.CreateChildPermission(AppPermissions.Pages_Main_Attachments_Create, L("CreateNewAttachment"));
            attachments.CreateChildPermission(AppPermissions.Pages_Main_Attachments_Edit, L("EditAttachment"));
            attachments.CreateChildPermission(AppPermissions.Pages_Main_Attachments_Delete, L("DeleteAttachment"));
            attachments.CreateChildPermission(AppPermissions.Pages_Main_Attachments_View, L("ViewAttachment"));

            var quotations = main.CreateChildPermission(AppPermissions.Pages_Main_Quotations, L("Quotations"));
            quotations.CreateChildPermission(AppPermissions.Pages_Main_Quotations_View, L("ViewQuotation"));
            quotations.CreateChildPermission(AppPermissions.Pages_Main_Quotations_Create, L("CreateNewQuotation"));
            quotations.CreateChildPermission(AppPermissions.Pages_Main_Quotations_Edit, L("EditQuotation"));
            quotations.CreateChildPermission(AppPermissions.Pages_Main_Quotations_Delete, L("DeleteQuotation"));
            quotations.CreateChildPermission(AppPermissions.Pages_Main_Quotations_QuotationDetailsCreate, L("CreateNewQuotationDetail"));
            quotations.CreateChildPermission(AppPermissions.Pages_Main_Quotations_QuotationDetailsEdit, L("EditQuotationDetail"));
            quotations.CreateChildPermission(AppPermissions.Pages_Main_Quotations_QuotationDetailsDelete, L("DeleteQuotationDetail"));

            var workOrders = main.CreateChildPermission(AppPermissions.Pages_Main_WorkOrders, L("WorkOrders"));
            workOrders.CreateChildPermission(AppPermissions.Pages_Main_WorkOrders_View, L("ViewWorkOrder"));
            workOrders.CreateChildPermission(AppPermissions.Pages_Main_WorkOrders_Create, L("CreateNewWorkOrder"));
            workOrders.CreateChildPermission(AppPermissions.Pages_Main_WorkOrders_Edit, L("EditWorkOrder"));
            workOrders.CreateChildPermission(AppPermissions.Pages_Main_WorkOrders_Delete, L("DeleteWorkOrder"));

            workOrders.CreateChildPermission(AppPermissions.Pages_Main_WorkOrders_CreateWorkOrderUpdate, L("CreateNewWorkOrderUpdate"));
            workOrders.CreateChildPermission(AppPermissions.Pages_Main_WorkOrders_EditWorkOrderUpdate, L("EditWorkOrderUpdate"));
            workOrders.CreateChildPermission(AppPermissions.Pages_Main_WorkOrders_DeleteWorkOrderUpdate, L("DeleteWorkOrderUpdate"));

            var rfqs = main.CreateChildPermission(AppPermissions.Pages_Main_Rfqs, L("Rfqs"));
            rfqs.CreateChildPermission(AppPermissions.Pages_Main_Rfqs_Create, L("CreateNewRfq"));
            rfqs.CreateChildPermission(AppPermissions.Pages_Main_Rfqs_Edit, L("EditRfq"));
            rfqs.CreateChildPermission(AppPermissions.Pages_Main_Rfqs_Delete, L("DeleteRfq"));

            var incidents = main.CreateChildPermission(AppPermissions.Pages_Main_Incidents, L("Incidents"));
            incidents.CreateChildPermission(AppPermissions.Pages_Main_Incidents_Create, L("CreateNewIncident"));
            incidents.CreateChildPermission(AppPermissions.Pages_Main_Incidents_Edit, L("EditIncident"));
            incidents.CreateChildPermission(AppPermissions.Pages_Main_Incidents_Delete, L("DeleteIncident"));
            incidents.CreateChildPermission(AppPermissions.Pages_Main_Incidents_View, L("ViewIncident"));
            incidents.CreateChildPermission(AppPermissions.Pages_Main_Incidents_CreateIncidentUpdates, L("CreateNewIncidentUpdate"));
            incidents.CreateChildPermission(AppPermissions.Pages_Main_Incidents_EditIncidentUpdates, L("EditIncidentUpdate"));
            incidents.CreateChildPermission(AppPermissions.Pages_Main_Incidents_DeleteIncidentUpdates, L("DeleteIncidentUpdate"));

            var billingEvents = main.CreateChildPermission(AppPermissions.Pages_Main_BillingEvents, L("BillingEvents"));
            billingEvents.CreateChildPermission(AppPermissions.Pages_Main_BillingEvents_Create, L("CreateNewBillingEvent"));
            billingEvents.CreateChildPermission(AppPermissions.Pages_Main_BillingEvents_Edit, L("EditBillingEvent"));
            billingEvents.CreateChildPermission(AppPermissions.Pages_Main_BillingEvents_Delete, L("DeleteBillingEvent"));

            billingEvents.CreateChildPermission(AppPermissions.Pages_Main_BillingEvents_CreateEventDetails, L("CreateNewBillingEventDetail"));
            billingEvents.CreateChildPermission(AppPermissions.Pages_Main_BillingEvents_EditEventDetails, L("EditBillingEventDetail"));
            billingEvents.CreateChildPermission(AppPermissions.Pages_Main_BillingEvents_DeleteEventDetails, L("DeleteBillingEventDetail"));


            var vendorCharges = main.CreateChildPermission(AppPermissions.Pages_Main_VendorCharges, L("VendorCharges"));
            vendorCharges.CreateChildPermission(AppPermissions.Pages_Main_VendorCharges_Create, L("CreateNewVendorCharge"));
            vendorCharges.CreateChildPermission(AppPermissions.Pages_Main_VendorCharges_Edit, L("EditVendorCharge"));
            vendorCharges.CreateChildPermission(AppPermissions.Pages_Main_VendorCharges_Delete, L("DeleteVendorCharge"));
            vendorCharges.CreateChildPermission(AppPermissions.Pages_Main_VendorCharges_VendorChargeDetailsCreate, L("CreateNewVendorChargeDetail"));
            vendorCharges.CreateChildPermission(AppPermissions.Pages_Main_VendorCharges_VendorChargeDetailsEdit, L("EditVendorChargeDetail"));
            vendorCharges.CreateChildPermission(AppPermissions.Pages_Main_VendorCharges_VendorChargeDetailsDelete, L("DeleteVendorChargeDetail"));


            var billingRules = main.CreateChildPermission(AppPermissions.Pages_Main_BillingRules, L("BillingRules"));
            billingRules.CreateChildPermission(AppPermissions.Pages_Main_BillingRules_Create, L("CreateNewBillingRule"));
            billingRules.CreateChildPermission(AppPermissions.Pages_Main_BillingRules_Edit, L("EditBillingRule"));
            billingRules.CreateChildPermission(AppPermissions.Pages_Main_BillingRules_Delete, L("DeleteBillingRule"));

            var usageMetricRecords = main.CreateChildPermission(AppPermissions.Pages_Main_UsageMetricRecords, L("UsageMetricRecords"));
            usageMetricRecords.CreateChildPermission(AppPermissions.Pages_Main_UsageMetricRecords_Create, L("CreateNewUsageMetricRecord"));
            usageMetricRecords.CreateChildPermission(AppPermissions.Pages_Main_UsageMetricRecords_Edit, L("EditUsageMetricRecord"));
            usageMetricRecords.CreateChildPermission(AppPermissions.Pages_Main_UsageMetricRecords_Delete, L("DeleteUsageMetricRecord"));

            var usageMetrics = main.CreateChildPermission(AppPermissions.Pages_Main_UsageMetrics, L("UsageMetrics"));
            usageMetrics.CreateChildPermission(AppPermissions.Pages_Main_UsageMetrics_Create, L("CreateNewUsageMetric"));
            usageMetrics.CreateChildPermission(AppPermissions.Pages_Main_UsageMetrics_Edit, L("EditUsageMetric"));
            usageMetrics.CreateChildPermission(AppPermissions.Pages_Main_UsageMetrics_Delete, L("DeleteUsageMetric"));

            var assetOwnerships = main.CreateChildPermission(AppPermissions.Pages_Main_AssetOwnerships, L("AssetOwnerships"));
            assetOwnerships.CreateChildPermission(AppPermissions.Pages_Main_AssetOwnerships_Create, L("CreateNewAssetOwnership"));
            assetOwnerships.CreateChildPermission(AppPermissions.Pages_Main_AssetOwnerships_Edit, L("EditAssetOwnership"));
            assetOwnerships.CreateChildPermission(AppPermissions.Pages_Main_AssetOwnerships_Delete, L("DeleteAssetOwnership"));

            var assets = main.CreateChildPermission(AppPermissions.Pages_Main_Assets, L("Assets"));
            assets.CreateChildPermission(AppPermissions.Pages_Main_Assets_Create, L("CreateNewAsset"));
            assets.CreateChildPermission(AppPermissions.Pages_Main_Assets_Edit, L("EditAsset"));
            assets.CreateChildPermission(AppPermissions.Pages_Main_Assets_Delete, L("DeleteAsset"));
            assets.CreateChildPermission(AppPermissions.Pages_Main_Assets_View, L("ViewAsset"));

            assets.CreateChildPermission(AppPermissions.Pages_Main_Assets_CreateNotes, L("CreateNewAssetNotes"));
            assets.CreateChildPermission(AppPermissions.Pages_Main_Assets_EditNotes, L("EditAssetNotes"));
            assets.CreateChildPermission(AppPermissions.Pages_Main_Assets_DeleteNotes, L("DeleteAssetNotes"));

            var leaseAgreements = main.CreateChildPermission(AppPermissions.Pages_Main_LeaseAgreements, L("LeaseAgreements"));
            leaseAgreements.CreateChildPermission(AppPermissions.Pages_Main_LeaseAgreements_Create, L("CreateNewLeaseAgreement"));
            leaseAgreements.CreateChildPermission(AppPermissions.Pages_Main_LeaseAgreements_Edit, L("EditLeaseAgreement"));
            leaseAgreements.CreateChildPermission(AppPermissions.Pages_Main_LeaseAgreements_Delete, L("DeleteLeaseAgreement"));
            leaseAgreements.CreateChildPermission(AppPermissions.Pages_Main_LeaseAgreements_View, L("ViewLeaseAgreement"));
            leaseAgreements.CreateChildPermission(AppPermissions.Pages_Main_LeaseAgreements_LeaseItemsCreate, L("CreateNewLeaseItem"));
            leaseAgreements.CreateChildPermission(AppPermissions.Pages_Main_LeaseAgreements_LeaseItemsEdit, L("EditLeaseItem"));
            leaseAgreements.CreateChildPermission(AppPermissions.Pages_Main_LeaseAgreements_LeaseItemsDelete, L("DeleteLeaseItem"));

            var contacts = main.CreateChildPermission(AppPermissions.Pages_Main_Contacts, L("Contacts"));
            contacts.CreateChildPermission(AppPermissions.Pages_Main_Contacts_Create, L("CreateNewContact"));
            contacts.CreateChildPermission(AppPermissions.Pages_Main_Contacts_Edit, L("EditContact"));
            contacts.CreateChildPermission(AppPermissions.Pages_Main_Contacts_Delete, L("DeleteContact"));

            var assetOwners = main.CreateChildPermission(AppPermissions.Pages_Main_AssetOwners, L("AssetOwners"));
            assetOwners.CreateChildPermission(AppPermissions.Pages_Main_AssetOwners_Profile, L("ViewAssetOwnerProfile"));
            assetOwners.CreateChildPermission(AppPermissions.Pages_Main_AssetOwners_View, L("ViewAssetOwner"));
            assetOwners.CreateChildPermission(AppPermissions.Pages_Main_AssetOwners_Create, L("CreateNewAssetOwner"));
            assetOwners.CreateChildPermission(AppPermissions.Pages_Main_AssetOwners_Edit, L("EditAssetOwner"));
            assetOwners.CreateChildPermission(AppPermissions.Pages_Main_AssetOwners_Delete, L("DeleteAssetOwner"));
            assetOwners.CreateChildPermission(AppPermissions.Pages_Main_AssetOwners_CreateAddress, L("CreareNewAssetOwnerAddress"));
            assetOwners.CreateChildPermission(AppPermissions.Pages_Main_AssetOwners_EditAddress, L("EditAssetOwnerAddress"));
            assetOwners.CreateChildPermission(AppPermissions.Pages_Main_AssetOwners_DeleteAddress, L("DeleteAssetOwnerAddress"));

            var customers = main.CreateChildPermission(AppPermissions.Pages_Main_Customers, L("Customers"));
            customers.CreateChildPermission(AppPermissions.Pages_Main_Customers_Profile, L("ViewCustomerProfile"));
            customers.CreateChildPermission(AppPermissions.Pages_Main_Customers_View, L("ViewCustomer"));
            customers.CreateChildPermission(AppPermissions.Pages_Main_Customers_Create, L("CreateNewCustomer"));
            customers.CreateChildPermission(AppPermissions.Pages_Main_Customers_Edit, L("EditCustomer"));
            customers.CreateChildPermission(AppPermissions.Pages_Main_Customers_Delete, L("DeleteCustomer"));
            customers.CreateChildPermission(AppPermissions.Pages_Main_Customers_CreateAddress, L("CreateNewCustomerAddress"));
            customers.CreateChildPermission(AppPermissions.Pages_Main_Customers_EditAddress, L("EditCustomerAddress"));
            customers.CreateChildPermission(AppPermissions.Pages_Main_Customers_DeleteAddress, L("DeleteCustomerAddress"));

            var customerGroupMemberships = main.CreateChildPermission(AppPermissions.Pages_Main_CustomerGroupMemberships, L("CustomerGroupMemberships"));
            customerGroupMemberships.CreateChildPermission(AppPermissions.Pages_Main_CustomerGroupMemberships_Create, L("CreateNewCustomerGroupMembership"));
            customerGroupMemberships.CreateChildPermission(AppPermissions.Pages_Main_CustomerGroupMemberships_Edit, L("EditCustomerGroupMembership"));
            customerGroupMemberships.CreateChildPermission(AppPermissions.Pages_Main_CustomerGroupMemberships_Delete, L("DeleteCustomerGroupMembership"));

            var customerGroups = main.CreateChildPermission(AppPermissions.Pages_Main_CustomerGroups, L("CustomerGroups"));
            customerGroups.CreateChildPermission(AppPermissions.Pages_Main_CustomerGroups_Create, L("CreateNewCustomerGroup"));
            customerGroups.CreateChildPermission(AppPermissions.Pages_Main_CustomerGroups_Edit, L("EditCustomerGroup"));
            customerGroups.CreateChildPermission(AppPermissions.Pages_Main_CustomerGroups_Delete, L("DeleteCustomerGroup"));

            var addresses = main.CreateChildPermission(AppPermissions.Pages_Main_Addresses, L("Addresses"));
            addresses.CreateChildPermission(AppPermissions.Pages_Main_Addresses_Create, L("CreateNewAddress"));
            addresses.CreateChildPermission(AppPermissions.Pages_Main_Addresses_Edit, L("EditAddress"));
            addresses.CreateChildPermission(AppPermissions.Pages_Main_Addresses_Delete, L("DeleteAddress"));

            var assetClasses = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_AssetClasses, L("AssetClasses"));
            assetClasses.CreateChildPermission(AppPermissions.Pages_Configuration_AssetClasses_Create, L("CreateNewAssetClass"));
            assetClasses.CreateChildPermission(AppPermissions.Pages_Configuration_AssetClasses_Edit, L("EditAssetClass"));
            assetClasses.CreateChildPermission(AppPermissions.Pages_Configuration_AssetClasses_Delete, L("DeleteAssetClass"));

            

            // CONFIGURATION ////////////


            var vendorChargeStatuses = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_VendorChargeStatuses, L("VendorChargeStatuses"));
            vendorChargeStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_VendorChargeStatuses_Create, L("CreateNewVendorChargeStatus"));
            vendorChargeStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_VendorChargeStatuses_Edit, L("EditVendorChargeStatus"));
            vendorChargeStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_VendorChargeStatuses_Delete, L("DeleteVendorChargeStatus"));

            var workOrderStatuses = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderStatuses, L("WorkOrderStatuses"));
            workOrderStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderStatuses_Create, L("CreateNewWorkOrderStatus"));
            workOrderStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderStatuses_Edit, L("EditWorkOrderStatus"));
            workOrderStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderStatuses_Delete, L("DeleteWorkOrderStatus"));

            var workOrderActions = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderActions, L("WorkOrderActions"));
            workOrderActions.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderActions_Create, L("CreateNewWorkOrderAction"));
            workOrderActions.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderActions_Edit, L("EditWorkOrderAction"));
            workOrderActions.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderActions_Delete, L("DeleteWorkOrderAction"));

            var customerInvoiceStatuses = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_CustomerInvoiceStatuses, L("CustomerInvoiceStatuses"));
            customerInvoiceStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_CustomerInvoiceStatuses_Create, L("CreateNewCustomerInvoiceStatus"));
            customerInvoiceStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_CustomerInvoiceStatuses_Edit, L("EditCustomerInvoiceStatus"));
            customerInvoiceStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_CustomerInvoiceStatuses_Delete, L("DeleteCustomerInvoiceStatus"));

            var estimateStatuses = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_EstimateStatuses, L("EstimateStatuses"));
            estimateStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_EstimateStatuses_Create, L("CreateNewEstimateStatus"));
            estimateStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_EstimateStatuses_Edit, L("EditEstimateStatus"));
            estimateStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_EstimateStatuses_Delete, L("DeleteEstimateStatus"));

            var assetTypes = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_AssetTypes, L("AssetTypes"));
            assetTypes.CreateChildPermission(AppPermissions.Pages_Configuration_AssetTypes_Create, L("CreateNewAssetType"));
            assetTypes.CreateChildPermission(AppPermissions.Pages_Configuration_AssetTypes_Edit, L("EditAssetType"));
            assetTypes.CreateChildPermission(AppPermissions.Pages_Configuration_AssetTypes_Delete, L("DeleteAssetType"));

            var billingEventTypes = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_BillingEventTypes, L("BillingEventTypes"));
            billingEventTypes.CreateChildPermission(AppPermissions.Pages_Configuration_BillingEventTypes_Create, L("CreateNewBillingEventType"));
            billingEventTypes.CreateChildPermission(AppPermissions.Pages_Configuration_BillingEventTypes_Edit, L("EditBillingEventType"));
            billingEventTypes.CreateChildPermission(AppPermissions.Pages_Configuration_BillingEventTypes_Delete, L("DeleteBillingEventType"));

            var assetStatuses = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_AssetStatuses, L("AssetStatuses"));
            assetStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_AssetStatuses_Create, L("CreateNewAssetStatus"));
            assetStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_AssetStatuses_Edit, L("EditAssetStatus"));
            assetStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_AssetStatuses_Delete, L("DeleteAssetStatus"));

            var consumableTypes = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_ConsumableTypes, L("ConsumableTypes"));
            consumableTypes.CreateChildPermission(AppPermissions.Pages_Configuration_ConsumableTypes_Create, L("CreateNewConsumableType"));
            consumableTypes.CreateChildPermission(AppPermissions.Pages_Configuration_ConsumableTypes_Edit, L("EditConsumableType"));
            consumableTypes.CreateChildPermission(AppPermissions.Pages_Configuration_ConsumableTypes_Delete, L("DeleteConsumableType"));

            var workOrderPriorities = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderPriorities, L("WorkOrderPriorities"));
            workOrderPriorities.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderPriorities_Create, L("CreateNewWorkOrderPriority"));
            workOrderPriorities.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderPriorities_Edit, L("EditWorkOrderPriority"));
            workOrderPriorities.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderPriorities_Delete, L("DeleteWorkOrderPriority"));

            var rfqTypes = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_RfqTypes, L("RfqTypes"));
            rfqTypes.CreateChildPermission(AppPermissions.Pages_Configuration_RfqTypes_Create, L("CreateNewRfqType"));
            rfqTypes.CreateChildPermission(AppPermissions.Pages_Configuration_RfqTypes_Edit, L("EditRfqType"));
            rfqTypes.CreateChildPermission(AppPermissions.Pages_Configuration_RfqTypes_Delete, L("DeleteRfqType"));

            var incidentStatuses = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentStatuses, L("IncidentStatuses"));
            incidentStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentStatuses_Create, L("CreateNewIncidentStatus"));
            incidentStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentStatuses_Edit, L("EditIncidentStatus"));
            incidentStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentStatuses_Delete, L("DeleteIncidentStatus"));

            var incidentPriorities = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentPriorities, L("IncidentPriorities"));
            incidentPriorities.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentPriorities_Create, L("CreateNewIncidentPriority"));
            incidentPriorities.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentPriorities_Edit, L("EditIncidentPriority"));
            incidentPriorities.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentPriorities_Delete, L("DeleteIncidentPriority"));

            var billingRuleTypes = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_BillingRuleTypes, L("BillingRuleTypes"));
            billingRuleTypes.CreateChildPermission(AppPermissions.Pages_Configuration_BillingRuleTypes_Create, L("CreateNewBillingRuleType"));
            billingRuleTypes.CreateChildPermission(AppPermissions.Pages_Configuration_BillingRuleTypes_Edit, L("EditBillingRuleType"));
            billingRuleTypes.CreateChildPermission(AppPermissions.Pages_Configuration_BillingRuleTypes_Delete, L("DeleteBillingRuleType"));

            var supportTypes = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_SupportTypes, L("SupportTypes"));
            supportTypes.CreateChildPermission(AppPermissions.Pages_Configuration_SupportTypes_Create, L("CreateNewSupportType"));
            supportTypes.CreateChildPermission(AppPermissions.Pages_Configuration_SupportTypes_Edit, L("EditSupportType"));
            supportTypes.CreateChildPermission(AppPermissions.Pages_Configuration_SupportTypes_Delete, L("DeleteSupportType"));

            var quotationStatuses = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_QuotationStatuses, L("QuotationStatuses"));
            quotationStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_QuotationStatuses_Create, L("CreateNewQuotationStatus"));
            quotationStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_QuotationStatuses_Edit, L("EditQuotationStatus"));
            quotationStatuses.CreateChildPermission(AppPermissions.Pages_Configuration_QuotationStatuses_Delete, L("DeleteQuotationStatus"));

            var itemTypes = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_ItemTypes, L("ItemTypes"));
            itemTypes.CreateChildPermission(AppPermissions.Pages_Configuration_ItemTypes_Create, L("CreateNewItemType"));
            itemTypes.CreateChildPermission(AppPermissions.Pages_Configuration_ItemTypes_Edit, L("EditItemType"));
            itemTypes.CreateChildPermission(AppPermissions.Pages_Configuration_ItemTypes_Delete, L("DeleteItemType"));

            var currencies = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_Currencies, L("Currencies"));
            currencies.CreateChildPermission(AppPermissions.Pages_Configuration_Currencies_Create, L("CreateNewCurrency"));
            currencies.CreateChildPermission(AppPermissions.Pages_Configuration_Currencies_Edit, L("EditCurrency"));
            currencies.CreateChildPermission(AppPermissions.Pages_Configuration_Currencies_Delete, L("DeleteCurrency"));

            var workOrderTypes = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderTypes, L("WorkOrderTypes"));
            workOrderTypes.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderTypes_Create, L("CreateNewWorkOrderType"));
            workOrderTypes.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderTypes_Edit, L("EditWorkOrderType"));
            workOrderTypes.CreateChildPermission(AppPermissions.Pages_Configuration_WorkOrderTypes_Delete, L("DeleteWorkOrderType"));

            var customerTypes = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_CustomerTypes, L("CustomerTypes"));
            customerTypes.CreateChildPermission(AppPermissions.Pages_Configuration_CustomerTypes_Create, L("CreateNewCustomerType"));
            customerTypes.CreateChildPermission(AppPermissions.Pages_Configuration_CustomerTypes_Edit, L("EditCustomerType"));
            customerTypes.CreateChildPermission(AppPermissions.Pages_Configuration_CustomerTypes_Delete, L("DeleteCustomerType"));

            var ssicCodes = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_SsicCodes, L("SsicCodes"));
            ssicCodes.CreateChildPermission(AppPermissions.Pages_Configuration_SsicCodes_Create, L("CreateNewSsicCode"));
            ssicCodes.CreateChildPermission(AppPermissions.Pages_Configuration_SsicCodes_Edit, L("EditSsicCode"));
            ssicCodes.CreateChildPermission(AppPermissions.Pages_Configuration_SsicCodes_Delete, L("DeleteSsicCode"));

            var uoms = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_Uoms, L("Uoms"));
            uoms.CreateChildPermission(AppPermissions.Pages_Configuration_Uoms_Create, L("CreateNewUom"));
            uoms.CreateChildPermission(AppPermissions.Pages_Configuration_Uoms_Edit, L("EditUom"));
            uoms.CreateChildPermission(AppPermissions.Pages_Configuration_Uoms_Delete, L("DeleteUom"));

            var languages = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_Languages, L("Languages"));
            languages.CreateChildPermission(AppPermissions.Pages_Configuration_Languages_Create, L("CreatingNewLanguage"));
            languages.CreateChildPermission(AppPermissions.Pages_Configuration_Languages_Edit, L("EditingLanguage"));
            languages.CreateChildPermission(AppPermissions.Pages_Configuration_Languages_Delete, L("DeletingLanguages"));
            languages.CreateChildPermission(AppPermissions.Pages_Configuration_Languages_ChangeTexts, L("ChangingTexts"));


            // ADMINISTRATION ////////////////

            var roles = administration.CreateChildPermission(AppPermissions.Pages_Administration_Roles, L("Roles"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Create, L("CreatingNewRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Edit, L("EditingRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Delete, L("DeletingRole"));

            var users = administration.CreateChildPermission(AppPermissions.Pages_Administration_Users, L("Users"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Create, L("CreatingNewUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Delete, L("DeletingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Impersonation, L("LoginForUsers"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Unlock, L("Unlock"));

            var organizationUnits = administration.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits, L("OrganizationUnits"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree, L("ManagingOrganizationTree"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers, L("ManagingMembers"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageRoles, L("ManagingRoles"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_AuditLogs, L("AuditLogs"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_UiCustomization, L("VisualSettings"));


            // CONFIGURATION

            var incidentTypes = configuration.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentTypes, L("IncidentTypes"));
            incidentTypes.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentTypes_Create, L("CreateNewIncidentType"));
            incidentTypes.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentTypes_Edit, L("EditIncidentType"));
            incidentTypes.CreateChildPermission(AppPermissions.Pages_Configuration_IncidentTypes_Delete, L("DeleteIncidentType"));


            //TENANT-SPECIFIC PERMISSIONS

            pages.CreateChildPermission(AppPermissions.Pages_Main_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);
            pages.CreateChildPermission(AppPermissions.Pages_Main_Dashboard_Finance, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_SubscriptionManagement, L("Subscription"), multiTenancySides: MultiTenancySides.Tenant);

            //HOST-SPECIFIC PERMISSIONS

            var editions = pages.CreateChildPermission(AppPermissions.Pages_Administration_Editions, L("Editions"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Administration_Editions_Create, L("CreatingNewEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Administration_Editions_Edit, L("EditingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Administration_Editions_Delete, L("DeletingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Administration_Editions_MoveTenantsToAnotherEdition, L("MoveTenantsToAnotherEdition"), multiTenancySides: MultiTenancySides.Host);

            var tenants = pages.CreateChildPermission(AppPermissions.Pages_Administration_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Administration_Tenants_Create, L("CreatingNewTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Administration_Tenants_Edit, L("EditingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Administration_Tenants_ChangeFeatures, L("ChangingFeatures"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Administration_Tenants_Delete, L("DeletingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Administration_Tenants_Impersonation, L("LoginForTenants"), multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Host);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Maintenance, L("Maintenance"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Main_HangfireDashboard, L("HangfireDashboard"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, EmsConsts.LocalizationSourceName);
        }
    }
}
