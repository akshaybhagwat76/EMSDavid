using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ems.MultiTenancy.HostDashboard.Dto;

namespace Ems.MultiTenancy.HostDashboard
{
    public interface IIncomeStatisticsService
    {
        Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval);
    }
}