using Abp.Dependency;
using Abp.Domain.Services;
using System;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Ems.Support;
using Ems.Assets;
using Ems.Quotations;
using Ems.MultiTenancy;
using Abp.Domain.Uow;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using Abp.BackgroundJobs;
using Ems.Support.Dtos;
using Ems.Assets.Dtos;
using Ems.Quotations.Dtos;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ems.Vendors;
using Ems.Customers;
using Abp.Runtime.Caching;

namespace Ems.Authorization
{
    public class CrossTenantPermissionManager : BackgroundJob<CrossTenantPermissionManagerArgs>, IDomainService, ITransientDependency

    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<CrossTenantPermission> _crossTenantPermissionRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<AssetOwner> _assetOwnerRepository;
        private readonly IRepository<LeaseItem> _leaseItemRepository;
        private readonly IRepository<SupportItem> _supportItemRepository;
        private readonly IRepository<LeaseAgreement> _leaseAgreementRepository;
        private readonly IRepository<SupportContract> _supportContractRepository;
        private readonly IRepository<Incident> _incidentRepository;
        private readonly IRepository<WorkOrder> _workOrderRepository;
        //private readonly IRepository<Quotation> _quotationRepository;
        private readonly ICacheManager _cacheManager;


        public CrossTenantPermissionManager(
              IUnitOfWorkManager unitOfWorkManager,
              IRepository<CrossTenantPermission> crossTenantPermissionRepository,
              IRepository<Vendor> vendorRepository,
              IRepository<Customer> customerRepository,
              IRepository<AssetOwner> assetOwnerRepository,
              IRepository<LeaseItem> leaseItemRepository,
              IRepository<SupportItem> supportItemRepository,
              IRepository<LeaseAgreement> leaseAgreementRepository,
              IRepository<SupportContract> supportContractRepository,
              IRepository<Incident> incidentRepository,
              IRepository<WorkOrder> workOrderRepository,
              ICacheManager cacheManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _crossTenantPermissionRepository = crossTenantPermissionRepository;
            _vendorRepository = vendorRepository;
            _customerRepository = customerRepository;
            _assetOwnerRepository = assetOwnerRepository;
            _leaseItemRepository = leaseItemRepository;
            _supportItemRepository = supportItemRepository;
            _leaseAgreementRepository = leaseAgreementRepository;
            _supportContractRepository = supportContractRepository;
            _incidentRepository = incidentRepository;
            _workOrderRepository = workOrderRepository;
            _cacheManager = cacheManager;
        }


        [UnitOfWork]
        public override void Execute(CrossTenantPermissionManagerArgs args)
        {
            switch (args.EntityType)
            {
                case "LeaseAgreement":
                    HandleLeaseAgreementEvent(args.LeaseAgreementDto, args.TenantInfo);
                    break;

                case "SupportContract":
                    HandleSupportContractEvent(args.SupportContractDto, args.TenantInfo); 
                    break;

                case "LeaseItem":
                    HandleLeaseItemEvent(args.LeaseItemDto, args.TenantInfo);
                    break;

                case "SupportItem":
                    HandleSupportItemEvent(args.SupportItemDto, args.TenantInfo); 
                    break;

                /*
            case "Asset":
                HandleAssetEvent(args.AssetDto, args.TenantInfo); //, args.CrossTenantPermissions);
                break;
            case "Incident":
                HandleIncidentEvent(args.IncidentDto, args.TenantInfo); //, args.CrossTenantPermissions);
                break;
            case "WorkOrder":
                HandleWorkOrderEvent(args.WorkOrderDto, args.TenantInfo); //, args.CrossTenantPermissions);
                break;
            case "Quotation":
                HandleQuotationEvent(args.QuotationDto, args.TenantInfo); //, args.CrossTenantPermissions);
                break;
            case "Estimate":
                HandleEstimateEvent(args.EstimateDto, args.TenantInfo); //, args.CrossTenantPermissions);
                break;
                */
                default:
                    throw new Exception($"'{args.EntityType}' entity cannot be handled!");

            }

        }

