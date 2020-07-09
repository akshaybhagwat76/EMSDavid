using Ems.Finance.Dtos;
using Ems.Finance;
using Ems.Authorization.Dtos;
using Ems.Authorization;
using Ems.Storage.Dtos;
using Ems.Storage;
using Ems.Telematics.Dtos;
using Ems.Telematics;
using Ems.Support.Dtos;
using Ems.Support;
using Ems.Quotations.Dtos;
using Ems.Quotations;
using Ems.Billing.Dtos;
using Ems.Billing;
using Ems.Vendors.Dtos;
using Ems.Vendors;
using Ems.Customers.Dtos;
using Ems.Customers;
using Ems.Metrics.Dtos;
using Ems.Metrics;
using Ems.Organizations.Dtos;
using Ems.Organizations;
using Ems.Assets.Dtos;
using Ems.Assets;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.EntityHistory;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.UI.Inputs;
using AutoMapper;
using Ems.Auditing.Dto;
using Ems.Authorization.Accounts.Dto;
using Ems.Authorization.Permissions.Dto;
using Ems.Authorization.Roles;
using Ems.Authorization.Roles.Dto;
using Ems.Authorization.Users;
using Ems.Authorization.Users.Dto;
using Ems.Authorization.Users.Importing.Dto;
using Ems.Authorization.Users.Profile.Dto;
using Ems.Chat;
using Ems.Chat.Dto;
using Ems.Editions;
using Ems.Editions.Dto;
using Ems.Friendships;
using Ems.Friendships.Cache;
using Ems.Friendships.Dto;
using Ems.Localization.Dto;
using Ems.MultiTenancy;
using Ems.MultiTenancy.Dto;
using Ems.MultiTenancy.HostDashboard.Dto;
using Ems.MultiTenancy.Payments;
using Ems.MultiTenancy.Payments.Dto;
using Ems.Notifications.Dto;
using Ems.Organizations.Dto;
using Ems.Sessions.Dto;

