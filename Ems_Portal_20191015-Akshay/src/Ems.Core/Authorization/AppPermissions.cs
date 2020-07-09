namespace Ems.Authorization
{
    /// <summary>
    /// Defines string constants for application's permission names.
    /// <see cref="AppAuthorizationProvider"/> for permission definitions.
    /// </summary>
    public static class AppPermissions
    {
        public const string Pages_Administration_XeroInvoices = "Pages.Administration.XeroInvoices";
        public const string Pages_Administration_XeroInvoices_Create = "Pages.Administration.XeroInvoices.Create";
        public const string Pages_Administration_XeroInvoices_Edit = "Pages.Administration.XeroInvoices.Edit";
        public const string Pages_Administration_XeroInvoices_Delete = "Pages.Administration.XeroInvoices.Delete";

        public const string Pages_InventoryItems = "Pages.InventoryItems";
        public const string Pages_InventoryItems_Create = "Pages.InventoryItems.Create";
        public const string Pages_InventoryItems_Edit = "Pages.InventoryItems.Edit";
        public const string Pages_InventoryItems_Delete = "Pages.InventoryItems.Delete";

        public const string Pages_Main_AssetPartStatuses = "Pages.Main.AssetPartStatuses";
        public const string Pages_Main_AssetPartStatuses_Create = "Pages.Main.AssetPartStatuses.Create";
        public const string Pages_Main_AssetPartStatuses_Edit = "Pages.Main.AssetPartStatuses.Edit";
        public const string Pages_Main_AssetPartStatuses_Delete = "Pages.Main.AssetPartStatuses.Delete";

        public const string Pages_Main_AssetParts = "Pages.Main.AssetParts";
        public const string Pages_Main_AssetParts_Create = "Pages.Main.AssetParts.Create";
        public const string Pages_Main_AssetParts_Edit = "Pages.Main.AssetParts.Edit";
        public const string Pages_Main_AssetParts_Delete = "Pages.Main.AssetParts.Delete";
        public const string Pages_Main_AssetParts_ManagePartTree = "Pages.Main.AssetParts.ManagePartTree";
        
        public const string Pages_AssetPartTypes = "Pages.AssetPartTypes";
        public const string Pages_AssetPartTypes_Create = "Pages.AssetPartTypes.Create";
        public const string Pages_AssetPartTypes_Edit = "Pages.AssetPartTypes.Edit";
        public const string Pages_AssetPartTypes_Delete = "Pages.AssetPartTypes.Delete";

        public const string Pages_AgedReceivablesPeriods = "Pages.AgedReceivablesPeriods";
        public const string Pages_AgedReceivablesPeriods_Create = "Pages.AgedReceivablesPeriods.Create";
        public const string Pages_AgedReceivablesPeriods_Edit = "Pages.AgedReceivablesPeriods.Edit";
        public const string Pages_AgedReceivablesPeriods_Delete = "Pages.AgedReceivablesPeriods.Delete";


        // Page Permissions Structure

        public const string Pages = "Pages";

        public const string Pages_Administration = "Pages.Administration";
        public const string Pages_Configuration = "Pages.Configuration";
        public const string Pages_Main = "Pages.Main";

        public const string Pages_DemoUiComponents = "Pages.DemoUiComponents";

        // MAIN //////////////

        // Dashboard

        public const string Pages_Main_HangfireDashboard = "Pages.Administration.HangfireDashboard";

        // Tenant Dashboard

        public const string Pages_Main_Dashboard = "Pages.Main.Dashboard";
        public const string Pages_Main_Dashboard_Finance = "Pages.Main.Dashboard.Finance";

        // Estimates

        public const string Pages_Main_Estimates = "Pages.Main.Estimates";
        public const string Pages_Main_Estimates_Create = "Pages.Main.Estimates.Create";
        public const string Pages_Main_Estimates_Edit = "Pages.Main.Estimates.Edit";
        public const string Pages_Main_Estimates_Delete = "Pages.Main.Estimates.Delete";
        public const string Pages_Main_Estimates_View = "Pages.Main.Estimates.View";

        public const string Pages_Main_Estimates_EstimateDetailsCreate = "Pages.Main.Estimates.EstimateDetailsCreate";
        public const string Pages_Main_Estimates_EstimateDetailsEdit = "Pages.Main.Estimates.EstimateDetailsEdit";
        public const string Pages_Main_Estimates_EstimateDetailsDelete = "Pages.Main.Estimates.EstimateDetailsDelete";

        // Attachments

        public const string Pages_Main_Attachments = "Pages.Main.Attachments";
        public const string Pages_Main_Attachments_Admin = "Pages.Main.Attachments.Admin";
        public const string Pages_Main_Attachments_Create = "Pages.Main.Attachments.Create";
        public const string Pages_Main_Attachments_Edit = "Pages.Main.Attachments.Edit";
        public const string Pages_Main_Attachments_Delete = "Pages.Main.Attachments.Delete";
        public const string Pages_Main_Attachments_View = "Pages.Main.Attachments.View";

        // Customer Invoices

        public const string Pages_Main_CustomerInvoices = "Pages.Main.CustomerInvoices";
        public const string Pages_Main_CustomerInvoices_Create = "Pages.Main.CustomerInvoices.Create";
        public const string Pages_Main_CustomerInvoices_Edit = "Pages.Main.CustomerInvoices.Edit";
        public const string Pages_Main_CustomerInvoices_Delete = "Pages.Main.CustomerInvoices.Delete";
        public const string Pages_Main_CustomerInvoices_View = "Pages.Main.CustomerInvoices.View";
        public const string Pages_Main_CustomerInvoices_Submit = "Pages.Main.CustomerInvoices.Submit";


        public const string Pages_Main_CustomerInvoices_CreateCustomerInvoiceDetail = "Pages.Main.CustomerInvoices.CreateCustomerInvoiceDetail";
        public const string Pages_Main_CustomerInvoices_EditCustomerInvoiceDetail = "Pages.Main.CustomerInvoices.EditCustomerInvoiceDetail";
        public const string Pages_Main_CustomerInvoices_DeleteCustomerInvoiceDetail = "Pages.Main.CustomerInvoices.DeleteCustomerInvoiceDetail";

        // WorkOrders

        public const string Pages_Main_WorkOrders = "Pages.Main.WorkOrders";
        public const string Pages_Main_WorkOrders_View = "Pages.Main.WorkOrders.View";
        public const string Pages_Main_WorkOrders_Create = "Pages.Main.WorkOrders.Create";
        public const string Pages_Main_WorkOrders_Edit = "Pages.Main.WorkOrders.Edit";
        public const string Pages_Main_WorkOrders_Delete = "Pages.Main.WorkOrders.Delete";

        public const string Pages_Main_WorkOrders_CreateWorkOrderUpdate = "Pages.Main.WorkOrders.CreateWorkOrderUpdate";
        public const string Pages_Main_WorkOrders_EditWorkOrderUpdate = "Pages.Main.WorkOrders.EditWorkOrderUpdate";
        public const string Pages_Main_WorkOrders_DeleteWorkOrderUpdate = "Pages.Main.WorkOrders.DeleteWorkOrderUpdate";

        // Quotations

        public const string Pages_Main_Quotations = "Pages.Main.Quotations";
        public const string Pages_Main_Quotations_View = "Pages.Main.Quotations.View";
        public const string Pages_Main_Quotations_Create = "Pages.Main.Quotations.Create";
        public const string Pages_Main_Quotations_Edit = "Pages.Main.Quotations.Edit";
        public const string Pages_Main_Quotations_Delete = "Pages.Main.Quotations.Delete";
        public const string Pages_Main_Quotations_QuotationDetailsCreate = "Pages.Main.Quotations.QuotationDetailsCreate";
        public const string Pages_Main_Quotations_QuotationDetailsEdit = "Pages.Main.Quotations.QuotationDetailsEdit";
        public const string Pages_Main_Quotations_QuotationDetailsDelete = "Pages.Main.Quotations.QuotationDetailsDelete";

        // RFQs

        public const string Pages_Main_Rfqs = "Pages.Main.Rfqs";
        public const string Pages_Main_Rfqs_Create = "Pages.Main.Rfqs.Create";
        public const string Pages_Main_Rfqs_Edit = "Pages.Main.Rfqs.Edit";
        public const string Pages_Main_Rfqs_Delete = "Pages.Main.Rfqs.Delete";

        // Incidents

        public const string Pages_Main_Incidents = "Pages.Main.Incidents";
        public const string Pages_Main_Incidents_Create = "Pages.Main.Incidents.Create";
        public const string Pages_Main_Incidents_Edit = "Pages.Main.Incidents.Edit";
        public const string Pages_Main_Incidents_Delete = "Pages.Main.Incidents.Delete";
        public const string Pages_Main_Incidents_View = "Pages.Main.Incidents.View";
        public const string Pages_Main_Incidents_CreateIncidentUpdates = "Pages.Main.Incidents.CreateIncidentUpdates";
        public const string Pages_Main_Incidents_EditIncidentUpdates = "Pages.Main.Incidents.EditIncidentUpdates";
        public const string Pages_Main_Incidents_DeleteIncidentUpdates = "Pages.Main.Incidents.DeleteIncidentUpdates";

        // Billing Events

        public const string Pages_Main_BillingEvents = "Pages.Main.BillingEvents";
        public const string Pages_Main_BillingEvents_Create = "Pages.Main.BillingEvents.Create";
        public const string Pages_Main_BillingEvents_Edit = "Pages.Main.BillingEvents.Edit";
        public const string Pages_Main_BillingEvents_Delete = "Pages.Main.BillingEvents.Delete";
        public const string Pages_Main_BillingEvents_CreateEventDetails = "Pages.Main.BillingEvents.CreateEventDetails";
        public const string Pages_Main_BillingEvents_EditEventDetails = "Pages.Main.BillingEvents.EditEventDetails";
        public const string Pages_Main_BillingEvents_DeleteEventDetails = "Pages.Main.BillingEvents.DeleteEventDetails";

        // Vendor Charges

        public const string Pages_Main_VendorCharges = "Pages.Main.VendorCharges";
        public const string Pages_Main_VendorCharges_Create = "Pages.Main.VendorCharges.Create";
        public const string Pages_Main_VendorCharges_Edit = "Pages.Main.VendorCharges.Edit";
        public const string Pages_Main_VendorCharges_Delete = "Pages.Main.VendorCharges.Delete";
        public const string Pages_Main_VendorCharges_VendorChargeDetailsCreate = "Pages.Main.VendorCharges.VendorChargeDetailsCreate";
        public const string Pages_Main_VendorCharges_VendorChargeDetailsEdit = "Pages.Main.VendorCharges.VendorChargeDetailsEdit";
        public const string Pages_Main_VendorCharges_VendorChargeDetailsDelete = "Pages.Main.VendorCharges.VendorChargeDetailsDelete";

        // Billing Rules

        public const string Pages_Main_BillingRules = "Pages.Main.BillingRules";
        public const string Pages_Main_BillingRules_Create = "Pages.Main.BillingRules.Create";
        public const string Pages_Main_BillingRules_Edit = "Pages.Main.BillingRules.Edit";
        public const string Pages_Main_BillingRules_Delete = "Pages.Main.BillingRules.Delete";

        // Usage Metrics

        public const string Pages_Main_UsageMetrics = "Pages.Main.UsageMetrics";
        public const string Pages_Main_UsageMetrics_Create = "Pages.Main.UsageMetrics.Create";
        public const string Pages_Main_UsageMetrics_Edit = "Pages.Main.UsageMetrics.Edit";
        public const string Pages_Main_UsageMetrics_Delete = "Pages.Main.UsageMetrics.Delete";

        public const string Pages_Main_UsageMetricRecords = "Pages.Main.UsageMetricRecords";
        public const string Pages_Main_UsageMetricRecords_Create = "Pages.Main.UsageMetricRecords.Create";
        public const string Pages_Main_UsageMetricRecords_Edit = "Pages.Main.UsageMetricRecords.Edit";
        public const string Pages_Main_UsageMetricRecords_Delete = "Pages.Main.UsageMetricRecords.Delete";

        // Lease Agreements

        public const string Pages_Main_LeaseAgreements = "Pages.Main.LeaseAgreements";
        public const string Pages_Main_LeaseAgreements_Create = "Pages.Main.LeaseAgreements.Create";
        public const string Pages_Main_LeaseAgreements_Edit = "Pages.Main.LeaseAgreements.Edit";
        public const string Pages_Main_LeaseAgreements_Delete = "Pages.Main.LeaseAgreements.Delete";
        public const string Pages_Main_LeaseAgreements_View = "Pages.Main.LeaseAgreements.View";
        public const string Pages_Main_LeaseAgreements_LeaseItemsCreate = "Pages.Main.LeaseAgreements.LeaseItemsCreate";
        public const string Pages_Main_LeaseAgreements_LeaseItemsEdit = "Pages.Main.LeaseAgreements.LeaseItemsEdit";
        public const string Pages_Main_LeaseAgreements_LeaseItemsDelete = "Pages.Main.LeaseAgreements.LeaseItemsDelete";

        // Asset Ownerships

        public const string Pages_Main_AssetOwnerships = "Pages.Main.AssetOwnerships";
        public const string Pages_Main_AssetOwnerships_Create = "Pages.Main.AssetOwnerships.Create";
        public const string Pages_Main_AssetOwnerships_Edit = "Pages.Main.AssetOwnerships.Edit";
        public const string Pages_Main_AssetOwnerships_Delete = "Pages.Main.AssetOwnerships.Delete";
        
        // Asset Owners

        public const string Pages_Main_AssetOwners = "Pages.Main.AssetOwners";
        public const string Pages_Main_AssetOwners_Profile = "Pages.Main.AssetOwners.Profile";
        public const string Pages_Main_AssetOwners_View = "Pages.Main.AssetOwners.View";
        public const string Pages_Main_AssetOwners_Create = "Pages.Main.AssetOwners.Create";
        public const string Pages_Main_AssetOwners_Edit = "Pages.Main.AssetOwners.Edit";
        public const string Pages_Main_AssetOwners_Delete = "Pages.Main.AssetOwners.Delete";
        public const string Pages_Main_AssetOwners_CreateAddress = "Pages.Main.AssetOwners.CreateAddress";
        public const string Pages_Main_AssetOwners_EditAddress = "Pages.Main.AssetOwners.EditAddress";
        public const string Pages_Main_AssetOwners_DeleteAddress = "Pages.Main.AssetOwners.DeleteAddress";

        // Assets

        public const string Pages_Main_Assets = "Pages.Main.Assets";
        public const string Pages_Main_Assets_Create = "Pages.Main.Assets.Create";
        public const string Pages_Main_Assets_Edit = "Pages.Main.Assets.Edit";
        public const string Pages_Main_Assets_Delete = "Pages.Main.Assets.Delete";
        public const string Pages_Main_Assets_View = "Pages.Main.Assets.View";
        public const string Pages_Main_Assets_CreateNotes = "Pages.Main.Assets.CreateNotes";
        public const string Pages_Main_Assets_EditNotes = "Pages.Main.Assets.EditNotes";
        public const string Pages_Main_Assets_DeleteNotes = "Pages.Main.Assets.DeleteNotes";



        // Contacts

        public const string Pages_Main_Contacts = "Pages.Main.Contacts";
        public const string Pages_Main_Contacts_Create = "Pages.Main.Contacts.Create";
        public const string Pages_Main_Contacts_Edit = "Pages.Main.Contacts.Edit";
        public const string Pages_Main_Contacts_Delete = "Pages.Main.Contacts.Delete";

        // Support Contracts

        public const string Pages_Main_SupportContracts = "Pages.Main.SupportContracts";
        public const string Pages_Main_SupportContracts_Create = "Pages.Main.SupportContracts.Create";
        public const string Pages_Main_SupportContracts_Edit = "Pages.Main.SupportContracts.Edit";
        public const string Pages_Main_SupportContracts_Delete = "Pages.Main.SupportContracts.Delete";
        public const string Pages_Main_SupportContracts_View = "Pages.Main.SupportContracts.View";
        public const string Pages_Main_SupportContracts_SupportItemsCreate = "Pages.Main.SupportContracts.SupportItemsCreate";
        public const string Pages_Main_SupportContracts_SupportItemsEdit = "Pages.Main.SupportContracts.SupportItemsEdit";
        public const string Pages_Main_SupportContracts_SupportItemsDelete = "Pages.Main.SupportContracts.SupportItemsDelete";

        // Vendors

        public const string Pages_Main_Vendors = "Pages.Main.Vendors";
        public const string Pages_Main_Vendors_Profile = "Pages.Main.Vendors.Profile";
        public const string Pages_Main_Vendors_View = "Pages.Main.Vendors.View";
        public const string Pages_Main_Vendors_Create = "Pages.Main.Vendors.Create";
        public const string Pages_Main_Vendors_Edit = "Pages.Main.Vendors.Edit";
        public const string Pages_Main_Vendors_Delete = "Pages.Main.Vendors.Delete";
        public const string Pages_Main_Vendors_CreateAddress = "Pages.Main.Vendors.CreateAddress";
        public const string Pages_Main_Vendors_EditAddress = "Pages.Main.Vendors.EditAddress";
        public const string Pages_Main_Vendors_DeleteAddress = "Pages.Main.Vendors.DeleteAddress";

        // Customer Groups

        public const string Pages_Main_CustomerGroups = "Pages.Main.CustomerGroups";
        public const string Pages_Main_CustomerGroups_Create = "Pages.Main.CustomerGroups.Create";
        public const string Pages_Main_CustomerGroups_Edit = "Pages.Main.CustomerGroups.Edit";
        public const string Pages_Main_CustomerGroups_Delete = "Pages.Main.CustomerGroups.Delete";

        public const string Pages_Main_CustomerGroupMemberships = "Pages.Main.CustomerGroupMemberships";
        public const string Pages_Main_CustomerGroupMemberships_Create = "Pages.Main.CustomerGroupMemberships.Create";
        public const string Pages_Main_CustomerGroupMemberships_Edit = "Pages.Main.CustomerGroupMemberships.Edit";
        public const string Pages_Main_CustomerGroupMemberships_Delete = "Pages.Main.CustomerGroupMemberships.Delete";

        // Customers

        public const string Pages_Main_Customers = "Pages.Main.Customers";
        public const string Pages_Main_Customers_Profile = "Pages.Main.Customers.Profile";
        public const string Pages_Main_Customers_View = "Pages.Main.Customers.View";
        public const string Pages_Main_Customers_Create = "Pages.Main.Customers.Create";
        public const string Pages_Main_Customers_Edit = "Pages.Main.Customers.Edit";
        public const string Pages_Main_Customers_Delete = "Pages.Main.Customers.Delete";
        public const string Pages_Main_Customers_CreateAddress = "Pages.Main.Customers.CreateAddress";
        public const string Pages_Main_Customers_EditAddress = "Pages.Main.Customers.EditAddress";
        public const string Pages_Main_Customers_DeleteAddress = "Pages.Main.Customers.DeleteAddress";

        // Addresses

        public const string Pages_Main_Addresses = "Pages.Main.Addresses";
        public const string Pages_Main_Addresses_Create = "Pages.Main.Addresses.Create";
        public const string Pages_Main_Addresses_Edit = "Pages.Main.Addresses.Edit";
        public const string Pages_Main_Addresses_Delete = "Pages.Main.Addresses.Delete";



        // CONFIGURATION ////////////

        // VendorChargeStatuses

        public const string Pages_Configuration_VendorChargeStatuses = "Pages.Configuration.VendorChargeStatuses";
        public const string Pages_Configuration_VendorChargeStatuses_Create = "Pages.Configuration.VendorChargeStatuses.Create";
        public const string Pages_Configuration_VendorChargeStatuses_Edit = "Pages.Configuration.VendorChargeStatuses.Edit";
        public const string Pages_Configuration_VendorChargeStatuses_Delete = "Pages.Configuration.VendorChargeStatuses.Delete";

        // WorkOrderStatuses

        public const string Pages_Configuration_WorkOrderStatuses = "Pages.Configuration.WorkOrderStatuses";
        public const string Pages_Configuration_WorkOrderStatuses_Create = "Pages.Configuration.WorkOrderStatuses.Create";
        public const string Pages_Configuration_WorkOrderStatuses_Edit = "Pages.Configuration.WorkOrderStatuses.Edit";
        public const string Pages_Configuration_WorkOrderStatuses_Delete = "Pages.Configuration.WorkOrderStatuses.Delete";

        // WorkOrderActions

        public const string Pages_Configuration_WorkOrderActions = "Pages.Configuration.WorkOrderActions";
        public const string Pages_Configuration_WorkOrderActions_Create = "Pages.Configuration.WorkOrderActions.Create";
        public const string Pages_Configuration_WorkOrderActions_Edit = "Pages.Configuration.WorkOrderActions.Edit";
        public const string Pages_Configuration_WorkOrderActions_Delete = "Pages.Configuration.WorkOrderActions.Delete";


        // CustomerInvoiceStatuses

        public const string Pages_Configuration_CustomerInvoiceStatuses = "Pages.Configuration.CustomerInvoiceStatuses";
        public const string Pages_Configuration_CustomerInvoiceStatuses_Create = "Pages.Configuration.CustomerInvoiceStatuses.Create";
        public const string Pages_Configuration_CustomerInvoiceStatuses_Edit = "Pages.Configuration.CustomerInvoiceStatuses.Edit";
        public const string Pages_Configuration_CustomerInvoiceStatuses_Delete = "Pages.Configuration.CustomerInvoiceStatuses.Delete";

        // Estimate Statuses

        public const string Pages_Configuration_EstimateStatuses = "Pages.Configuration.EstimateStatuses";
        public const string Pages_Configuration_EstimateStatuses_Create = "Pages.Configuration.EstimateStatuses.Create";
        public const string Pages_Configuration_EstimateStatuses_Edit = "Pages.Configuration.EstimateStatuses.Edit";
        public const string Pages_Configuration_EstimateStatuses_Delete = "Pages.Configuration.EstimateStatuses.Delete";
               
        // Asset Types

        public const string Pages_Configuration_AssetTypes = "Pages.Configuration.AssetTypes";
        public const string Pages_Configuration_AssetTypes_Create = "Pages.Configuration.AssetTypes.Create";
        public const string Pages_Configuration_AssetTypes_Edit = "Pages.Configuration.AssetTypes.Edit";
        public const string Pages_Configuration_AssetTypes_Delete = "Pages.Configuration.AssetTypes.Delete";

        // Work Order Priorities

        public const string Pages_Configuration_WorkOrderPriorities = "Pages.Configuration.WorkOrderPriorities";
        public const string Pages_Configuration_WorkOrderPriorities_Create = "Pages.Configuration.WorkOrderPriorities.Create";
        public const string Pages_Configuration_WorkOrderPriorities_Edit = "Pages.Configuration.WorkOrderPriorities.Edit";
        public const string Pages_Configuration_WorkOrderPriorities_Delete = "Pages.Configuration.WorkOrderPriorities.Delete";

        // RFQ Types

        public const string Pages_Configuration_RfqTypes = "Pages.Configuration.RfqTypes";
        public const string Pages_Configuration_RfqTypes_Create = "Pages.Configuration.RfqTypes.Create";
        public const string Pages_Configuration_RfqTypes_Edit = "Pages.Configuration.RfqTypes.Edit";
        public const string Pages_Configuration_RfqTypes_Delete = "Pages.Configuration.RfqTypes.Delete";

        // Incident Statues

        public const string Pages_Configuration_IncidentStatuses = "Pages.Configuration.IncidentStatuses";
        public const string Pages_Configuration_IncidentStatuses_Create = "Pages.Configuration.IncidentStatuses.Create";
        public const string Pages_Configuration_IncidentStatuses_Edit = "Pages.Configuration.IncidentStatuses.Edit";
        public const string Pages_Configuration_IncidentStatuses_Delete = "Pages.Configuration.IncidentStatuses.Delete";

        // Incident Priorites

        public const string Pages_Configuration_IncidentPriorities = "Pages.Configuration.IncidentPriorities";
        public const string Pages_Configuration_IncidentPriorities_Create = "Pages.Configuration.IncidentPriorities.Create";
        public const string Pages_Configuration_IncidentPriorities_Edit = "Pages.Configuration.IncidentPriorities.Edit";
        public const string Pages_Configuration_IncidentPriorities_Delete = "Pages.Configuration.IncidentPriorities.Delete";

        // Billing Rule Types

        public const string Pages_Configuration_BillingRuleTypes = "Pages.Configuration.BillingRuleTypes";
        public const string Pages_Configuration_BillingRuleTypes_Create = "Pages.Configuration.BillingRuleTypes.Create";
        public const string Pages_Configuration_BillingRuleTypes_Edit = "Pages.Configuration.BillingRuleTypes.Edit";
        public const string Pages_Configuration_BillingRuleTypes_Delete = "Pages.Configuration.BillingRuleTypes.Delete";

        // Billing Event Types

        public const string Pages_Configuration_BillingEventTypes = "Pages.Configuration.BillingEventTypes";
        public const string Pages_Configuration_BillingEventTypes_Create = "Pages.Configuration.BillingEventTypes.Create";
        public const string Pages_Configuration_BillingEventTypes_Edit = "Pages.Configuration.BillingEventTypes.Edit";
        public const string Pages_Configuration_BillingEventTypes_Delete = "Pages.Configuration.BillingEventTypes.Delete";

        // Asset Statuses

        public const string Pages_Configuration_AssetStatuses = "Pages.Configuration.AssetStatuses";
        public const string Pages_Configuration_AssetStatuses_Create = "Pages.Configuration.AssetStatuses.Create";
        public const string Pages_Configuration_AssetStatuses_Edit = "Pages.Configuration.AssetStatuses.Edit";
        public const string Pages_Configuration_AssetStatuses_Delete = "Pages.Configuration.AssetStatuses.Delete";

        // Consumable Types

        public const string Pages_Configuration_ConsumableTypes = "Pages.Configuration.ConsumableTypes";
        public const string Pages_Configuration_ConsumableTypes_Create = "Pages.Configuration.ConsumableTypes.Create";
        public const string Pages_Configuration_ConsumableTypes_Edit = "Pages.Configuration.ConsumableTypes.Edit";
        public const string Pages_Configuration_ConsumableTypes_Delete = "Pages.Configuration.ConsumableTypes.Delete";

        // Support Types

        public const string Pages_Configuration_SupportTypes = "Pages.Configuration.SupportTypes";
        public const string Pages_Configuration_SupportTypes_Create = "Pages.Configuration.SupportTypes.Create";
        public const string Pages_Configuration_SupportTypes_Edit = "Pages.Configuration.SupportTypes.Edit";
        public const string Pages_Configuration_SupportTypes_Delete = "Pages.Configuration.SupportTypes.Delete";

        // Quotation Statuses

        public const string Pages_Configuration_QuotationStatuses = "Pages.Configuration.QuotationStatuses";
        public const string Pages_Configuration_QuotationStatuses_Create = "Pages.Configuration.QuotationStatuses.Create";
        public const string Pages_Configuration_QuotationStatuses_Edit = "Pages.Configuration.QuotationStatuses.Edit";
        public const string Pages_Configuration_QuotationStatuses_Delete = "Pages.Configuration.QuotationStatuses.Delete";

        // Item Types

        public const string Pages_Configuration_ItemTypes = "Pages.Configuration.ItemTypes";
        public const string Pages_Configuration_ItemTypes_Create = "Pages.Configuration.ItemTypes.Create";
        public const string Pages_Configuration_ItemTypes_Edit = "Pages.Configuration.ItemTypes.Edit";
        public const string Pages_Configuration_ItemTypes_Delete = "Pages.Configuration.ItemTypes.Delete";

        // Currencies

        public const string Pages_Configuration_Currencies = "Pages.Configuration.Currencies";
        public const string Pages_Configuration_Currencies_Create = "Pages.Configuration.Currencies.Create";
        public const string Pages_Configuration_Currencies_Edit = "Pages.Configuration.Currencies.Edit";
        public const string Pages_Configuration_Currencies_Delete = "Pages.Configuration.Currencies.Delete";

        // Work Order Types

        public const string Pages_Configuration_WorkOrderTypes = "Pages.Configuration.WorkOrderTypes";
        public const string Pages_Configuration_WorkOrderTypes_Create = "Pages.Configuration.WorkOrderTypes.Create";
        public const string Pages_Configuration_WorkOrderTypes_Edit = "Pages.Configuration.WorkOrderTypes.Edit";
        public const string Pages_Configuration_WorkOrderTypes_Delete = "Pages.Configuration.WorkOrderTypes.Delete";

        // Customer Types

        public const string Pages_Configuration_CustomerTypes = "Pages.Configuration.CustomerTypes";
        public const string Pages_Configuration_CustomerTypes_Create = "Pages.Configuration.CustomerTypes.Create";
        public const string Pages_Configuration_CustomerTypes_Edit = "Pages.Configuration.CustomerTypes.Edit";
        public const string Pages_Configuration_CustomerTypes_Delete = "Pages.Configuration.CustomerTypes.Delete";

        // SSIC Codes

        public const string Pages_Configuration_SsicCodes = "Pages.Configuration.SsicCodes";
        public const string Pages_Configuration_SsicCodes_Create = "Pages.Configuration.SsicCodes.Create";
        public const string Pages_Configuration_SsicCodes_Edit = "Pages.Configuration.SsicCodes.Edit";
        public const string Pages_Configuration_SsicCodes_Delete = "Pages.Configuration.SsicCodes.Delete";

        // UOMs

        public const string Pages_Configuration_Uoms = "Pages.Configuration.Uoms";
        public const string Pages_Configuration_Uoms_Create = "Pages.Configuration.Uoms.Create";
        public const string Pages_Configuration_Uoms_Edit = "Pages.Configuration.Uoms.Edit";
        public const string Pages_Configuration_Uoms_Delete = "Pages.Configuration.Uoms.Delete";

        // Incident Types

        public const string Pages_Configuration_IncidentTypes = "Pages.Configuration.IncidentTypes";
        public const string Pages_Configuration_IncidentTypes_Create = "Pages.Configuration.IncidentTypes.Create";
        public const string Pages_Configuration_IncidentTypes_Edit = "Pages.Configuration.IncidentTypes.Edit";
        public const string Pages_Configuration_IncidentTypes_Delete = "Pages.Configuration.IncidentTypes.Delete";

        // Asset Classes

        public const string Pages_Configuration_AssetClasses = "Pages.Configuration.AssetClasses";
        public const string Pages_Configuration_AssetClasses_Create = "Pages.Configuration.AssetClasses.Create";
        public const string Pages_Configuration_AssetClasses_Edit = "Pages.Configuration.AssetClasses.Edit";
        public const string Pages_Configuration_AssetClasses_Delete = "Pages.Configuration.AssetClasses.Delete";

        
        // Languages

        public const string Pages_Configuration_Languages = "Pages.Configuration.Languages";
        public const string Pages_Configuration_Languages_Create = "Pages.Configuration.Languages.Create";
        public const string Pages_Configuration_Languages_Edit = "Pages.Configuration.Languages.Edit";
        public const string Pages_Configuration_Languages_Delete = "Pages.Configuration.Languages.Delete";
        public const string Pages_Configuration_Languages_ChangeTexts = "Pages.Configuration.Languages.ChangeTexts";



        // ADMINISTRATION /////////////

        // Cross Tenant Permissions

        public const string Pages_Administration_CrossTenantPermissions = "Pages.Administration.CrossTenantPermissions";
        public const string Pages_Administration_CrossTenantPermissions_Create = "Pages.Administration.CrossTenantPermissions.Create";
        public const string Pages_Administration_CrossTenantPermissions_Edit = "Pages.Administration.CrossTenantPermissions.Edit";
        public const string Pages_Administration_CrossTenantPermissions_Delete = "Pages.Administration.CrossTenantPermissions.Delete";

        // Roles

        public const string Pages_Administration_Roles = "Pages.Administration.Roles";
        public const string Pages_Administration_Roles_Create = "Pages.Administration.Roles.Create";
        public const string Pages_Administration_Roles_Edit = "Pages.Administration.Roles.Edit";
        public const string Pages_Administration_Roles_Delete = "Pages.Administration.Roles.Delete";

        // Users

        public const string Pages_Administration_Users = "Pages.Administration.Users";
        public const string Pages_Administration_Users_Create = "Pages.Administration.Users.Create";
        public const string Pages_Administration_Users_Edit = "Pages.Administration.Users.Edit";
        public const string Pages_Administration_Users_Delete = "Pages.Administration.Users.Delete";
        public const string Pages_Administration_Users_ChangePermissions = "Pages.Administration.Users.ChangePermissions";
        public const string Pages_Administration_Users_Impersonation = "Pages.Administration.Users.Impersonation";
        public const string Pages_Administration_Users_Unlock = "Pages.Administration.Users.Unlock";

        // Ausit Logs

        public const string Pages_Administration_AuditLogs = "Pages.Administration.AuditLogs";

        // Organisation Units

        public const string Pages_Administration_OrganizationUnits = "Pages.Administration.OrganizationUnits";
        public const string Pages_Administration_OrganizationUnits_ManageOrganizationTree = "Pages.Administration.OrganizationUnits.ManageOrganizationTree";
        public const string Pages_Administration_OrganizationUnits_ManageMembers = "Pages.Administration.OrganizationUnits.ManageMembers";
        public const string Pages_Administration_OrganizationUnits_ManageRoles = "Pages.Administration.OrganizationUnits.ManageRoles";

        // UI Customizations

        public const string Pages_Administration_UiCustomization = "Pages.Administration.UiCustomization";

        // Editions

        public const string Pages_Administration_Editions = "Pages.Administration.Editions";
        public const string Pages_Administration_Editions_Create = "Pages.Administration.Editions.Create";
        public const string Pages_Administration_Editions_Edit = "Pages.Administration.Editions.Edit";
        public const string Pages_Administration_Editions_Delete = "Pages.Administration.Editions.Delete";
        public const string Pages_Administration_Editions_MoveTenantsToAnotherEdition = "Pages.Administration.Editions.MoveTenantsToAnotherEdition";

        // Tenants

        public const string Pages_Administration_Tenants = "Pages.Administration.Tenants";
        public const string Pages_Administration_Tenants_Create = "Pages.Administration.Tenants.Create";
        public const string Pages_Administration_Tenants_Edit = "Pages.Administration.Tenants.Edit";
        public const string Pages_Administration_Tenants_ChangeFeatures = "Pages.Administration.Tenants.ChangeFeatures";
        public const string Pages_Administration_Tenants_Delete = "Pages.Administration.Tenants.Delete";
        public const string Pages_Administration_Tenants_Impersonation = "Pages.Administration.Tenants.Impersonation";

        // Tenant Specific Pages //////////////

        public const string Pages_Administration_Tenant_Settings = "Pages.Administration.Tenant.Settings";
        public const string Pages_Administration_Tenant_SubscriptionManagement = "Pages.Administration.Tenant.SubscriptionManagement";

        // Host Specific Pages /////////////////

        public const string Pages_Administration_Host_Maintenance = "Pages.Administration.Host.Maintenance";
        public const string Pages_Administration_Host_Settings = "Pages.Administration.Host.Settings";
        public const string Pages_Administration_Host_Dashboard = "Pages.Administration.Host.Dashboard";

    }
}