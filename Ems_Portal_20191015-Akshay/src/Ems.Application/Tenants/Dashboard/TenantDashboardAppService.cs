using Abp.Auditing;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Ems.Assets;
using Ems.Authorization;
using Ems.Finance;
using Ems.Logging.Dto;
using Ems.Metrics;
using Ems.MultiTenancy;
using Ems.Quotations;
using Ems.Support;
using Ems.Tenants.Dashboard.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ems.Tenants.Dashboard
{
    [DisableAuditing]
    [AbpAuthorize(AppPermissions.Pages_Main_Dashboard)]
    public class TenantDashboardAppService : EmsAppServiceBase, ITenantDashboardAppService
    {
        private readonly IRepository<AssetOwnership> _assetOwnershipRepository;
        private readonly IRepository<Quotation> _quotationRepository;
        private readonly IRepository<Incident> _incidentRepository;
        private readonly IRepository<Estimate> _estimateRepository;
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IRepository<LeaseItem> _leaseItemRepository;
        private readonly IRepository<Uom> _uomRepository;
        private readonly IRepository<AgedReceivablesPeriod> _agedReceivablesPeriodRepository;

        public TenantDashboardAppService(
            IRepository<AssetOwnership> assetOwnershipRepository,
            IRepository<AgedReceivablesPeriod> agedReceivablesPeriodRepository, 
            IRepository<Quotation> quotationRepository,
            IRepository<Incident> incidentRepository,
            IRepository<Estimate> estimateRepository,
            IRepository<WorkOrder> workOrderRepository,
            IRepository<LeaseItem> leaseItemRepository, 
            IRepository<Uom> uomRepository
            )
        {
            _assetOwnershipRepository = assetOwnershipRepository;
            _leaseItemRepository = leaseItemRepository;
            _quotationRepository = quotationRepository;
            _estimateRepository = estimateRepository;
            _incidentRepository = incidentRepository;
            _workOrderRepository = workOrderRepository;
            _uomRepository = uomRepository;
            _agedReceivablesPeriodRepository = agedReceivablesPeriodRepository;
        }

        public GetMemberActivityOutput GetMemberActivity()
        {
            return new GetMemberActivityOutput
            (
                DashboardRandomDataGenerator.GenerateMemberActivities()
            );
        }

        private List<int> GetQuotationsInfo(TenantInfo tenantInfo)
        {
            var tenantType = tenantInfo.Tenant.TenantType;

            var assetsOwned = _assetOwnershipRepository.GetAll()
                .Include(a => a.AssetFk)
                .WhereIf(tenantType == "A", a => a.AssetOwnerId == tenantInfo.AssetOwner.Id)  // Filter out any Assets that are not relevant to the AssetOwener
                .Where(a => !a.IsDeleted)
                .Select(a => a.AssetId)
                .ToList();

            var quotationsInLastTwoMonths = _quotationRepository.GetAll()
                .Include(q => q.AssetFk)
                .Where(q => q.CreationTime > DateTime.Now.AddDays(-60))
                .Where(q => !q.IsDeleted)
                .Where(q => q.SupportContractFk.AssetOwnerId == tenantInfo.AssetOwner.Id || assetsOwned.Contains(q.AssetId))
                .ToList();

            decimal quotationsThisMonthDec = quotationsInLastTwoMonths
                .Where(q => q.CreationTime > DateTime.Now.AddDays(-30))
                .Count();

            decimal previousMonth = quotationsInLastTwoMonths.Count() - quotationsThisMonthDec;

            quotationsThisMonthDec = (quotationsThisMonthDec == 0) ? (decimal)0.001 : quotationsThisMonthDec;
            previousMonth = (previousMonth == 0) ? (decimal)0.001 : previousMonth;

            decimal changeDec = (quotationsThisMonthDec > previousMonth) ? (previousMonth / quotationsThisMonthDec) : -(quotationsThisMonthDec / previousMonth);

            int change = (int)Math.Round(changeDec);
            int quotationsThisMonth = (int)Math.Round(quotationsThisMonthDec);

            return new List<int>() { quotationsThisMonth, change };
        }

        private List<int> GetIncidentsInfo(TenantInfo tenantInfo)
        {
            var tenantType = tenantInfo.Tenant.TenantType;

            var assetsOwned = _assetOwnershipRepository.GetAll()
                .Include(a => a.AssetFk)
                .WhereIf(tenantType == "A", a => a.AssetOwnerId == tenantInfo.AssetOwner.Id)  // Filter out any Assets that are not relevant to the AssetOwener
                .Where(a => !a.IsDeleted)
                .Select(a => a.AssetId)
                .ToList();

            var incidentsInLastTwoMonths = _incidentRepository.GetAll()
                .Include(q => q.AssetFk)
                .Where(q => q.CreationTime > DateTime.Now.AddDays(-60))
                .Where(q => !q.IsDeleted)
                .Where(q => assetsOwned.Contains(q.AssetId))
                .ToList();

            decimal incidentsThisMonthDec = incidentsInLastTwoMonths
                .Where(q => q.CreationTime > DateTime.Now.AddDays(-30))
                .Count();

            decimal previousMonth = incidentsInLastTwoMonths.Count() - incidentsThisMonthDec;

            incidentsThisMonthDec = (incidentsThisMonthDec == 0) ? (decimal)0.001 : incidentsThisMonthDec;
            previousMonth = (previousMonth == 0) ? (decimal)0.001 : previousMonth;

            decimal changeDec = (incidentsThisMonthDec > previousMonth) ? (previousMonth / incidentsThisMonthDec) : -(incidentsThisMonthDec / previousMonth);

            int change = (int)Math.Round(changeDec);
            int incidentsThisMonth = (int)Math.Round(incidentsThisMonthDec);

            return new List<int>() { incidentsThisMonth, change };
        }

        private List<int> GetEstimatesInfo(TenantInfo tenantInfo)
        {
            var tenantType = tenantInfo.Tenant.TenantType;

            var estimatesInLastTwoMonths = _estimateRepository.GetAll()
                .Where(q => q.TenantId == tenantInfo.Tenant.Id)
                .Where(q => q.CreationTime > DateTime.Now.AddDays(-60))
                .Where(q => !q.IsDeleted)
                .ToList();

            decimal estimatesThisMonthDec = estimatesInLastTwoMonths
                .Where(q => q.CreationTime > DateTime.Now.AddDays(-30))
                .Count();

            decimal previousMonth = estimatesInLastTwoMonths.Count() - estimatesThisMonthDec;

            estimatesThisMonthDec = (estimatesThisMonthDec == 0) ? (decimal)0.001 : estimatesThisMonthDec;
            previousMonth = (previousMonth == 0) ? (decimal)0.001 : previousMonth;

            decimal changeDec = (estimatesThisMonthDec > previousMonth) ? (previousMonth / estimatesThisMonthDec) : -(estimatesThisMonthDec / previousMonth);

            int change = (int)Math.Round(changeDec);
            int estimatesThisMonth = (int)Math.Round(estimatesThisMonthDec);

            return new List<int>() { estimatesThisMonth, change };
        }

        private List<int> GetWorkOrdersInfo(TenantInfo tenantInfo)
        {
            var tenantType = tenantInfo.Tenant.TenantType;

            var assetsOwned = _assetOwnershipRepository.GetAll()
                .Include(a => a.AssetFk)
                .WhereIf(tenantType == "A", a => a.AssetOwnerId == tenantInfo.AssetOwner.Id)  // Filter out any Assets that are not relevant to the AssetOwener
                .Where(a => !a.IsDeleted)
                .Select(a => a.AssetId)
                .ToList();

            var workOrdersInLastTwoMonths = _workOrderRepository.GetAll()
                .Include(q => q.AssetOwnershipFk)
                .Where(q => q.CreationTime > DateTime.Now.AddDays(-60))
                .Where(q => !q.IsDeleted)
                .Where(q => assetsOwned.Contains(q.AssetOwnershipFk.AssetId))
                .ToList();

            decimal workOrdersThisMonthDec = workOrdersInLastTwoMonths
                .Where(q => q.CreationTime > DateTime.Now.AddDays(-30))
                .Count();

            decimal previousMonth = workOrdersInLastTwoMonths.Count() - workOrdersThisMonthDec;

            workOrdersThisMonthDec = (workOrdersThisMonthDec == 0) ? (decimal)0.001 : workOrdersThisMonthDec;
            previousMonth = (previousMonth == 0) ? (decimal)0.001 : previousMonth;

            decimal changeDec = (workOrdersThisMonthDec > previousMonth) ? (previousMonth / workOrdersThisMonthDec) : -(workOrdersThisMonthDec / previousMonth);

            int change = (int)Math.Round(changeDec);
            int workOrdersThisMonth = (int)Math.Round(workOrdersThisMonthDec);

            return new List<int>() { workOrdersThisMonth, change };
        }

        public GetDashboardDataOutput GetDashboardData(GetDashboardDataInput input)
        {
            var tenantInfo = TenantManager.GetTenantInfo().Result;

            var quotations = GetQuotationsInfo(tenantInfo);
            var incidents = GetIncidentsInfo(tenantInfo);
            var estimates = GetEstimatesInfo(tenantInfo);
            var workOrders = GetWorkOrdersInfo(tenantInfo);

            var agedReceivables = GetAgedReceivablesData(input.AgedReceivablesDatePeriod, tenantInfo);
            var revenueForecast = GetRevenueForecastData(tenantInfo);

            var newEstimates = estimates[0];
            var newEstimatesChange = estimates[1];
            var newWorkOrders = workOrders[0];
            var newWorkOrdersChange = workOrders[1];
            var newIncidents = incidents[0];
            var newIncidentsChange = incidents[1];
            var newQuotations = quotations[0];
            var newQuotationsChange = quotations[1];

            var output = new GetDashboardDataOutput
            {
                NewEstimates = newEstimates   , // DashboardRandomDataGenerator.GetRandomInt(5, 10),
                NewEstimatesChange = newEstimatesChange, // DashboardRandomDataGenerator.GetRandomInt(30, 80),
                NewWorkOrders = newWorkOrders, // DashboardRandomDataGenerator.GetRandomInt(5, 10),
                NewWorkOrdersChange = newWorkOrdersChange, // DashboardRandomDataGenerator.GetRandomInt(30, 80),
                NewIncidents = newIncidents, // DashboardRandomDataGenerator.GetRandomInt(5, 10),
                NewIncidentsChange = newIncidentsChange, // DashboardRandomDataGenerator.GetRandomInt(30, 80),
                NewQuotations = newQuotations, // DashboardRandomDataGenerator.GetRandomInt(5, 10),
                NewQuotationsChange = newQuotationsChange, //DashboardRandomDataGenerator.GetRandomInt(30, 80),
                AgedReceivables = agedReceivables,
                RevenueForecast = revenueForecast,

                TotalProfit = DashboardRandomDataGenerator.GetRandomInt(500000, 900000),
                NewFeedbacks = DashboardRandomDataGenerator.GetRandomInt(1000, 5000),
                NewOrders = DashboardRandomDataGenerator.GetRandomInt(100, 900),
                NewUsers = DashboardRandomDataGenerator.GetRandomInt(50, 500),
                SalesSummary = DashboardRandomDataGenerator.GenerateSalesSummaryData(input.SalesSummaryDatePeriod),
                Expenses = DashboardRandomDataGenerator.GetRandomInt(5000, 10000),
                Growth = DashboardRandomDataGenerator.GetRandomInt(5000, 10000),
                Revenue = DashboardRandomDataGenerator.GetRandomInt(1000, 9000),
                TotalSales = DashboardRandomDataGenerator.GetRandomInt(10000, 90000),
                TransactionPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                NewVisitPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                BouncePercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                DailySales = DashboardRandomDataGenerator.GetRandomArray(30, 10, 50),
                ProfitShares = DashboardRandomDataGenerator.GetRandomPercentageArray(3)
            };
            return output;
        }

        public GetSalesSummaryOutput GetSalesSummary(GetSalesSummaryInput input)
        {
            return new GetSalesSummaryOutput(DashboardRandomDataGenerator.GenerateSalesSummaryData(input.SalesSummaryDatePeriod));
        }

        public GetRevenueForecastOutput GetRevenueForecast(TenantInfo tenantInfo)
        {
            return new GetRevenueForecastOutput(GetRevenueForecastData(tenantInfo));
        }

        public GetAgedReceivablesOutput GetAgedReceivables(GetAgedReceivablesInput input, TenantInfo tenantInfo)
        {
            return new GetAgedReceivablesOutput(GetAgedReceivablesData(input.AgedReceivablesDatePeriod, tenantInfo));
        }

        public GetRegionalStatsOutput GetRegionalStats()
        {
            return new GetRegionalStatsOutput(DashboardRandomDataGenerator.GenerateRegionalStat());
        }

        public GetGeneralStatsOutput GetGeneralStats()
        {
            return new GetGeneralStatsOutput
            {
                TransactionPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                NewVisitPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                BouncePercent = DashboardRandomDataGenerator.GetRandomInt(10, 100)
            };
        }
        
        private List<RevenueForecastData> GetRevenueForecastData(TenantInfo tenantInfo)
        {
            var tenantType = tenantInfo.Tenant.TenantType;
            int? monthUomId = _uomRepository.GetAll().Where(u => u.UnitOfMeasurement == "Month").FirstOrDefault().Id;

            if (monthUomId == null || monthUomId == 0)
            {
                throw new Exception("No UOM ID found that matches 'Month'.");
            };

            List<RevenueForecastData> data = new List<RevenueForecastData>();

            var allLeaseItems = _leaseItemRepository.GetAll()
                .Include(e => e.LeaseAgreementFk)
                .WhereIf(tenantType == "A", e => e.LeaseAgreementFk.AssetOwnerId == tenantInfo.AssetOwner.Id)  // Filter out any LeaseItems that are not relevant to the AssetOwener
                .Where(i => !i.IsDeleted)
                .Where(i => i.RentalUomRefId == monthUomId)
                .ToList();

            List<DateTime> periods = new List<DateTime>()
            {
                DateTime.Now.AddMonths(1),
                DateTime.Now.AddMonths(2),
                DateTime.Now.AddMonths(3),
                DateTime.Now.AddMonths(4),
                DateTime.Now.AddMonths(5),
                DateTime.Now.AddMonths(6),
                DateTime.Now.AddMonths(7),
                DateTime.Now.AddMonths(8),
                DateTime.Now.AddMonths(9),
                DateTime.Now.AddMonths(10),
                DateTime.Now.AddMonths(11),
                DateTime.Now.AddMonths(12)
            };

            foreach (var period in periods)
            {
                var monthlyIncome = allLeaseItems.Where(i => i.StartDate <= period && i.EndDate >= period).Sum(i => i.UnitRentalRate);
                data.Add(new RevenueForecastData(period.ToString("MMM yyyy"), Convert.ToInt32(monthlyIncome)));
            };

            return data;
        }

        private List<AgedReceivablesData> GetAgedReceivablesData(AgedReceivablesDatePeriod inputAgedDebtorDatePeriod, TenantInfo tenantInfo)
        {


            List<AgedReceivablesData> data = new List<AgedReceivablesData>();
            string dateFormat = "MMM-yyyy";
            var allPeriodData = _agedReceivablesPeriodRepository.GetAll().Where(a => a.TenantId == tenantInfo.Tenant.Id).ToList();

            if (allPeriodData.Count > 0)
            {
                var latestPeriod = allPeriodData.OrderByDescending(p => p.Period).Select(p => p.Period).FirstOrDefault();

                List<DateTime> periods = new List<DateTime>()
                {
                    latestPeriod.AddMonths(-11),
                    latestPeriod.AddMonths(-10),
                    latestPeriod.AddMonths(-9),
                    latestPeriod.AddMonths(-8),
                    latestPeriod.AddMonths(-7),
                    latestPeriod.AddMonths(-6),
                    latestPeriod.AddMonths(-5),
                    latestPeriod.AddMonths(-4),
                    latestPeriod.AddMonths(-3),
                    latestPeriod.AddMonths(-2),
                    latestPeriod.AddMonths(-1),
                    latestPeriod
                };

                foreach(var period in periods)
                {
                
                    var periodData = allPeriodData.Where(r => r.Period.Year == period.Year && r.Period.Month == period.Month).FirstOrDefault();

                    var newPeriod = new AgedReceivablesData(period.ToString(dateFormat), (int)periodData.Current, (int)periodData.Over30, (int)periodData.Over60, (int)periodData.Over90, (int)periodData.Over120);
                    data.Add(newPeriod);
                }
            }
            

             /*
            Random random = new Random();
            switch (inputAgedDebtorDatePeriod)
            {
                case AgedReceivablesDatePeriod.Daily:
                    data = new List<AgedReceivablesData>
                    {
                        new AgedReceivablesData(DateTime.Now.AddDays(-5).ToString(dateFormat), random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000)),
                        new AgedReceivablesData(DateTime.Now.AddDays(-4).ToString(dateFormat), random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000)),
                        new AgedReceivablesData(DateTime.Now.AddDays(-3).ToString(dateFormat), random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000)),
                        new AgedReceivablesData(DateTime.Now.AddDays(-2).ToString(dateFormat), random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000)),
                        new AgedReceivablesData(DateTime.Now.AddDays(-1).ToString(dateFormat), random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000))
                    };
                    break;

                case AgedReceivablesDatePeriod.Weekly:
                    var lastYear = DateTime.Now.AddYears(-1).Year;
                    data = new List<AgedReceivablesData>
                    {
                        new AgedReceivablesData(lastYear + " W4", random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000)),
                        new AgedReceivablesData(lastYear + " W3", random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000)),
                        new AgedReceivablesData(lastYear + " W2", random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000)),
                        new AgedReceivablesData(lastYear + " W1", random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000))
                    };
                    break;

                case AgedReceivablesDatePeriod.Monthly:
                default:

                    data = new List<AgedReceivablesData>
                    {
                        new AgedReceivablesData(DateTime.Now.AddMonths(-4).ToString("MMM yyyy"), random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000)),
                        new AgedReceivablesData(DateTime.Now.AddMonths(-3).ToString("MMM yyyy"), random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000)),
                        new AgedReceivablesData(DateTime.Now.AddMonths(-2).ToString("MMM yyyy"), random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000)),
                        new AgedReceivablesData(DateTime.Now.AddMonths(-1).ToString("MMM yyyy"), random.Next(1000, 2000),
                            random.Next(100, 999), random.Next(1000, 2000), random.Next(100, 999), random.Next(1000, 2000)),
                    };
                    break;
            }
            */
            return data;
        }
    }
}