namespace Ems
{
    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CreateOrEditXeroInvoiceDto, XeroInvoice>().ReverseMap();
            configuration.CreateMap<XeroInvoiceDto, XeroInvoice>().ReverseMap();
            configuration.CreateMap<CreateOrEditInventoryItemDto, InventoryItem>().ReverseMap();
            configuration.CreateMap<InventoryItemDto, InventoryItem>().ReverseMap();
            configuration.CreateMap<CreateOrEditAssetPartStatusDto, AssetPartStatus>().ReverseMap();
            configuration.CreateMap<AssetPartStatusDto, AssetPartStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditAssetPartDto, AssetPart>().ReverseMap();
            configuration.CreateMap<AssetPartDto, AssetPart>().ReverseMap();
            configuration.CreateMap<CreateOrEditAssetPartTypeDto, AssetPartType>().ReverseMap();
            configuration.CreateMap<AssetPartTypeDto, AssetPartType>().ReverseMap();
            configuration.CreateMap<CreateOrEditAgedReceivablesPeriodDto, AgedReceivablesPeriod>().ReverseMap();
            configuration.CreateMap<AgedReceivablesPeriodDto, AgedReceivablesPeriod>().ReverseMap();
            configuration.CreateMap<CreateOrEditCrossTenantPermissionDto, CrossTenantPermission>().ReverseMap();
            configuration.CreateMap<CrossTenantPermissionDto, CrossTenantPermission>().ReverseMap();
            configuration.CreateMap<CreateOrEditVendorChargeStatusDto, VendorChargeStatus>().ReverseMap();
            configuration.CreateMap<VendorChargeStatusDto, VendorChargeStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditWorkOrderStatusDto, WorkOrderStatus>().ReverseMap();
            configuration.CreateMap<WorkOrderStatusDto, WorkOrderStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditWorkOrderActionDto, WorkOrderAction>().ReverseMap();
            configuration.CreateMap<WorkOrderActionDto, WorkOrderAction>().ReverseMap();
            configuration.CreateMap<CreateOrEditCustomerInvoiceDetailDto, CustomerInvoiceDetail>().ReverseMap();
            configuration.CreateMap<CustomerInvoiceDetailDto, CustomerInvoiceDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditCustomerInvoiceDto, CustomerInvoice>().ReverseMap();
            configuration.CreateMap<CustomerInvoiceDto, CustomerInvoice>().ReverseMap();
            configuration.CreateMap<CreateOrEditCustomerInvoiceStatusDto, CustomerInvoiceStatus>().ReverseMap();
            configuration.CreateMap<CustomerInvoiceStatusDto, CustomerInvoiceStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditEstimateDto, Estimate>().ReverseMap();
            configuration.CreateMap<EstimateDto, Estimate>().ReverseMap();
            configuration.CreateMap<CreateOrEditEstimateDetailDto, EstimateDetail>().ReverseMap();
            configuration.CreateMap<EstimateDetailDto, EstimateDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditEstimateStatusDto, EstimateStatus>().ReverseMap();
            configuration.CreateMap<EstimateStatusDto, EstimateStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditAttachmentDto, Attachment>().ReverseMap();
            configuration.CreateMap<AttachmentDto, Attachment>().ReverseMap();
            configuration.CreateMap<CreateOrEditWorkOrderUpdateDto, WorkOrderUpdate>().ReverseMap();
            configuration.CreateMap<WorkOrderUpdateDto, WorkOrderUpdate>().ReverseMap();
            configuration.CreateMap<CreateOrEditQuotationDetailDto, QuotationDetail>().ReverseMap();
            configuration.CreateMap<QuotationDetailDto, QuotationDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditWorkOrderDto, WorkOrder>().ReverseMap();
            configuration.CreateMap<WorkOrderDto, WorkOrder>().ReverseMap();
            configuration.CreateMap<CreateOrEditWorkOrderTypeDto, WorkOrderType>().ReverseMap();
            configuration.CreateMap<WorkOrderTypeDto, WorkOrderType>().ReverseMap();
            configuration.CreateMap<CreateOrEditWorkOrderPriorityDto, WorkOrderPriority>().ReverseMap();
            configuration.CreateMap<WorkOrderPriorityDto, WorkOrderPriority>().ReverseMap();
            configuration.CreateMap<CreateOrEditRfqDto, Rfq>().ReverseMap();
            configuration.CreateMap<RfqDto, Rfq>().ReverseMap();
            configuration.CreateMap<CreateOrEditRfqTypeDto, RfqType>().ReverseMap();
            configuration.CreateMap<RfqTypeDto, RfqType>().ReverseMap();
            configuration.CreateMap<CreateOrEditIncidentUpdateDto, IncidentUpdate>().ReverseMap();
            configuration.CreateMap<IncidentUpdateDto, IncidentUpdate>().ReverseMap();
            configuration.CreateMap<CreateOrEditIncidentDto, Incident>().ReverseMap();
            configuration.CreateMap<IncidentDto, Incident>().ReverseMap();
            configuration.CreateMap<CreateOrEditIncidentTypeDto, IncidentType>().ReverseMap();
            configuration.CreateMap<IncidentTypeDto, IncidentType>().ReverseMap();
            configuration.CreateMap<CreateOrEditIncidentStatusDto, IncidentStatus>().ReverseMap();
            configuration.CreateMap<IncidentStatusDto, IncidentStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditIncidentPriorityDto, IncidentPriority>().ReverseMap();
            configuration.CreateMap<IncidentPriorityDto, IncidentPriority>().ReverseMap();
            configuration.CreateMap<CreateOrEditSupportItemDto, SupportItem>().ReverseMap();
            configuration.CreateMap<SupportItemDto, SupportItem>().ReverseMap();
            configuration.CreateMap<CreateOrEditBillingEventDetailDto, BillingEventDetail>().ReverseMap();
            configuration.CreateMap<BillingEventDetailDto, BillingEventDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditBillingEventDto, BillingEvent>().ReverseMap();
            configuration.CreateMap<BillingEventDto, BillingEvent>().ReverseMap();
            configuration.CreateMap<CreateOrEditVendorChargeDto, VendorCharge>().ReverseMap();
            configuration.CreateMap<VendorChargeDto, VendorCharge>().ReverseMap();
            configuration.CreateMap<CreateOrEditBillingRuleDto, BillingRule>().ReverseMap();
            configuration.CreateMap<BillingRuleDto, BillingRule>().ReverseMap();
            configuration.CreateMap<CreateOrEditUsageMetricRecordDto, UsageMetricRecord>().ReverseMap();
            configuration.CreateMap<UsageMetricRecordDto, UsageMetricRecord>().ReverseMap();
            configuration.CreateMap<CreateOrEditUsageMetricDto, UsageMetric>().ReverseMap();
            configuration.CreateMap<UsageMetricDto, UsageMetric>().ReverseMap();
            configuration.CreateMap<CreateOrEditLeaseItemDto, LeaseItem>().ReverseMap();
            configuration.CreateMap<LeaseItemDto, LeaseItem>().ReverseMap();
            configuration.CreateMap<CreateOrEditAssetOwnershipDto, AssetOwnership>().ReverseMap();
            configuration.CreateMap<AssetOwnershipDto, AssetOwnership>().ReverseMap();
            configuration.CreateMap<CreateOrEditAssetDto, Asset>().ReverseMap();
            configuration.CreateMap<AssetDto, Asset>().ReverseMap();
            configuration.CreateMap<CreateOrEditLeaseAgreementDto, LeaseAgreement>().ReverseMap();
            configuration.CreateMap<LeaseAgreementDto, LeaseAgreement>().ReverseMap();
            configuration.CreateMap<CreateOrEditContactDto, Contact>().ReverseMap();
            configuration.CreateMap<ContactDto, Contact>().ReverseMap();
            configuration.CreateMap<CreateOrEditQuotationDto, Quotation>().ReverseMap();
            configuration.CreateMap<QuotationDto, Quotation>().ReverseMap();
            configuration.CreateMap<CreateOrEditBillingRuleTypeDto, BillingRuleType>().ReverseMap();
            configuration.CreateMap<BillingRuleTypeDto, BillingRuleType>().ReverseMap();
            configuration.CreateMap<CreateOrEditBillingEventTypeDto, BillingEventType>().ReverseMap();
            configuration.CreateMap<BillingEventTypeDto, BillingEventType>().ReverseMap();
            configuration.CreateMap<CreateOrEditAssetStatusDto, AssetStatus>().ReverseMap();
            configuration.CreateMap<AssetStatusDto, AssetStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditConsumableTypeDto, ConsumableType>().ReverseMap();
            configuration.CreateMap<ConsumableTypeDto, ConsumableType>().ReverseMap();
            configuration.CreateMap<CreateOrEditSupportTypeDto, SupportType>().ReverseMap();
            configuration.CreateMap<SupportTypeDto, SupportType>().ReverseMap();
            configuration.CreateMap<CreateOrEditSupportContractDto, SupportContract>().ReverseMap();
            configuration.CreateMap<SupportContractDto, SupportContract>().ReverseMap();
            configuration.CreateMap<CreateOrEditQuotationStatusDto, QuotationStatus>().ReverseMap();
            configuration.CreateMap<QuotationStatusDto, QuotationStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditItemTypeDto, ItemType>().ReverseMap();
            configuration.CreateMap<ItemTypeDto, ItemType>().ReverseMap();
            configuration.CreateMap<CreateOrEditAssetOwnerDto, AssetOwner>().ReverseMap();
            configuration.CreateMap<AssetOwnerDto, AssetOwner>().ReverseMap();
            configuration.CreateMap<CreateOrEditVendorDto, Vendor>().ReverseMap();
            configuration.CreateMap<VendorDto, Vendor>().ReverseMap();
            configuration.CreateMap<CreateOrEditCurrencyDto, Currency>().ReverseMap();
            configuration.CreateMap<CurrencyDto, Currency>().ReverseMap();
            configuration.CreateMap<CreateOrEditVendorChargeDetailDto, VendorChargeDetail>().ReverseMap();
            configuration.CreateMap<VendorChargeDetailDto, VendorChargeDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditCustomerGroupMembershipDto, CustomerGroupMembership>().ReverseMap();
            configuration.CreateMap<CustomerGroupMembershipDto, CustomerGroupMembership>().ReverseMap();
            configuration.CreateMap<CreateOrEditCustomerGroupDto, CustomerGroup>().ReverseMap();
            configuration.CreateMap<CustomerGroupDto, CustomerGroup>().ReverseMap();
            configuration.CreateMap<CreateOrEditCustomerDto, Customer>().ReverseMap();
            configuration.CreateMap<CustomerDto, Customer>().ReverseMap();
            configuration.CreateMap<CreateOrEditCustomerTypeDto, CustomerType>().ReverseMap();
            configuration.CreateMap<CustomerTypeDto, CustomerType>().ReverseMap();
            configuration.CreateMap<CreateOrEditSsicCodeDto, SsicCode>().ReverseMap();
            configuration.CreateMap<SsicCodeDto, SsicCode>().ReverseMap();
            configuration.CreateMap<CreateOrEditUomDto, Uom>().ReverseMap();
            configuration.CreateMap<UomDto, Uom>().ReverseMap();
            configuration.CreateMap<CreateOrEditAddressDto, Address>().ReverseMap();
            configuration.CreateMap<AddressDto, Address>().ReverseMap();
            configuration.CreateMap<CreateOrEditAssetClassDto, AssetClass>().ReverseMap();
            configuration.CreateMap<AssetClassDto, AssetClass>().ReverseMap();

