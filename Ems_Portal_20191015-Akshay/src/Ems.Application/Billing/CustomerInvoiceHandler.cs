using Abp.Dependency;
using Abp.Domain.Services;
using System;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Ems.MultiTenancy;
using Abp.Domain.Uow;
using Abp.Domain.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using Abp.BackgroundJobs;
using System.Linq;
using Ems.Billing.Dtos;

namespace Ems.Billing
{
    class CustomerInvoiceHandler : EmsAppServiceBase, IDomainService, ITransientDependency,
        IAsyncEventHandler<EntityCreatedEventData<CustomerInvoice>>//, IAsyncEventHandler<EntityChangedEventData<CustomerInvoice>>

    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<CustomerInvoice> _invoiceRepository;
        private readonly IRepository<CustomerInvoiceDetail> _invoiceDetailRepository;

        public CustomerInvoiceHandler(
            IBackgroundJobManager backgroundJobManager,
            IRepository<CustomerInvoice> invoiceRepository,
            IRepository<CustomerInvoiceDetail> invoiceDetailRepository
            )

        {
            _backgroundJobManager = backgroundJobManager;
            _invoiceRepository = invoiceRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
        }

        public async Task HandleEventAsync(EntityCreatedEventData<CustomerInvoice> eventData)
        {
            //TenantInfo tenantInfo = TenantManager.GetTenantInfo().Result;
            GetCustomerInvoiceForViewDto entity = new GetCustomerInvoiceForViewDto { CustomerInvoice = ObjectMapper.Map<CustomerInvoiceDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CustomerInvoiceManager, CustomerInvoiceManagerArgs>(
                new CustomerInvoiceManagerArgs
                {   
                    Created = true,
                    CustomerInvoiceDto = entity.CustomerInvoice,
                });
        }

        public async Task HandleEventAsync(EntityChangedEventData<CustomerInvoice> eventData)
        {
            //TenantInfo tenantInfo = TenantManager.GetTenantInfo().Result;
            GetCustomerInvoiceForViewDto entity = new GetCustomerInvoiceForViewDto { CustomerInvoice = ObjectMapper.Map<CustomerInvoiceDto>(eventData.Entity) };
            await _backgroundJobManager.EnqueueAsync<CustomerInvoiceManager, CustomerInvoiceManagerArgs>(
                new CustomerInvoiceManagerArgs
                {
                    Created = false,
                    CustomerInvoiceDto = entity.CustomerInvoice,
                });
        }
    }
}


