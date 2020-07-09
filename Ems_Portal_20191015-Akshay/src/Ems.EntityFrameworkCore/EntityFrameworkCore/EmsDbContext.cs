using Ems.Finance;
using Ems.Authorization;
using Ems.Telematics;
using Ems.Support;
using Ems.Quotations;
using Ems.Billing;
using Ems.Vendors;
using Ems.Customers;
using Ems.Metrics;
using Ems.Organizations;
using Ems.Assets;
using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ems.Authorization.Roles;
using Ems.Authorization.Users;
using Ems.Chat;
using Ems.Editions;
using Ems.Friendships;
using Ems.MultiTenancy;
using Ems.MultiTenancy.Accounting;
using Ems.MultiTenancy.Payments;
using Ems.Storage;
using Ems.Logs;

namespace Ems.EntityFrameworkCore
{
    public class EmsDbContext : AbpZeroDbContext<Tenant, Role, User, EmsDbContext>, IAbpPersistedGrantDbContext
    {
        public virtual DbSet<XeroInvoice> XeroInvoices { get; set; }

        public virtual DbSet<InventoryItem> InventoryItems { get; set; }

        public virtual DbSet<AssetPartStatus> AssetPartStatuses { get; set; }

        public virtual DbSet<AssetPart> AssetParts { get; set; }

        public virtual DbSet<AssetPartType> AssetPartTypes { get; set; }

        public virtual DbSet<AgedReceivablesPeriod> AgedReceivablesPeriods { get; set; }

        public virtual DbSet<CrossTenantPermission> CrossTenantPermissions { get; set; }

        public virtual DbSet<VendorChargeStatus> VendorChargeStatuses { get; set; }

        public virtual DbSet<WorkOrderStatus> WorkOrderStatuses { get; set; }

        public virtual DbSet<WorkOrderAction> WorkOrderActions { get; set; }

        public virtual DbSet<CustomerInvoiceDetail> CustomerInvoiceDetails { get; set; }

        public virtual DbSet<CustomerInvoice> CustomerInvoices { get; set; }

        public virtual DbSet<CustomerInvoiceStatus> CustomerInvoiceStatuses { get; set; }

        public virtual DbSet<Estimate> Estimates { get; set; }

        public virtual DbSet<EstimateDetail> EstimateDetails { get; set; }

        public virtual DbSet<EstimateStatus> EstimateStatuses { get; set; }

        public virtual DbSet<Attachment> Attachments { get; set; }

        public virtual DbSet<WorkOrderUpdate> WorkOrderUpdates { get; set; }

        public virtual DbSet<QuotationDetail> QuotationDetails { get; set; }

        public virtual DbSet<WorkOrder> WorkOrders { get; set; }

        public virtual DbSet<WorkOrderPriority> WorkOrderPriorities { get; set; }

        //public virtual DbSet<WorkOrderLoc8GUID> WorkOrderLoc8GUIDs { get; set; }

        public virtual DbSet<Rfq> Rfqs { get; set; }

        public virtual DbSet<RfqType> RfqTypes { get; set; }

        public virtual DbSet<IncidentUpdate> IncidentUpdates { get; set; }

        public virtual DbSet<Incident> Incidents { get; set; }

        public virtual DbSet<IncidentType> IncidentTypes { get; set; }

        public virtual DbSet<IncidentStatus> IncidentStatuses { get; set; }

        public virtual DbSet<IncidentPriority> IncidentPriorities { get; set; }

        public virtual DbSet<SupportItem> SupportItems { get; set; }

        public virtual DbSet<BillingEventDetail> BillingEventDetails { get; set; }

        public virtual DbSet<BillingEvent> BillingEvents { get; set; }

        public virtual DbSet<VendorCharge> VendorCharges { get; set; }

        public virtual DbSet<BillingRule> BillingRules { get; set; }

        public virtual DbSet<UsageMetricRecord> UsageMetricRecords { get; set; }

        public virtual DbSet<UsageMetric> UsageMetrics { get; set; }

        public virtual DbSet<LeaseItem> LeaseItems { get; set; }

        public virtual DbSet<AssetOwnership> AssetOwnerships { get; set; }

        public virtual DbSet<Asset> Assets { get; set; }

        public virtual DbSet<LeaseAgreement> LeaseAgreements { get; set; }

        public virtual DbSet<Contact> Contacts { get; set; }

        public virtual DbSet<Quotation> Quotations { get; set; }

        public virtual DbSet<BillingRuleType> BillingRuleTypes { get; set; }

        public virtual DbSet<BillingEventType> BillingEventTypes { get; set; }

        public virtual DbSet<AssetStatus> AssetStatuses { get; set; }

        public virtual DbSet<ConsumableType> ConsumableTypes { get; set; }

        public virtual DbSet<SupportType> SupportTypes { get; set; }

        public virtual DbSet<SupportContract> SupportContracts { get; set; }

        public virtual DbSet<QuotationStatus> QuotationStatuses { get; set; }

        public virtual DbSet<ItemType> ItemTypes { get; set; }

        public virtual DbSet<AssetOwner> AssetOwners { get; set; }

        public virtual DbSet<Vendor> Vendors { get; set; }

        public virtual DbSet<Currency> Currencies { get; set; }

        public virtual DbSet<VendorChargeDetail> VendorChargeDetails { get; set; }

        public virtual DbSet<WorkOrderType> WorkOrderTypes { get; set; }