        [UnitOfWork]
        public virtual void HandleSupportContractEvent(SupportContractDto supportContractDto, TenantInfo tenantInfo)
        {
            // Add Cross-Tenant Permissions between Asset Owners and Vendors based on the new Support Contract...
            // Permissions are for Incidents, WorkOrders, Quotations, Assets and Users

            int assetOwnerTenantId = (int)_assetOwnerRepository.Get((int)supportContractDto.AssetOwnerId).TenantId;
            int vendorTenantId = (int)_vendorRepository.Get((int)supportContractDto.VendorId).TenantId;

            List<string> entities = new List<string>() { "Incident","WorkOrder","Quotation","Asset","User" };
            List<int> tenantIds = new List<int> { assetOwnerTenantId, vendorTenantId};

            var allCtps = _crossTenantPermissionRepository.GetAllList();

            bool clearCache = false;

            foreach (var entity in entities)
            {
                foreach (var tenantId in tenantIds)
                {
                    int otherTenantId = (tenantId == tenantIds.Last()) ? tenantIds.First() : tenantIds.Last();
                    string tenantType = (tenantId == tenantIds.First()) ? "A" : "V";

                    var existingPermission = allCtps.Where(p => p.TenantId == tenantId && p.EntityType == entity).FirstOrDefault();

                    if (existingPermission != null)
                    {
                        var existingIds = existingPermission.Tenants.Split(',').ToList().Select(int.Parse).ToList();
                        if (!existingIds.Contains(otherTenantId))
                        {
                            existingIds.Add(otherTenantId);

                            var ctp = _crossTenantPermissionRepository.GetAll().Where(p => p.TenantId == tenantId && p.EntityType == entity).FirstOrDefault();
                            ctp.Tenants = string.Join(",", existingIds);
                            _crossTenantPermissionRepository.Update(ctp);
                            clearCache = true;
                        }
                    }
                    else
                    {
                        CrossTenantPermission ctp = new CrossTenantPermission()
                        {
                            EntityType = entity,
                            TenantId = tenantId,
                            Tenants = otherTenantId.ToString(),
                            TenantType = tenantType,
                            TenantRefId = tenantId
                        };

                        _crossTenantPermissionRepository.Insert(ctp);
                        clearCache = true;
                    }
                }
            }
            if (clearCache)
            {
                _cacheManager.GetCrossTenantPermissionCache().Clear();
            }
        }

        [UnitOfWork]
        public virtual void HandleLeaseAgreementEvent(LeaseAgreementDto leaseAgreementDto, TenantInfo tenantInfo)
        {
            // Add Cross-Tenant Permissions between Asset Owners and Customer based on the new Lease Agreement
            //      Permissions are for Incidents, WorkOrders, Estimates, Assets and Users

            int assetOwnerTenantId = (int)_assetOwnerRepository.Get((int)leaseAgreementDto.AssetOwnerId).TenantId;
            int customerTenantId = (int)_customerRepository.Get((int)leaseAgreementDto.CustomerId).TenantId;

            List<string> entities = new List<string>() { "Incident", "WorkOrder", "Estimate", "Asset", "User" };
            List<int> tenantIds = new List<int> { assetOwnerTenantId, customerTenantId };

            var allCtps = _crossTenantPermissionRepository.GetAllList();

            bool clearCache = false;

            foreach (var entity in entities)
            {
                foreach (var tenantId in tenantIds)
                {
                    int otherTenantId = (tenantId == tenantIds.Last()) ? tenantIds.First() : tenantIds.Last();
                    string tenantType = (tenantId == tenantIds.First()) ? "A" : "C";

                    var existingPermission = allCtps.Where(p => p.TenantId == tenantId && p.EntityType == entity).FirstOrDefault();

                    if (existingPermission != null)
                    {
                        var existingIds = existingPermission.Tenants.Split(',').ToList().Select(int.Parse).ToList();
                        if (!existingIds.Contains(otherTenantId))
                        {
                            existingIds.Add(otherTenantId);

                            var ctp = _crossTenantPermissionRepository.GetAll().Where(p => p.TenantId == tenantId && p.EntityType == entity).FirstOrDefault();
                            ctp.Tenants = string.Join(",", existingIds);
                            _crossTenantPermissionRepository.Update(ctp);
                            clearCache = true;
                        }
                    }
                    else
                    {
                        CrossTenantPermission ctp = new CrossTenantPermission()
                        {
                            EntityType = entity,
                            TenantId = tenantId,
                            Tenants = otherTenantId.ToString(),
                            TenantType = tenantType,
                            TenantRefId = tenantId
                        };

                        _crossTenantPermissionRepository.Insert(ctp);
                        clearCache = true;
                    }
                }
            }
            if (clearCache)
            {
                _cacheManager.GetCrossTenantPermissionCache().Clear();
            }
        }

