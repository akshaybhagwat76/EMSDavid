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
using System.Threading.Tasks;
using System.Collections.Generic;
using Abp.BackgroundJobs;
using Ems.Support.Dtos;
using System.Linq;
using Ems.Quotations.Dtos;
using Ems.Assets.Dtos;

namespace Ems.Authorization
{
    class CrossTenantPermissionHandler : EmsAppServiceBase, IDomainService, ITransientDependency, 
        IAsyncEventHandler<EntityCreatedEventData<SupportContract>>,
        IAsyncEventHandler<EntityCreatedEventData<LeaseAgreement>>,
        IAsyncEventHandler<EntityCreatedEventData<SupportItem>>,
        IAsyncEventHandler<EntityCreatedEventData<LeaseItem>>
    //IAsyncEventHandler<EntityCreatedEventData<Asset>>,
    //IAsyncEventHandler<EntityCreatedEventData<Incident>>,
    //IAsyncEventHandler<EntityCreatedEventData<WorkOrder>>,
    //IAsyncEventHandler<EntityCreatedEventData<Quotation>>,
    //IAsyncEventHandler<EntityCreatedEventData<Estimate>>

    {
        //private readonly CrossTenantPermissionManager _crossTenantPermissionManager;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<CrossTenantPermission> _crossTenantPermissionRepository;

        public CrossTenantPermissionHandler(
            IBackgroundJobManager backgroundJobManager, 
            IRepository<Tenant> tenantRepository, 
            IRepository<CrossTenantPermission> crossTenantPermissionRepository
            )
        {
            _backgroundJobManager = backgroundJobManager;
            _tenantRepository = tenantRepository;
            _crossTenantPermissionRepository = crossTenantPermissionRepository;
        }

        public async Task HandleEventAsync(EntityCreatedEventData<LeaseAgreement> eventData)
        {
            TenantInfo tenantInfo = TenantManager.GetTenantInfo().Result;
            //List<int> crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "LeaseAgreement");
            GetLeaseAgreementForViewDto entity = new GetLeaseAgreementForViewDto { LeaseAgreement = ObjectMapper.Map<LeaseAgreementDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    //CrossTenantPermissions = crossTenantPermissions,
                    LeaseAgreementDto = entity.LeaseAgreement,
                    EntityType = "LeaseAgreement",
                    TenantInfo = tenantInfo
                });
        }

        public async Task HandleEventAsync(EntityCreatedEventData<LeaseItem> eventData)
        {
            TenantInfo tenantInfo = TenantManager.GetTenantInfo().Result;
            //List<int> crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "LeaseItem");
            GetLeaseItemForViewDto entity = new GetLeaseItemForViewDto { LeaseItem = ObjectMapper.Map<LeaseItemDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    //CrossTenantPermissions = crossTenantPermissions,
                    LeaseItemDto = entity.LeaseItem,
                    EntityType = "LeaseItem",
                    TenantInfo = tenantInfo
                });
        }

        public async Task HandleEventAsync(EntityCreatedEventData<SupportContract> eventData)
        {
            TenantInfo tenantInfo = TenantManager.GetTenantInfo().Result;
            //List<int> crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "SupportContract");
            GetSupportContractForViewDto entity = new GetSupportContractForViewDto { SupportContract = ObjectMapper.Map<SupportContractDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    //CrossTenantPermissions = crossTenantPermissions,
                    SupportContractDto = entity.SupportContract,
                    EntityType = "SupportContract",
                    TenantInfo = tenantInfo
                });
        }
        public async Task HandleEventAsync(EntityCreatedEventData<SupportItem> eventData)
        {
            TenantInfo tenantInfo = TenantManager.GetTenantInfo().Result;
            //List<int> crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "SupportItem");
            GetSupportItemForViewDto entity = new GetSupportItemForViewDto { SupportItem = ObjectMapper.Map<SupportItemDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    //CrossTenantPermissions = crossTenantPermissions,
                    SupportItemDto = entity.SupportItem,
                    EntityType = "SupportItem",
                    TenantInfo = tenantInfo
                });
        }

        /*
        public async Task HandleEventAsync(EntityCreatedEventData<Incident> eventData)
        {
            TenantInfo tenantInfo = TenantManager.GetTenantInfo().Result;
            //List<int> crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "Incident");
            GetIncidentForViewDto entity = new GetIncidentForViewDto { Incident = ObjectMapper.Map<IncidentDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    //CrossTenantPermissions = crossTenantPermissions,
                    IncidentDto = entity.Incident,
                    EntityType = "Incident",
                    TenantInfo = tenantInfo
                });
        }


        public async Task HandleEventAsync(EntityCreatedEventData<Quotation> eventData)
        {
            TenantInfo tenantInfo = TenantManager.GetTenantInfo().Result;
            //List<int> crossTenantPermissions = TenantManager.GetCrossTenantPermissions(tenantInfo, "Quotation").Result;
            GetQuotationForViewDto entity = new GetQuotationForViewDto { Quotation = ObjectMapper.Map<QuotationDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    //CrossTenantPermissions = crossTenantPermissions,
                    QuotationDto = entity.Quotation,
                    EntityType = "Quotation",
                    TenantInfo = tenantInfo
                });
        }

        public async Task HandleEventAsync(EntityCreatedEventData<Estimate> eventData)
        {
            TenantInfo tenantInfo = TenantManager.GetTenantInfo().Result;
            //List<int> crossTenantPermissions = TenantManager.GetCrossTenantPermissions(tenantInfo, "Estimate").Result;
            GetEstimateForViewDto entity = new GetEstimateForViewDto { Estimate = ObjectMapper.Map<EstimateDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    //CrossTenantPermissions = crossTenantPermissions,
                    EstimateDto = entity.Estimate,
                    EntityType = "Estimate",
                    TenantInfo = tenantInfo
                });
        }


        public async Task HandleEventAsync(EntityCreatedEventData<WorkOrder> eventData)
        {
            TenantInfo tenantInfo = TenantManager.GetTenantInfo().Result;
            //List<int> crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "WorkOrder");
            GetWorkOrderForViewDto entity = new GetWorkOrderForViewDto { WorkOrder = ObjectMapper.Map<WorkOrderDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    //CrossTenantPermissions = crossTenantPermissions,
                    WorkOrderDto = entity.WorkOrder,
                    EntityType = "WorkOrder",
                    TenantInfo = tenantInfo
                });
        }

        public async Task HandleEventAsync(EntityCreatedEventData<Asset> eventData)
        {
            TenantInfo tenantInfo = TenantManager.GetTenantInfo().Result;
            //List<int> crossTenantPermissions = TenantManager.GetCrossTenantPermissions(tenantInfo, "Asset").Result;
            GetAssetForViewDto entity = new GetAssetForViewDto { Asset = ObjectMapper.Map<AssetDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CrossTenantPermissionManager, CrossTenantPermissionManagerArgs>(
                new CrossTenantPermissionManagerArgs
                {
                    //CrossTenantPermissions = crossTenantPermissions,
                    AssetDto = entity.Asset,
                    EntityType = "Asset",
                    TenantInfo = tenantInfo
                });
        }
        */
    }
}