            configuration.CreateMap<CreateOrEditAssetNotesDto, AssetNote>().ReverseMap();
            configuration.CreateMap<AssetNotesDto, AssetNote>().ReverseMap();

            configuration.CreateMap<CreateOrEditAssetTypeDto, AssetType>().ReverseMap();
            configuration.CreateMap<AssetTypeDto, AssetType>().ReverseMap();
            //Inputs
            configuration.CreateMap<CheckboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<SingleLineStringInputType, FeatureInputTypeDto>();
            configuration.CreateMap<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<IInputType, FeatureInputTypeDto>()
                .Include<CheckboxInputType, FeatureInputTypeDto>()
                .Include<SingleLineStringInputType, FeatureInputTypeDto>()
                .Include<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<ILocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>()
                .Include<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<LocalizableComboboxItem, LocalizableComboboxItemDto>();
            configuration.CreateMap<ILocalizableComboboxItem, LocalizableComboboxItemDto>()
                .Include<LocalizableComboboxItem, LocalizableComboboxItemDto>();

            //Chat
            configuration.CreateMap<ChatMessage, ChatMessageDto>();
            configuration.CreateMap<ChatMessage, ChatMessageExportDto>();

            //Feature
            configuration.CreateMap<FlatFeatureSelectDto, Feature>().ReverseMap();
            configuration.CreateMap<Feature, FlatFeatureDto>();

