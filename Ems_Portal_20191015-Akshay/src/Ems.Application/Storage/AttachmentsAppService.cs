using Ems.Assets;
using Ems.Support;
using Ems.Quotations;
using Ems.Billing;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Storage.Exporting;
using Ems.Storage.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Ems.Authorization.Users;
using Abp.Domain.Uow;

namespace Ems.Storage
{
    [AbpAuthorize(AppPermissions.Pages_Main_Attachments)]
    public class AttachmentsAppService : EmsAppServiceBase, IAttachmentsAppService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Attachment> _attachmentRepository;
        private readonly IAttachmentsExcelExporter _attachmentsExcelExporter;
        private readonly IRepository<Asset, int> _lookup_assetRepository;
        private readonly IRepository<Incident, int> _lookup_incidentRepository;
        private readonly IRepository<LeaseAgreement, int> _lookup_leaseAgreementRepository;
        private readonly IRepository<Quotation, int> _lookup_quotationRepository;
        private readonly IRepository<SupportContract, int> _lookup_supportContractRepository;
        private readonly IRepository<WorkOrder, int> _lookup_workOrderRepository;
        private readonly IRepository<CustomerInvoice, int> _lookup_customerInvoiceRepository;
        private readonly IRepository<User, long> _lookup_userRepository;

        private readonly IRepository<CustomerInvoice> _customerInvoiceRepository;
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IRepository<SupportContract> _supportContractRepository;
        private readonly IRepository<SupportItem> _supportItemRepository;
        private readonly IRepository<Quotation> _quotationRepository;
        private readonly IRepository<LeaseAgreement> _leaseAgreementRepository;
        private readonly IRepository<LeaseItem> _leaseItemRepository;
        private readonly IRepository<Incident> _incidentRepository;
        private readonly IRepository<Asset> _assetRepository;


        public AttachmentsAppService( //IRepository<Attachment> attachmentRepository, IAttachmentsExcelExporter attachmentsExcelExporter , IRepository<Asset, int> lookup_assetRepository, IRepository<Incident, int> lookup_incidentRepository, IRepository<LeaseAgreement, int> lookup_leaseAgreementRepository, IRepository<Quotation, int> lookup_quotationRepository, IRepository<SupportContract, int> lookup_supportContractRepository, IRepository<WorkOrder, int> lookup_workOrderRepository, IRepository<CustomerInvoice, int> lookup_customerInvoiceRepository) 
            IUnitOfWorkManager unitOfWorkManager,
                IRepository<CustomerInvoice> customerInvoiceRepository,
                IRepository<WorkOrder> workOrderRepository,
                IRepository<SupportContract> supportContractRepository,
                IRepository<SupportItem> supportItemRepository,
                IRepository<Quotation> quotationRepository,
                IRepository<LeaseAgreement> leaseAgreementRepository,
                IRepository<LeaseItem> leaseItemRepository,
                IRepository<Incident> incidentRepository,
                IRepository<Asset> assetRepository,
                IRepository<Attachment> attachmentRepository,
                IAttachmentsExcelExporter attachmentsExcelExporter,
                IRepository<Asset, int> lookup_assetRepository,
                IRepository<Incident, int> lookup_incidentRepository,
                IRepository<LeaseAgreement, int> lookup_leaseAgreementRepository,
                IRepository<Quotation, int> lookup_quotationRepository,
                IRepository<SupportContract, int> lookup_supportContractRepository,
                IRepository<WorkOrder, int> lookup_workOrderRepository,
                IRepository<CustomerInvoice, int> lookup_customerInvoiceRepository,
                IRepository<User, long> lookup_userRepository
                )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _customerInvoiceRepository = customerInvoiceRepository;
            _workOrderRepository = workOrderRepository;
            _supportContractRepository = supportContractRepository;
            _supportItemRepository = supportItemRepository;
            _quotationRepository = quotationRepository;
            _leaseItemRepository = leaseItemRepository;
            _leaseAgreementRepository = leaseAgreementRepository;
            _assetRepository = assetRepository;
            _incidentRepository = incidentRepository;