        public virtual DbSet<CustomerGroupMembership> CustomerGroupMemberships { get; set; }

        public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<CustomerType> CustomerTypes { get; set; }

        public virtual DbSet<SsicCode> SsicCodes { get; set; }

        public virtual DbSet<Uom> Uoms { get; set; }

        public virtual DbSet<Address> Addresses { get; set; }

        public virtual DbSet<Location> Locations { get; set; }

        public virtual DbSet<AssetNote> AssetNotes { get; set; }

        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }

        public virtual DbSet<AssetClass> AssetClasses { get; set; }

        public virtual DbSet<AssetType> AssetTypes { get; set; }

        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public EmsDbContext(DbContextOptions<EmsDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<XeroInvoice>(x =>
            {
                x.HasIndex(e => new { e.TenantId });
            });
 modelBuilder.Entity<InventoryItem>(i =>
                        {
                            i.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<AssetPartStatus>(a =>
                        {
                            a.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<AssetPart>(a =>
                        {
                            a.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<AssetPartType>(a =>
                        {
                            a.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<AgedReceivablesPeriod>(a =>
                        {
                            a.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<CrossTenantPermission>(c =>
                        {
                            c.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<VendorChargeStatus>(v =>
                        {
                            v.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<WorkOrderStatus>(w =>
                        {
                            w.HasIndex(e => new { e.TenantId });
                        });
                        modelBuilder.Entity<WorkOrderAction>(w =>
                        {
                            w.HasIndex(e => new { e.TenantId });
                        });
                        modelBuilder.Entity<CustomerInvoiceDetail>(c =>
                        {
                            c.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<CustomerInvoice>(c =>
                        {
                            c.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<CustomerInvoiceStatus>(c =>
                        {
                            c.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<Estimate>(x =>
                        {
                            x.HasIndex(e => new { e.TenantId });
                        });
                        modelBuilder.Entity<EstimateDetail>(q =>
                        {
                            q.HasIndex(e => new { e.TenantId });
                        });
                        modelBuilder.Entity<EstimateStatus>(x =>
                        {
                            x.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<Attachment>(a =>
                        {
                            a.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<WorkOrderUpdate>(w =>
                        {
                            w.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<QuotationDetail>(q =>
                        {
                            q.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<WorkOrder>(w =>
                        {
                            w.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<WorkOrderType>(w =>
                        {
                            w.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<WorkOrderPriority>(w =>
                        {
                            w.HasIndex(e => new { e.TenantId });
                        });

             modelBuilder.Entity<Rfq>(r =>
                        {
                            r.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<RfqType>(r =>
                        {
                            r.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<IncidentUpdate>(i =>
                        {
                            i.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<Incident>(i =>
                        {
                            i.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<IncidentType>(i =>
                        {
                            i.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<IncidentStatus>(i =>
                        {
                            i.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<IncidentPriority>(i =>
                        {
                            i.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<SupportItem>(s =>
                        {
                            s.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<BillingEventDetail>(b =>
                        {
                            b.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<BillingEvent>(b =>
                        {
                            b.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<VendorCharge>(v =>
                        {
                            v.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<BillingRule>(b =>
                        {
                            b.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<UsageMetricRecord>(u =>
                        {
                            u.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<UsageMetric>(u =>
                        {
                            u.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<LeaseItem>(l =>
                        {
                            l.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<AssetOwnership>(a =>
                        {
                            a.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<Asset>(a =>
                        {
                            a.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<LeaseAgreement>(l =>
                        {
                            l.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<Contact>(c =>
                        {
                            c.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<Quotation>(q =>
                        {
                            q.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<BillingRuleType>(b =>
                        {
                            b.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<BillingEventType>(b =>
                        {
                            b.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<AssetStatus>(a =>
                        {
                            a.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<ConsumableType>(c =>
                        {
                            c.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<SupportType>(s =>
                        {
                            s.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<SupportContract>(s =>
                        {
                            s.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<QuotationStatus>(q =>
                        {
                            q.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<ItemType>(i =>
                        {
                            i.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<AssetOwner>(a =>
                        {
                            a.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<Vendor>(v =>
                        {
                            v.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<Currency>(c =>
                        {
                            c.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<VendorChargeDetail>(v =>
                        {
                            v.HasIndex(e => new { e.TenantId });
                        });

             modelBuilder.Entity<CustomerGroupMembership>(c =>
                        {
                            c.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<CustomerGroup>(c =>
                        {
                            c.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<Customer>(c =>
                        {
                            c.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<CustomerType>(c =>
                        {
                            c.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<SsicCode>(s =>
                        {
                            s.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<Uom>(u =>
                        {
                            u.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<Address>(a =>
            {
                a.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<Location>(a =>
            {
                a.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<AssetNote>(a =>
            {
                a.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<ExceptionLog>(a =>
            {
                a.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<AssetClass>(a =>
            {
                a.HasIndex(e => new { e.TenantId });
            });
             modelBuilder.Entity<AssetType>(a =>
                        {
                            a.HasIndex(e => new { e.TenantId });
                        });
             modelBuilder.Entity<BinaryObject>(b =>
            {
                b.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.SubscriptionEndDateUtc });
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.HasIndex(e => new { e.Status, e.CreationTime });
                b.HasIndex(e => new { PaymentId = e.ExternalPaymentId, e.Gateway });
            });

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}