        [UnitOfWork]
        public virtual void HandleLeaseItemEvent(LeaseItemDto leaseItemDto, TenantInfo tenantInfo) 
        {
            // Add Cross-Tenant Permissions between Vendors and Customers based on the new LeaseItem
            //     via SupportItems for the relevant Asset
            //           Permissions are for Incidents, WorkOrders, Assets and Users

            List<string> entities = new List<string>() {  "Incident", "WorkOrder", "Asset", "User" };
            bool clearCache = false;

            LeaseAgreement leaseAgreement = _leaseAgreementRepository.Get((int)leaseItemDto.LeaseAgreementId);
            Customer customer = _customerRepository.Get((int)leaseAgreement.CustomerId);
            int customerTenantId = (int)customer.TenantId;

            List<int> supportContractIds = _supportItemRepository.GetAll().Where(s => s.AssetId == leaseItemDto.AssetId).Select(s => (int)s.SupportContractId).ToList();
            List<int> vendorIds = _supportContractRepository.GetAll().Where(l => supportContractIds.Contains(l.Id)).Select(l => (int)l.VendorId).Distinct().ToList();
            List<int> vendorTenantIds = _vendorRepository.GetAll().Where(c => vendorIds.Contains(c.Id)).Select(c => (int)c.TenantId).Distinct().ToList();
            
            var allCtps = _crossTenantPermissionRepository.GetAllList();

            foreach (var entity in entities)
            {

                foreach (var vendorTenantId in vendorTenantIds)
                {
                    // Add the Vendor Tenant Ids to the Customers' CTPs

                    var existingCustomerPermission = allCtps.Where(p => p.TenantId == customerTenantId && p.EntityType == entity).FirstOrDefault();

                    if (existingCustomerPermission != null)
                    {
                        var existingIds = existingCustomerPermission.Tenants.Split(',').ToList().Select(int.Parse).ToList();
                        if (!existingIds.Contains(vendorTenantId))
                        {
                            existingIds.Add(vendorTenantId);

                            var ctp = _crossTenantPermissionRepository.GetAll().Where(p => p.TenantId == customerTenantId && p.EntityType == entity).FirstOrDefault();
                            ctp.Tenants = string.Join(",", existingIds);
                            _crossTenantPermissionRepository.Update(ctp);
                            clearCache = true;
                        }
                    }
                    else
                    {
                        CrossTenantPermission ctp = new CrossTenantPermission()
                        {
                            EntityType = entity,
                            TenantId = customerTenantId,
                            Tenants = vendorTenantId.ToString(),
                            TenantType = "C",
                            TenantRefId = customerTenantId
                        };

                        _crossTenantPermissionRepository.Insert(ctp);
                        clearCache = true;
                    }

                    // Add the Customer Tenant Id to this Vendors' CTPs

                    var existingVendorPermission = allCtps.Where(p => p.TenantId == vendorTenantId && p.EntityType == entity).FirstOrDefault();

                    if (existingVendorPermission != null)
                    {
                        var existingIds = existingVendorPermission.Tenants.Split(',').ToList().Select(int.Parse).ToList();
                        if (!existingIds.Contains(customerTenantId))
                        {
                            existingIds.Add(customerTenantId);

                            var ctp = _crossTenantPermissionRepository.GetAll().Where(p => p.TenantId == vendorTenantId && p.EntityType == entity).FirstOrDefault();
                            ctp.Tenants = string.Join(",", existingIds);
                            _crossTenantPermissionRepository.Update(ctp);
                            clearCache = true;
                        }
                    }
                    else
                    {
                        CrossTenantPermission ctp = new CrossTenantPermission()
                        {
                            EntityType = entity,
                            TenantId = vendorTenantId,
                            Tenants = customerTenantId.ToString(),
                            TenantType = "V",
                            TenantRefId = vendorTenantId
                        };

                        _crossTenantPermissionRepository.Insert(ctp);
                        clearCache = true;
                    }
                }
            }
            if (clearCache)
            {
                _cacheManager.GetCrossTenantPermissionCache().Clear();
            }
        }