            _attachmentRepository = attachmentRepository;
            _attachmentsExcelExporter = attachmentsExcelExporter;
            _lookup_assetRepository = lookup_assetRepository;
            _lookup_incidentRepository = lookup_incidentRepository;
            _lookup_leaseAgreementRepository = lookup_leaseAgreementRepository;
            _lookup_quotationRepository = lookup_quotationRepository;
            _lookup_supportContractRepository = lookup_supportContractRepository;
            _lookup_workOrderRepository = lookup_workOrderRepository;
            _lookup_customerInvoiceRepository = lookup_customerInvoiceRepository;
            _lookup_userRepository = lookup_userRepository;

        }

        public async Task<PagedResultDto<GetAttachmentForViewDto>> GetAll(GetAllAttachmentsInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var filteredAttachments = _attachmentRepository.GetAll()
                        .Include(e => e.AssetFk)
                        .Include(e => e.IncidentFk)
                        .Include(e => e.LeaseAgreementFk)
                        .Include(e => e.QuotationFk)
                        .Include(e => e.SupportContractFk)
                        .Include(e => e.WorkOrderFk)
                        .Include(e => e.CustomerInvoiceFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Filename.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.BlobFolder.Contains(input.Filter) || e.BlobId.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.FilenameFilter), e => e.Filename.ToLower() == input.FilenameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MinUploadedAtFilter != null, e => e.UploadedAt >= input.MinUploadedAtFilter)
                        .WhereIf(input.MaxUploadedAtFilter != null, e => e.UploadedAt <= input.MaxUploadedAtFilter)
                        .WhereIf(input.MinUploadedByFilter != null, e => e.UploadedBy >= input.MinUploadedByFilter)
                        .WhereIf(input.MaxUploadedByFilter != null, e => e.UploadedBy <= input.MaxUploadedByFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BlobFolderFilter), e => e.BlobFolder.ToLower() == input.BlobFolderFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BlobIdFilter), e => e.BlobId.ToLower() == input.BlobIdFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IncidentDescriptionFilter), e => e.IncidentFk != null && e.IncidentFk.Description.ToLower() == input.IncidentDescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LeaseAgreementReferenceFilter), e => e.LeaseAgreementFk != null && e.LeaseAgreementFk.Reference.ToLower() == input.LeaseAgreementReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationTitleFilter), e => e.QuotationFk != null && e.QuotationFk.Title.ToLower() == input.QuotationTitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SupportContractTitleFilter), e => e.SupportContractFk != null && e.SupportContractFk.Title.ToLower() == input.SupportContractTitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.ToLower() == input.WorkOrderSubjectFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerInvoiceDescriptionFilter), e => e.CustomerInvoiceFk != null && e.CustomerInvoiceFk.Description.ToLower() == input.CustomerInvoiceDescriptionFilter.ToLower().Trim());

                var pagedAndFilteredAttachments = filteredAttachments
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);

                var attachments = from o in pagedAndFilteredAttachments
                                  join o1 in _lookup_assetRepository.GetAll() on o.AssetId equals o1.Id into j1
                                  from s1 in j1.DefaultIfEmpty()

                                  join o2 in _lookup_incidentRepository.GetAll() on o.IncidentId equals o2.Id into j2
                                  from s2 in j2.DefaultIfEmpty()

                                  join o3 in _lookup_leaseAgreementRepository.GetAll() on o.LeaseAgreementId equals o3.Id into j3
                                  from s3 in j3.DefaultIfEmpty()

                                  join o4 in _lookup_quotationRepository.GetAll() on o.QuotationId equals o4.Id into j4
                                  from s4 in j4.DefaultIfEmpty()

                                  join o5 in _lookup_supportContractRepository.GetAll() on o.SupportContractId equals o5.Id into j5
                                  from s5 in j5.DefaultIfEmpty()

                                  join o6 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o6.Id into j6
                                  from s6 in j6.DefaultIfEmpty()

                                  join o7 in _lookup_customerInvoiceRepository.GetAll() on o.CustomerInvoiceId equals o7.Id into j7
                                  from s7 in j7.DefaultIfEmpty()

                                  join o8 in _lookup_userRepository.GetAll() on o.UploadedBy equals o8.Id into j8
                                  from s8 in j8.DefaultIfEmpty()

                                  select new GetAttachmentForViewDto()
                                  {
                                      Attachment = new AttachmentDto
                                      {
                                          Filename = o.Filename,
                                          Description = o.Description,
                                          UploadedAt = o.UploadedAt,
                                          UploadedBy = o.UploadedBy,
                                          BlobFolder = o.BlobFolder,
                                          BlobId = o.BlobId,
                                          Id = o.Id
                                      },
                                      AssetReference = s1 == null ? "" : s1.Reference.ToString(),
                                      IncidentDescription = s2 == null ? "" : s2.Description.ToString(),
                                      LeaseAgreementReference = s3 == null ? "" : s3.Reference.ToString(),
                                      QuotationTitle = s4 == null ? "" : s4.Title.ToString(),
                                      SupportContractTitle = s5 == null ? "" : s5.Title.ToString(),
                                      WorkOrderSubject = s6 == null ? "" : s6.Subject.ToString(),
                                      CustomerInvoiceDescription = s7 == null ? "" : s7.Description.ToString(),
                                      UploadedByName = s8 == null ? "" : s8.Name + " (" + s8.EmailAddress + ")"
                                  };

                var totalCount = await filteredAttachments.CountAsync();

                return new PagedResultDto<GetAttachmentForViewDto>(
                    totalCount,
                    await attachments.ToListAsync()
                );
            }
        }


        public async Task<PagedResultDto<GetAttachmentForViewDto>> GetSome(GetSomeAttachmentsInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var tenantType = tenantInfo.Tenant.TenantType;

                string relatedType = string.Empty;

                bool authForAsset = false;
                bool authForIncident = false;
                bool authForLeaseAgreement = false;
                bool authForQuotation = false;
                bool authForSupportContract = false;
                bool authForWorkOrder = false;


                // Check Authorization for Attachments related to Assets
                Asset relatedAsset = new Asset();
                if (input.RelatedEntity == "Asset")
                {
                    relatedAsset = _assetRepository.Get(input.ReferenceId);
                    SupportItem relatedSupportItem = _supportItemRepository.GetAll().Where(c => c.AssetId == relatedAsset.Id).Include(c => c.SupportContractFk).FirstOrDefault();

                    switch (tenantType)
                    {
                        case "A":
                            authForAsset = (relatedSupportItem?.SupportContractFk?.AssetOwnerId == tenantInfo.AssetOwner.Id) ? true : false;
                            break;
                        case "V":
                            authForAsset = (relatedSupportItem?.SupportContractFk?.VendorId == tenantInfo.Vendor.Id) ? true : false;
                            break;
                        case "C":
                            LeaseItem relatedLeaseItem = _leaseItemRepository.GetAll().Where(l => l.AssetId == relatedAsset.Id).Include(l => l.LeaseAgreementFk).FirstOrDefault();
                            authForAsset = (relatedLeaseItem?.LeaseAgreementFk?.CustomerId == tenantInfo.Customer.Id) ? true : false;
                            break;
                        default:
                            authForAsset = true;
                            break;
                    }
                }

                SupportContract relatedSupportContract = new SupportContract();
                if (input.RelatedEntity == "SupportContract")
                {
                    relatedSupportContract = _supportContractRepository.Get(input.ReferenceId);

                    switch (tenantType)
                    {
                        case "A":
                            authForSupportContract = (relatedSupportContract?.AssetOwnerId == tenantInfo.AssetOwner.Id) ? true : false;
                            break;
                        case "V":
                            authForSupportContract = (relatedSupportContract?.VendorId == tenantInfo.Vendor.Id) ? true : false;
                            break;
                        case "C":
                            List<int> relatedAssetIds = _supportItemRepository.GetAll().Where(c => c.SupportContractId == input.ReferenceId).Select(c => c.AssetId).ToList();
                            List<LeaseItem> relatedLeaseItems = _leaseItemRepository.GetAll().Where(l => relatedAssetIds.Contains((int)l.AssetId)).Include(l => l.LeaseAgreementFk).ToList();
                            List<int?> relatedCustomerIds = relatedLeaseItems.Select(l => l.LeaseAgreementFk).Select(l => l.CustomerId).ToList();
                            authForSupportContract = (relatedCustomerIds.Contains(tenantInfo.Customer.Id)) ? true : false;
                            break;
                        default:
                            authForSupportContract = true;
                            break;
                    }
                }

                WorkOrder relatedWorkOrder = new WorkOrder();
                if (input.RelatedEntity == "WorkOrder")
                {
                    relatedWorkOrder = _workOrderRepository.GetAll().Include(c => c.AssetOwnershipFk).Where(w => w.Id == input.ReferenceId).FirstOrDefault();

                    switch (tenantType)
                    {
                        case "A":
                            authForWorkOrder = (relatedWorkOrder?.AssetOwnershipFk?.AssetOwnerId == tenantInfo.AssetOwner.Id) ? true : false;
                            break;
                        case "V":
                            authForWorkOrder = (relatedWorkOrder?.VendorId == tenantInfo.Vendor.Id) ? true : false;
                            break;
                        case "C":
                            authForWorkOrder = (relatedWorkOrder?.CustomerId == tenantInfo.Customer.Id) ? true : false;
                            break;
                        default:
                            authForWorkOrder = true;
                            break;
                    }
                }

                // TODO: Complete Auth Check
                LeaseAgreement relatedLeaseAgreement = new LeaseAgreement();
                if (input.RelatedEntity == "LeaseAgreement")
                {
                    relatedLeaseAgreement = _leaseAgreementRepository.Get(input.ReferenceId);

                    if (relatedLeaseAgreement?.Id > 0)
                        authForLeaseAgreement = true;
                }

                // TODO: Complete Auth Check
                Incident relatedIncident = new Incident();
                if (input.RelatedEntity == "Incident")
                {
                    relatedIncident = _incidentRepository.Get(input.ReferenceId);

                    if (relatedIncident?.Id > 0)
                        authForIncident = true;
                }

                // TODO: Complete Auth Check
                Quotation relatedQuotation = new Quotation();
                if (input.RelatedEntity == "Quotation")
                {
                    relatedQuotation = _quotationRepository.Get(input.ReferenceId);

                    if (relatedQuotation?.Id > 0)
                        authForQuotation = true;
                }


                var filteredAttachments = _attachmentRepository.GetAll()
                            .Include(e => e.AssetFk)
                            .Include(e => e.IncidentFk)
                            .Include(e => e.LeaseAgreementFk)
                            .Include(e => e.QuotationFk)
                            .Include(e => e.SupportContractFk)
                            .Include(e => e.WorkOrderFk)
                            .Include(e => e.CustomerInvoiceFk)
                            .WhereIf((!authForAsset && !authForIncident && !authForLeaseAgreement && !authForQuotation && !authForSupportContract && !authForWorkOrder), e => e.Id == 0)
                            .WhereIf(authForAsset, e => e.AssetId == input.ReferenceId)
                            .WhereIf(authForIncident, e => e.IncidentId == input.ReferenceId)
                            .WhereIf(authForLeaseAgreement, e => e.LeaseAgreementId == input.ReferenceId)
                            .WhereIf(authForQuotation, e => e.QuotationId == input.ReferenceId)
                            .WhereIf(authForSupportContract, e => e.SupportContractId == input.ReferenceId)
                            .WhereIf(authForWorkOrder, e => e.WorkOrderId == input.ReferenceId);

                var pagedAndFilteredAttachments = filteredAttachments
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);

                var attachments = from o in pagedAndFilteredAttachments
                                  join o1 in _lookup_assetRepository.GetAll() on o.AssetId equals o1.Id into j1
                                  from s1 in j1.DefaultIfEmpty()

                                  join o2 in _lookup_incidentRepository.GetAll() on o.IncidentId equals o2.Id into j2
                                  from s2 in j2.DefaultIfEmpty()

                                  join o3 in _lookup_leaseAgreementRepository.GetAll() on o.LeaseAgreementId equals o3.Id into j3
                                  from s3 in j3.DefaultIfEmpty()

                                  join o4 in _lookup_quotationRepository.GetAll() on o.QuotationId equals o4.Id into j4
                                  from s4 in j4.DefaultIfEmpty()

                                  join o5 in _lookup_supportContractRepository.GetAll() on o.SupportContractId equals o5.Id into j5
                                  from s5 in j5.DefaultIfEmpty()

                                  join o6 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o6.Id into j6
                                  from s6 in j6.DefaultIfEmpty()

                                  join o7 in _lookup_customerInvoiceRepository.GetAll() on o.IncidentId equals o7.Id into j7
                                  from s7 in j7.DefaultIfEmpty()

                                  join o8 in _lookup_userRepository.GetAll() on o.UploadedBy equals o8.Id into j8
                                  from s8 in j8.DefaultIfEmpty()

                                  select new GetAttachmentForViewDto()
                                  {
                                      Attachment = new AttachmentDto
                                      {
                                          Filename = o.Filename,
                                          Description = o.Description,
                                          UploadedAt = o.UploadedAt,
                                          UploadedBy = o.UploadedBy,
                                          BlobFolder = o.BlobFolder,
                                          BlobId = o.BlobId,
                                          Id = o.Id
                                      },

                                      AssetReference = s1 == null ? "" : s1.Reference.ToString(),
                                      IncidentDescription = s2 == null ? "" : s2.Description.ToString(),
                                      LeaseAgreementReference = s3 == null ? "" : s3.Reference.ToString(),
                                      QuotationTitle = s4 == null ? "" : s4.Title.ToString(),
                                      SupportContractTitle = s5 == null ? "" : s5.Title.ToString(),
                                      WorkOrderSubject = s6 == null ? "" : s6.Subject.ToString(),
                                      CustomerInvoiceDescription = s7 == null ? "" : s7.Description.ToString(),
                                      UploadedByName = s8 == null ? "" : s8.Name + " (" + s8.EmailAddress + ")"

                                      //AssetReference = relatedAsset.Reference.ToString(),
                                      //IncidentDescription = relatedIncident.Description.ToString(),
                                      //LeaseAgreementReference = relatedLeaseAgreement.Reference.ToString(),
                                      //QuotationTitle = relatedQuotation.Title.ToString(),
                                      //SupportContractTitle = relatedSupportContract.Title.ToString(),
                                      //WorkOrderSubject = relatedWorkOrder.Subject.ToString()
                                  };

                var totalCount = await filteredAttachments.CountAsync();
                var pagedResult = new PagedResultDto<GetAttachmentForViewDto>(
                    totalCount,
                    await attachments.ToListAsync());

                return pagedResult;
            }
        }



        public async Task<GetAttachmentForViewDto> GetAttachmentForView(int id)
        {
            var attachment = await _attachmentRepository.GetAsync(id);

            var output = new GetAttachmentForViewDto { Attachment = ObjectMapper.Map<AttachmentDto>(attachment) };

            if (output.Attachment.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.Attachment.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

            if (output.Attachment.IncidentId != null)
            {
                var _lookupIncident = await _lookup_incidentRepository.FirstOrDefaultAsync((int)output.Attachment.IncidentId);
                output.IncidentDescription = _lookupIncident.Description.ToString();
            }

            if (output.Attachment.LeaseAgreementId != null)
            {
                var _lookupLeaseAgreement = await _lookup_leaseAgreementRepository.FirstOrDefaultAsync((int)output.Attachment.LeaseAgreementId);
                output.LeaseAgreementReference = _lookupLeaseAgreement.Reference.ToString();
            }

            if (output.Attachment.QuotationId != null)
            {
                var _lookupQuotation = await _lookup_quotationRepository.FirstOrDefaultAsync((int)output.Attachment.QuotationId);
                output.QuotationTitle = _lookupQuotation.Title.ToString();
            }

            if (output.Attachment.SupportContractId != null)
            {
                var _lookupSupportContract = await _lookup_supportContractRepository.FirstOrDefaultAsync((int)output.Attachment.SupportContractId);
                output.SupportContractTitle = _lookupSupportContract.Title.ToString();
            }

            if (output.Attachment.WorkOrderId != null)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.Attachment.WorkOrderId);
                output.WorkOrderSubject = _lookupWorkOrder.Subject.ToString();
            }

            if (output.Attachment.CustomerInvoiceId != null)
            {
                var _lookupCustomerInvoice = await _lookup_customerInvoiceRepository.FirstOrDefaultAsync((int)output.Attachment.CustomerInvoiceId);
                output.CustomerInvoiceDescription = _lookupCustomerInvoice.Description.ToString();
            }

            if (output.Attachment.UploadedBy != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((int)output.Attachment.UploadedBy);
                output.UploadedByName = _lookupUser.Name + " (" + _lookupUser.EmailAddress + ")";
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Attachments_Edit)]
        public async Task<GetAttachmentForEditOutput> GetAttachmentForEdit(EntityDto input)
        {
            var attachment = await _attachmentRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetAttachmentForEditOutput { Attachment = ObjectMapper.Map<CreateOrEditAttachmentDto>(attachment) };

            if (output.Attachment.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.Attachment.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

            if (output.Attachment.IncidentId != null)
            {
                var _lookupIncident = await _lookup_incidentRepository.FirstOrDefaultAsync((int)output.Attachment.IncidentId);
                output.IncidentDescription = _lookupIncident.Description.ToString();
            }

            if (output.Attachment.LeaseAgreementId != null)
            {
                var _lookupLeaseAgreement = await _lookup_leaseAgreementRepository.FirstOrDefaultAsync((int)output.Attachment.LeaseAgreementId);
                output.LeaseAgreementReference = _lookupLeaseAgreement.Reference.ToString();
            }

            if (output.Attachment.QuotationId != null)
            {
                var _lookupQuotation = await _lookup_quotationRepository.FirstOrDefaultAsync((int)output.Attachment.QuotationId);
                output.QuotationTitle = _lookupQuotation.Title.ToString();
            }

            if (output.Attachment.SupportContractId != null)
            {
                var _lookupSupportContract = await _lookup_supportContractRepository.FirstOrDefaultAsync((int)output.Attachment.SupportContractId);
                output.SupportContractTitle = _lookupSupportContract.Title.ToString();
            }

            if (output.Attachment.WorkOrderId != null)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.Attachment.WorkOrderId);
                output.WorkOrderSubject = _lookupWorkOrder.Subject.ToString();
            }

            if (output.Attachment.CustomerInvoiceId != null)
            {
                var _lookupCustomerInvoice = await _lookup_customerInvoiceRepository.FirstOrDefaultAsync((int)output.Attachment.CustomerInvoiceId);
                output.CustomerInvoiceDescription = _lookupCustomerInvoice.Description.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditAttachmentDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Attachments_Create)]
        protected virtual async Task Create(CreateOrEditAttachmentDto input)
        {
            var attachment = ObjectMapper.Map<Attachment>(input);

            if (AbpSession.UserId != null)
                attachment.UploadedBy = (int?)AbpSession.UserId;

            if (AbpSession.TenantId != null)
                attachment.TenantId = (int?)AbpSession.TenantId;

            await _attachmentRepository.InsertAsync(attachment);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Attachments_Edit)]
        protected virtual async Task Update(CreateOrEditAttachmentDto input)
        {
            var attachment = await _attachmentRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, attachment);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Attachments_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _attachmentRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetAttachmentsToExcel(GetAllAttachmentsForExcelInput input)
        {

            var filteredAttachments = _attachmentRepository.GetAll()
                        .Include(e => e.AssetFk)
                        .Include(e => e.IncidentFk)
                        .Include(e => e.LeaseAgreementFk)
                        .Include(e => e.QuotationFk)
                        .Include(e => e.SupportContractFk)
                        .Include(e => e.WorkOrderFk)
                        .Include(e => e.CustomerInvoiceFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Filename.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.BlobFolder.Contains(input.Filter) || e.BlobId.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.FilenameFilter), e => e.Filename.ToLower() == input.FilenameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MinUploadedAtFilter != null, e => e.UploadedAt >= input.MinUploadedAtFilter)
                        .WhereIf(input.MaxUploadedAtFilter != null, e => e.UploadedAt <= input.MaxUploadedAtFilter)
                        .WhereIf(input.MinUploadedByFilter != null, e => e.UploadedBy >= input.MinUploadedByFilter)
                        .WhereIf(input.MaxUploadedByFilter != null, e => e.UploadedBy <= input.MaxUploadedByFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BlobFolderFilter), e => e.BlobFolder.ToLower() == input.BlobFolderFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BlobIdFilter), e => e.BlobId.ToLower() == input.BlobIdFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IncidentDescriptionFilter), e => e.IncidentFk != null && e.IncidentFk.Description.ToLower() == input.IncidentDescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LeaseAgreementReferenceFilter), e => e.LeaseAgreementFk != null && e.LeaseAgreementFk.Reference.ToLower() == input.LeaseAgreementReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationTitleFilter), e => e.QuotationFk != null && e.QuotationFk.Title.ToLower() == input.QuotationTitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SupportContractTitleFilter), e => e.SupportContractFk != null && e.SupportContractFk.Title.ToLower() == input.SupportContractTitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.ToLower() == input.WorkOrderSubjectFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerInvoiceDescriptionFilter), e => e.CustomerInvoiceFk != null && e.CustomerInvoiceFk.Description.ToLower() == input.CustomerInvoiceDescriptionFilter.ToLower().Trim());

            var query = (from o in filteredAttachments
                         join o1 in _lookup_assetRepository.GetAll() on o.AssetId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_incidentRepository.GetAll() on o.IncidentId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_leaseAgreementRepository.GetAll() on o.LeaseAgreementId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_quotationRepository.GetAll() on o.QuotationId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         join o5 in _lookup_supportContractRepository.GetAll() on o.SupportContractId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()

                         join o6 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()

                         join o7 in _lookup_customerInvoiceRepository.GetAll() on o.CustomerInvoiceId equals o7.Id into j7
                         from s7 in j7.DefaultIfEmpty()

                         select new GetAttachmentForViewDto()
                         {
                             Attachment = new AttachmentDto
                             {
                                 Filename = o.Filename,
                                 Description = o.Description,
                                 UploadedAt = o.UploadedAt,
                                 UploadedBy = o.UploadedBy,
                                 BlobFolder = o.BlobFolder,
                                 BlobId = o.BlobId,
                                 Id = o.Id
                             },
                             AssetReference = s1 == null ? "" : s1.Reference.ToString(),
                             IncidentDescription = s2 == null ? "" : s2.Description.ToString(),
                             LeaseAgreementReference = s3 == null ? "" : s3.Reference.ToString(),
                             QuotationTitle = s4 == null ? "" : s4.Title.ToString(),
                             SupportContractTitle = s5 == null ? "" : s5.Title.ToString(),
                             WorkOrderSubject = s6 == null ? "" : s6.Subject.ToString(),
                             CustomerInvoiceDescription = s7 == null ? "" : s7.Description.ToString()
                         });


            var attachmentListDtos = await query.ToListAsync();

            return _attachmentsExcelExporter.ExportToFile(attachmentListDtos);
        }



        [AbpAuthorize(AppPermissions.Pages_Main_Attachments)]
        public async Task<PagedResultDto<AttachmentAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_assetRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Reference.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AttachmentAssetLookupTableDto>();
            foreach (var asset in assetList)
            {
                lookupTableDtoList.Add(new AttachmentAssetLookupTableDto
                {
                    Id = asset.Id,
                    DisplayName = asset.Reference?.ToString()
                });
            }

            return new PagedResultDto<AttachmentAssetLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Attachments)]
        public async Task<PagedResultDto<AttachmentIncidentLookupTableDto>> GetAllIncidentForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_incidentRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Description.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var incidentList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AttachmentIncidentLookupTableDto>();
            foreach (var incident in incidentList)
            {
                lookupTableDtoList.Add(new AttachmentIncidentLookupTableDto
                {
                    Id = incident.Id,
                    DisplayName = incident.Description?.ToString()
                });
            }

            return new PagedResultDto<AttachmentIncidentLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Attachments)]
        public async Task<PagedResultDto<AttachmentLeaseAgreementLookupTableDto>> GetAllLeaseAgreementForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_leaseAgreementRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Reference.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var leaseAgreementList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AttachmentLeaseAgreementLookupTableDto>();
            foreach (var leaseAgreement in leaseAgreementList)
            {
                lookupTableDtoList.Add(new AttachmentLeaseAgreementLookupTableDto
                {
                    Id = leaseAgreement.Id,
                    DisplayName = leaseAgreement.Reference?.ToString()
                });
            }

            return new PagedResultDto<AttachmentLeaseAgreementLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Attachments)]
        public async Task<PagedResultDto<AttachmentQuotationLookupTableDto>> GetAllQuotationForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_quotationRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Title.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var quotationList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AttachmentQuotationLookupTableDto>();
            foreach (var quotation in quotationList)
            {
                lookupTableDtoList.Add(new AttachmentQuotationLookupTableDto
                {
                    Id = quotation.Id,
                    DisplayName = quotation.Title?.ToString()
                });
            }

            return new PagedResultDto<AttachmentQuotationLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Attachments)]
        public async Task<PagedResultDto<AttachmentSupportContractLookupTableDto>> GetAllSupportContractForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_supportContractRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Title.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var supportContractList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AttachmentSupportContractLookupTableDto>();
            foreach (var supportContract in supportContractList)
            {
                lookupTableDtoList.Add(new AttachmentSupportContractLookupTableDto
                {
                    Id = supportContract.Id,
                    DisplayName = supportContract.Title?.ToString()
                });
            }

            return new PagedResultDto<AttachmentSupportContractLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Attachments)]
        public async Task<PagedResultDto<AttachmentWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_workOrderRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Subject.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var workOrderList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AttachmentWorkOrderLookupTableDto>();
            foreach (var workOrder in workOrderList)
            {
                lookupTableDtoList.Add(new AttachmentWorkOrderLookupTableDto
                {
                    Id = workOrder.Id,
                    DisplayName = workOrder.Subject?.ToString()
                });
            }

            return new PagedResultDto<AttachmentWorkOrderLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Attachments)]
        public async Task<PagedResultDto<AttachmentCustomerInvoiceLookupTableDto>> GetAllCustomerInvoiceForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_customerInvoiceRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Description.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var customerInvoiceList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AttachmentCustomerInvoiceLookupTableDto>();
            foreach (var customerInvoice in customerInvoiceList)
            {
                lookupTableDtoList.Add(new AttachmentCustomerInvoiceLookupTableDto
                {
                    Id = customerInvoice.Id,
                    DisplayName = customerInvoice.Description?.ToString()
                });
            }

            return new PagedResultDto<AttachmentCustomerInvoiceLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}