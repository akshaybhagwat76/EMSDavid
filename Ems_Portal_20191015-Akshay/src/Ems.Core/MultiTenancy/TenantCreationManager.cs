using Ems.MultiTenancy.Dto;
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
using Ems.Billing.Dtos;
using Ems.Billing;

namespace Ems.MultiTenancy
{
    public class TenantCreationManager : BackgroundJob<TenantCreationManagerArgs>, IDomainService, ITransientDependency

    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Tenant> _tenantRepository;

        public TenantCreationManager(
              IUnitOfWorkManager unitOfWorkManager,
              IRepository<Tenant> tenantRepository

            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _tenantRepository = tenantRepository;
        }


        [UnitOfWork]
        public override void Execute(TenantCreationManagerArgs args)
        {
            if (args.TenantDto.Id > 0)
            {
                // do stuff
            }
        }

    }

    public class TenantCreationManagerArgs
    {
        public TenantDto TenantDto { get; set; }
    }
}

