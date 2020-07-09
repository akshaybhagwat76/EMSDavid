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

namespace Ems.Billing
{
    public class CustomerInvoiceManager : BackgroundJob<CustomerInvoiceManagerArgs>, IDomainService, ITransientDependency

    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<CustomerInvoice> _customerInvoiceRepository;
        private readonly IRepository<CustomerInvoiceDetail> _customerInvoiceDetailRepository;
        private readonly IRepository<Estimate> _estimateRepository;
        private readonly IRepository<EstimateDetail> _estimateDetailRepository;
        private readonly IRepository<XeroInvoice> _xeroInvoiceRepository;

        public CustomerInvoiceManager(
              IUnitOfWorkManager unitOfWorkManager,
              IRepository<CustomerInvoice> customerInvoiceRepository,
              IRepository<CustomerInvoiceDetail> customerInvoiceDetailRepository,
              IRepository<Estimate> estimateRepository,
              IRepository<EstimateDetail> estimateDetailRepository,
              IRepository<XeroInvoice> xeroInvoiceRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _customerInvoiceRepository = customerInvoiceRepository;
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository;
            _estimateRepository = estimateRepository;
            _estimateDetailRepository = estimateDetailRepository;
            _xeroInvoiceRepository = xeroInvoiceRepository;
        }


        [UnitOfWork]
        public override void Execute(CustomerInvoiceManagerArgs args)
        {
            if (args.CustomerInvoiceDto.Id > 0 && args.CustomerInvoiceDto.EstimateId > 0 && args.Created)
            {
                CloneAllEstimateDetails(args.CustomerInvoiceDto.Id, (int)args.CustomerInvoiceDto.EstimateId);
                GenerateXeroInvoice(args.CustomerInvoiceDto);
            }
        }

        [UnitOfWork]
        public virtual void GenerateXeroInvoice(CustomerInvoiceDto invoice)
        {

            // Akshay to implement this function

            // Use _xeroInvoiceRepository to store the results of the API call to Xero
        }

        public void CloneAllEstimateDetails(int customerInvoiceId, int estimateId)
        {
            var estimate = _estimateRepository.Get(estimateId);
            
            if (estimate != null)
            {
                var tenantId = estimate.TenantId;
                var estimateDetailList = _estimateDetailRepository.GetAll()
                    .Where(e => e.EstimateId == estimate.Id && !e.IsDeleted);

                if (estimateDetailList != null && estimateDetailList.Count() > 0)
                {
                    foreach (var item in estimateDetailList)
                    {
                        CustomerInvoiceDetail customerInvoiceDetail = new CustomerInvoiceDetail()
                        {
                            Charge = item.Charge,
                            CustomerInvoiceId = customerInvoiceId,
                            Description = item.Description,
                            Discount = item.Discount,
                            Gross = item.Cost,
                            Net = item.MarkUp,
                            Quantity = item.Quantity,
                            ItemTypeId = item.ItemTypeId,
                            UomId = item.UomId,
                            WorkOrderActionId = item.WorkOrderActionId,
                            Tax = item.Tax,
                            UnitPrice = item.UnitPrice
                        };

                        if (tenantId != null)
                            customerInvoiceDetail.TenantId = (int?)tenantId;

                        using (var unitOfWork = _unitOfWorkManager.Begin())
                        {
                            _customerInvoiceDetailRepository.InsertAndGetId(customerInvoiceDetail);
                            unitOfWork.Complete();
                            unitOfWork.Dispose();
                        }
                    }

                    UpdateCustomerInvoicePrices(customerInvoiceId);
                }
            }
        }

        private void UpdateCustomerInvoicePrices(int customerInvoiceId)
        {
            var invoice = _customerInvoiceRepository.Get(customerInvoiceId);

            if (invoice != null)
            {
                var invoiceDetailList = _customerInvoiceDetailRepository.GetAll()
                    .Where(e => e.CustomerInvoiceId == invoice.Id && !e.IsDeleted)
                    .ToList();

                if (invoiceDetailList != null && invoiceDetailList.Count() > 0)
                {
                    decimal totalPrice = 0, totalTax = 0, totalDiscount = 0, totalCharge = 0;

                    foreach (var item in invoiceDetailList)
                    {
                        decimal discountPrice = 0, taxPrice = 0, costPrice = 0;

                        costPrice = item.UnitPrice * item.Quantity;

                        if (item.Net > 0)
                            costPrice += costPrice * (item.Net / 100);

                        if (item.Discount > 0)
                            discountPrice = costPrice * ((decimal)item.Discount / 100);

                        if (item.Tax > 0)
                            taxPrice = (costPrice - discountPrice) * ((decimal)item.Tax / 100);

                        totalDiscount += discountPrice;
                        totalTax += taxPrice;
                        totalPrice += costPrice;
                        totalCharge += (costPrice - discountPrice) + taxPrice;
                    }

                    invoice.TotalPrice = totalPrice;
                    invoice.TotalTax = totalTax;
                    invoice.TotalDiscount = totalDiscount;
                    invoice.TotalCharge = totalCharge;
                }
                else
                {
                    invoice.TotalPrice = 0;
                    invoice.TotalTax = 0;
                    invoice.TotalDiscount = 0;
                    invoice.TotalCharge = 0;
                }

                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    _customerInvoiceRepository.Update(invoice);
                    unitOfWork.Complete();
                    unitOfWork.Dispose();
                }
                
            }
        }
    }

    public class CustomerInvoiceManagerArgs
    {
        public bool Created { get; set; }
        public CustomerInvoiceDto CustomerInvoiceDto { get; set; }
    }
}