            //Role
            configuration.CreateMap<RoleEditDto, Role>().ReverseMap();
            configuration.CreateMap<Role, RoleListDto>();
            configuration.CreateMap<UserRole, UserListRoleDto>();

            //Edition
            configuration.CreateMap<EditionEditDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<EditionCreateDto, SubscribableEdition>();
            configuration.CreateMap<EditionSelectDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<Edition, EditionInfoDto>().Include<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<SubscribableEdition, EditionListDto>();
            configuration.CreateMap<Edition, EditionEditDto>();
            configuration.CreateMap<Edition, SubscribableEdition>();
            configuration.CreateMap<Edition, EditionSelectDto>();


            //Payment
            configuration.CreateMap<SubscriptionPaymentDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPaymentListDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPayment, SubscriptionPaymentInfoDto>();

            //Permission
            configuration.CreateMap<Permission, FlatPermissionDto>();
            configuration.CreateMap<Permission, FlatPermissionWithLevelDto>();

            //Language
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageListDto>();
            configuration.CreateMap<NotificationDefinition, NotificationSubscriptionWithDisplayNameDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>()
                .ForMember(ldto => ldto.IsEnabled, options => options.MapFrom(l => !l.IsDisabled));

            //Tenant
            configuration.CreateMap<Tenant, RecentTenant>();
            configuration.CreateMap<Tenant, TenantLoginInfoDto>();
            configuration.CreateMap<Tenant, TenantListDto>();
            configuration.CreateMap<TenantEditDto, Tenant>().ReverseMap();
            configuration.CreateMap<CurrentTenantInfoDto, Tenant>().ReverseMap();

            //User
            configuration.CreateMap<User, UserEditDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ReverseMap()
                .ForMember(user => user.Password, options => options.Ignore());
            configuration.CreateMap<User, UserLoginInfoDto>();
            configuration.CreateMap<User, UserListDto>();
            configuration.CreateMap<User, ChatUserDto>();
            configuration.CreateMap<User, OrganizationUnitUserListDto>();
            configuration.CreateMap<Role, OrganizationUnitRoleListDto>();
            configuration.CreateMap<CurrentUserProfileEditDto, User>().ReverseMap();
            configuration.CreateMap<UserLoginAttemptDto, UserLoginAttempt>().ReverseMap();
            configuration.CreateMap<ImportUserDto, User>();

            //AuditLog
            configuration.CreateMap<AuditLog, AuditLogListDto>();
            configuration.CreateMap<EntityChange, EntityChangeListDto>();
            configuration.CreateMap<EntityPropertyChange, EntityPropertyChangeDto>();

            //Friendship
            configuration.CreateMap<Friendship, FriendDto>();
            configuration.CreateMap<FriendCacheItem, FriendDto>();

            //OrganizationUnit
            configuration.CreateMap<OrganizationUnit, OrganizationUnitDto>();

            /* ADD YOUR OWN CUSTOM AUTOMAPPER MAPPINGS HERE */
        }
    }
}