        [UnitOfWork]
        public virtual void HandleSupportItemEvent(SupportItemDto supportItemDto, TenantInfo tenantInfo)
        {
            // Add Cross-Tenant Permissions between Vendors and Customers based on the new SupportItem
            //     via LeaseItems for the relevant Asset
            //           Permissions are for Incidents, WorkOrders, Assets and Users

            List<string> entities = new List<string>() { "Incident", "WorkOrder", "Asset", "User" };

            SupportContract supportContract = _supportContractRepository.Get((int)supportItemDto.SupportContractId);
            Vendor vendor = _vendorRepository.Get((int)supportContract.VendorId);
            int vendorTenantId = (int)vendor.TenantId;

            List<int> leaseAgreementIds = _leaseItemRepository.GetAll().Where(s => s.AssetId == supportItemDto.AssetId).Select(s => (int)s.LeaseAgreementId).ToList();
            List<int> customerIds = _leaseAgreementRepository.GetAll().Where(l => leaseAgreementIds.Contains(l.Id)).Select(l => (int)l.CustomerId).Distinct().ToList();
            List<int> customerTenantIds = _customerRepository.GetAll().Where(c => customerIds.Contains(c.Id)).Select(c => (int)c.TenantId).Distinct().ToList();

            bool clearCache = false;

            var allCtps = _crossTenantPermissionRepository.GetAllList();

            foreach (var entity in entities)
            {
                foreach (var customerTenantId in customerTenantIds)
                {
                    // Add the Customer Tenant Ids to the Vendors' CTPs

                    var existingVendorPermission = allCtps.Where(p => p.TenantId == vendorTenantId && p.EntityType == entity).FirstOrDefault();

                    if (existingVendorPermission != null)
                    {
                        var existingIds = existingVendorPermission.Tenants.Split(',').ToList().Select(int.Parse).ToList();
                        if (!existingIds.Contains(customerTenantId))
                        {
                            existingIds.Add(customerTenantId);

                            var ctp = _crossTenantPermissionRepository.GetAll().Where(p => p.TenantId == vendorTenantId && p.EntityType == entity).FirstOrDefault();
                            ctp.Tenants = string.Join(",", existingIds);
                            _crossTenantPermissionRepository.Update(ctp);
                            clearCache = true;
                        }
                    }
                    else
                    {
                        CrossTenantPermission ctp = new CrossTenantPermission()
                        {
                            EntityType = entity,
                            TenantId = vendorTenantId,
                            Tenants = customerTenantId.ToString(),
                            TenantType = "V",
                            TenantRefId = vendorTenantId
                        };

                        _crossTenantPermissionRepository.Insert(ctp);
                        clearCache = true;
                    }

                    // Add the Vendor Tenant Id to this Customer' CTPs

                    var existingCustomerPermission = allCtps.Where(p => p.TenantId == customerTenantId && p.EntityType == entity).FirstOrDefault();

                    if (existingCustomerPermission != null)
                    {
                        var existingIds = existingCustomerPermission.Tenants.Split(',').ToList().Select(int.Parse).ToList();
                        if (!existingIds.Contains(vendorTenantId))
                        {
                            existingIds.Add(vendorTenantId);

                            var ctp = _crossTenantPermissionRepository.GetAll().Where(p => p.TenantId == customerTenantId && p.EntityType == entity).FirstOrDefault();
                            ctp.Tenants = string.Join(",", existingIds);
                            _crossTenantPermissionRepository.Update(ctp);
                            clearCache = true;
                        }
                    }
                    else
                    {
                        CrossTenantPermission ctp = new CrossTenantPermission()
                        {
                            EntityType = entity,
                            TenantId = customerTenantId,
                            Tenants = vendorTenantId.ToString(),
                            TenantType = "C",
                            TenantRefId = customerTenantId
                        };

                        _crossTenantPermissionRepository.Insert(ctp);
                        clearCache = true;
                    }
                }
            }

            if (clearCache)
            {
                _cacheManager.GetCrossTenantPermissionCache().Clear();
            }
        }

        /*

        public void HandleIncidentEvent(IncidentDto incidentDto, TenantInfo tenantInfo) //, List<int> crossTenantPermissions)
        {
            //throw new NotImplementedException();
        }

        public void HandleEstimateEvent(EstimateDto estimateDto, TenantInfo tenantInfo) //, List<int> crossTenantPermissions)
        {
            //throw new NotImplementedException();
        }

        public void HandleQuotationEvent(QuotationDto quotationDto, TenantInfo tenantInfo) //, List<int> crossTenantPermissions)
        {
            //throw new NotImplementedException();
        }

        public void HandleWorkOrderEvent(WorkOrderDto workOrderDto, TenantInfo tenantInfo) //, List<int> crossTenantPermissions)
        {
            var t = tenantInfo;
        }

        public void HandleAssetEvent(AssetDto assetDto, TenantInfo tenantInfo) //, List<int> crossTenantPermissions)
        {
            //throw new NotImplementedException();
        }
        */
    }

    public class CrossTenantPermissionManagerArgs
    {
        public string EntityType { get; set; }

        public LeaseAgreementDto LeaseAgreementDto { get; set; }
        public SupportContractDto SupportContractDto { get; set; }
        public LeaseItemDto LeaseItemDto { get; set; }
        public SupportItemDto SupportItemDto { get; set; }

        // public AssetDto AssetDto { get; set; }
        // public IncidentDto IncidentDto { get; set; }
        // public WorkOrderDto WorkOrderDto { get; set; }
        // public QuotationDto QuotationDto { get; set; }
        // public EstimateDto EstimateDto { get; set; }

        public TenantInfo TenantInfo { get; set; }
        // public List<int> CrossTenantPermissions { get; set; }
    }
}

