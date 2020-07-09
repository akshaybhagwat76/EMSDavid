import { AfterViewInit, Component, Injector, ViewEncapsulation, ViewChild  } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantDashboardServiceProxy, SalesSummaryDatePeriod, AgedReceivablesDatePeriod } from '@shared/service-proxies/service-proxies';
import { curveBasis } from 'd3-shape';

import { Table } from 'primeng/components/table/table';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { AssetsServiceProxy, WorkOrderUpdatesServiceProxy } from '@shared/service-proxies/service-proxies';
//import { Paginator } from 'primeng/components/paginator/paginator';

//import { ActivatedRoute } from '@angular/router';

import * as _ from 'lodash';


@Component({
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})

export class DashboardComponent extends AppComponentBase implements AfterViewInit {

    @ViewChild('dataTable', { static: true }) dataTable: Table; // For Assets with Workorders
    @ViewChild('dataTable1', { static: true }) dataTable1: Table; // For Support Items Consumed

    primengTableHelper1: PrimengTableHelper;

    appSalesSummaryDateInterval = SalesSummaryDatePeriod;
    appAgedReceivablesDateInterval = AgedReceivablesDatePeriod;
    selectedSalesSummaryDatePeriod: any = SalesSummaryDatePeriod.Daily;
    selectedAgedReceivablesDatePeriod: any = AgedReceivablesDatePeriod.Monthly;
    dashboardHeaderStats: DashboardHeaderStats;
    salesSummaryChart: SalesSummaryChart;
    agedReceivablesChart: AgedReceivablesChart;
    revenueForecastChart: RevenueForecastChart;
    regionalStatsTable: RegionalStatsTable;
    generalStatsPieChart: GeneralStatsPieChart;
    dailySalesLineChart: DailySalesLineChart;
    profitSharePieChart: ProfitSharePieChart;
    memberActivityTable: MemberActivityTable;

    filterText = '';
    classFilter = '';
    assetTypeTypeFilter = '';
    //days = 30;

    constructor(
        injector: Injector,
        private _dashboardService: TenantDashboardServiceProxy,
        private _assetsServiceProxy: AssetsServiceProxy,
        private _workorderUpdatesServiceProxy : WorkOrderUpdatesServiceProxy
    ) {
        super(injector);
        this.dashboardHeaderStats = new DashboardHeaderStats();
        this.salesSummaryChart = new SalesSummaryChart(this._dashboardService);
        this.agedReceivablesChart = new AgedReceivablesChart(this._dashboardService);
        this.revenueForecastChart = new RevenueForecastChart(this._dashboardService);
        this.regionalStatsTable = new RegionalStatsTable(this._dashboardService);
        this.generalStatsPieChart = new GeneralStatsPieChart(this._dashboardService);
        this.dailySalesLineChart = new DailySalesLineChart(this._dashboardService);
        this.profitSharePieChart = new ProfitSharePieChart(this._dashboardService);
        this.memberActivityTable = new MemberActivityTable(this._dashboardService);
        this.primengTableHelper1 = new PrimengTableHelper(); // For Support Items Consumed
    }

    getWorkorderItems(event?: LazyLoadEvent) {
        this.updateWorkorderItems(30);
    }

    updateWorkorderItems(days: number)
    {
        this._workorderUpdatesServiceProxy.getWorkorderItems(
            days,
            this.primengTableHelper1.getSorting(this.dataTable),
            0, 10
        ).subscribe(result => {
            this.primengTableHelper1.records = result;
            this.primengTableHelper1.hideLoadingIndicator();
        });
    }

    getAssetsWithWorkorders(event?: LazyLoadEvent) {
        this.updateAssetsWithWorkorders(30);
    }
    
    updateAssetsWithWorkorders(days: number)
    {
        this._assetsServiceProxy.getAssetsWithWorkorders(
            days,
            this.primengTableHelper.getSorting(this.dataTable),
            0, 10
        ).subscribe(result => {
            this.primengTableHelper.records = result;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    getDashboardStatisticsData(salesSummaryDatePeriod, agedReceivablesDatePeriod): void {
        this.salesSummaryChart.showLoading();
        this.agedReceivablesChart.showLoading();
        this.generalStatsPieChart.showLoading();

        this._dashboardService
            .getDashboardData(salesSummaryDatePeriod, agedReceivablesDatePeriod )
            .subscribe(result => {
                this.dashboardHeaderStats.init(result.newIncidents, result.newIncidentsChange, result.newWorkOrders, result.newWorkOrdersChange, result.newQuotations, result.newQuotationsChange, result.newEstimates, result.newEstimatesChange);
                this.generalStatsPieChart.init(result.transactionPercent, result.newVisitPercent, result.bouncePercent);
                this.dailySalesLineChart.init(result.dailySales);
                this.profitSharePieChart.init(result.profitShares);
                this.salesSummaryChart.init(result.salesSummary, result.totalSales, result.revenue, result.expenses, result.growth);
                this.agedReceivablesChart.init(result.agedReceivables, result.totalSales, result.revenue, result.expenses, result.growth ,
                        result.agedReceivablesDate1, result.agedReceivablesTotal1,
                        result.agedReceivablesDate2, result.agedReceivablesTotal2,
                        result.agedReceivablesDate3, result.agedReceivablesTotal3,
                        result.agedReceivablesDate4, result.agedReceivablesTotal4),
                this.revenueForecastChart.init(result.revenueForecast);
            });
    }

    ngAfterViewInit(): void {
        this.getDashboardStatisticsData(SalesSummaryDatePeriod.Daily, AgedReceivablesDatePeriod.Monthly);
        this.regionalStatsTable.init();
        this.memberActivityTable.init();
    }
}


abstract class DashboardChartBase {
    loading = true;

    showLoading() {
        setTimeout(() => { this.loading = true; });
    }

    hideLoading() {
        setTimeout(() => { this.loading = false; });
    }
}

class SalesSummaryChart extends DashboardChartBase {
    totalSales = 0; totalSalesCounter = 0;
    revenue = 0; revenuesCounter = 0;
    expenses = 0; expensesCounter = 0;
    growth = 0; growthCounter = 0;

    selectedDatePeriod: SalesSummaryDatePeriod = SalesSummaryDatePeriod.Daily;

    data = [];

    constructor(
        private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init(salesSummaryData, totalSales, revenue, expenses, growth) {
        this.totalSales = totalSales;
        this.totalSalesCounter = totalSales;

        this.revenue = revenue;
        this.expenses = expenses;
        this.growth = growth;

        this.setChartData(salesSummaryData);

        this.hideLoading();
    }

    setChartData(items): void {
        let sales = [];
        let profit = [];

        _.forEach(items, (item) => {

            sales.push({
                'name': item['period'],
                'value': item['sales']
            });

            profit.push({
                'name': item['period'],
                'value': item['profit']
            });
        });

        this.data = [
            {
                'name': 'Sales',
                'series': sales
            }, {
                'name': 'Profit',
                'series': profit
            }
        ];
    }

    reload(datePeriod) {
        this.selectedDatePeriod = datePeriod;

        this.showLoading();
        this._dashboardService
            .getSalesSummary(datePeriod)
            .subscribe(result => {
                this.setChartData(result.salesSummary);
                this.hideLoading();
            });
    }
}


class AgedReceivablesChart extends DashboardChartBase {
    totalSales = 0; totalSalesCounter = 0;
    revenue = 0; revenuesCounter = 0;
    expenses = 0; expensesCounter = 0;
    growth = 0; growthCounter = 0;

    agedReceivablesDate1 = ""; agedReceivablesTotal1 = 0; agedReceivablesTotalCounter1 = 0;
    agedReceivablesDate2 = ""; agedReceivablesTotal2 = 0; agedReceivablesTotalCounter2 = 0;
    agedReceivablesDate3 = ""; agedReceivablesTotal3 = 0; agedReceivablesTotalCounter3 = 0;
    agedReceivablesDate4 = ""; agedReceivablesTotal4 = 0; agedReceivablesTotalCounter4 = 0;

    selectedDatePeriod: AgedReceivablesDatePeriod = AgedReceivablesDatePeriod.Monthly;

    data = [];

    constructor(
        private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init(agedReceivablesData, totalSales, revenue, expenses, growth, agedReceivablesDate1, agedReceivablesTotal1, agedReceivablesDate2, agedReceivablesTotal2, agedReceivablesDate3, agedReceivablesTotal3, agedReceivablesDate4, agedReceivablesTotal4) {
        this.totalSales = totalSales;
        this.totalSalesCounter = totalSales;

        this.revenue = revenue;
        this.expenses = expenses;
        this.growth = growth;

        this.agedReceivablesDate1 = agedReceivablesDate1;
        this.agedReceivablesTotal1 = agedReceivablesTotal1;
        this.agedReceivablesTotalCounter1 = agedReceivablesTotal1;

        this.agedReceivablesDate2 = agedReceivablesDate2;
        this.agedReceivablesTotal2 = agedReceivablesTotal2;
        this.agedReceivablesTotalCounter2 = agedReceivablesTotal2;

        this.agedReceivablesDate3 = agedReceivablesDate3;
        this.agedReceivablesTotal3 = agedReceivablesTotal3;
        this.agedReceivablesTotalCounter3 = agedReceivablesTotal3;

        this.agedReceivablesDate4 = agedReceivablesDate4;
        this.agedReceivablesTotal4 = agedReceivablesTotal4;
        this.agedReceivablesTotalCounter4 = agedReceivablesTotal4;

        this.setChartData(agedReceivablesData);

        this.hideLoading();
    }

    setChartData(items): void {
        let current = [];
        let over30 = [];
        let over60 = [];
        let over90 = [];
        let over120 = [];

        _.forEach(items, (item) => {

            current.push({
                'name': item['period'],
                'value': item['current']
            });

            over30.push({
                'name': item['period'],
                'value': item['over30']
            });

            over60.push({
                'name': item['period'],
                'value': item['over60']
            });

            over90.push({
                'name': item['period'],
                'value': item['over90']
            });

            over120.push({
                'name': item['period'],
                'value': item['over120']
            });
        });

        this.data = [
            {
                'name': 'Current',
                'series': current
            }, {
                'name': 'Over 30',
                'series': over30
            }, {
                'name': 'Over 60',
                'series': over60
            }, {
                'name': 'Over 90',
                'series': over90
            }, {
                'name': 'Over 120',
                'series': over120
            }
        ];
    }

    reload(datePeriod) {
        this.selectedDatePeriod = datePeriod;

        this.showLoading();
        this._dashboardService
            .getAgedReceivables(datePeriod)
            .subscribe(result => {
                this.setChartData(result.agedReceivables);
                this.hideLoading();
            });
    }
}


class RevenueForecastChart extends DashboardChartBase {

    revenueForecastDate1 = ""; revenueForecastTotal1 = 0; revenueForecastTotalCounter1 = 0;
    revenueForecastDate2 = ""; revenueForecastTotal2 = 0; revenueForecastTotalCounter2 = 0;
    revenueForecastDate3 = ""; revenueForecastTotal3 = 0; revenueForecastTotalCounter3 = 0;
    revenueForecastDate4 = ""; revenueForecastTotal4 = 0; revenueForecastTotalCounter4 = 0;
    revenueForecastDate5 = ""; revenueForecastTotal5 = 0; revenueForecastTotalCounter5 = 0;
    revenueForecastDate6 = ""; revenueForecastTotal6 = 0; revenueForecastTotalCounter6 = 0;


    data = [];

    constructor(
        private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init(revenueForecastData) {

        this.revenueForecastDate1 = revenueForecastData[0].period;
        this.revenueForecastTotal1 = revenueForecastData[0].revenue;
        this.revenueForecastTotalCounter1 = this.revenueForecastTotal1

        this.revenueForecastDate2 = revenueForecastData[1].period;
        this.revenueForecastTotal2 = revenueForecastData[1].revenue;
        this.revenueForecastTotalCounter2 = this.revenueForecastTotal2

        this.revenueForecastDate3 = revenueForecastData[2].period;
        this.revenueForecastTotal3 = revenueForecastData[2].revenue;
        this.revenueForecastTotalCounter3 = this.revenueForecastTotal3

        this.revenueForecastDate4 = revenueForecastData[3].period;
        this.revenueForecastTotal4 = revenueForecastData[3].revenue;
        this.revenueForecastTotalCounter4 = this.revenueForecastTotal4

        this.revenueForecastDate5 = revenueForecastData[4].period;
        this.revenueForecastTotal5 = revenueForecastData[4].revenue;
        this.revenueForecastTotalCounter5 = this.revenueForecastTotal5

        this.revenueForecastDate6 = revenueForecastData[5].period;
        this.revenueForecastTotal6 = revenueForecastData[5].revenue;
        this.revenueForecastTotalCounter6 = this.revenueForecastTotal6

        this.setChartData(revenueForecastData);

        this.hideLoading();
    }

    setChartData(items): void {
        let formattedData = [];
        _.forEach(items, (item) => {

            formattedData.push({
                'name': item['period'],
                'value': item['revenue']
            });
        });
        this.data = formattedData;
    }

    reload() {

        this.showLoading();
        this._dashboardService
            .getRevenueForecast()
            .subscribe(result => {
                this.setChartData(result.revenueForecast);
                this.hideLoading();
            });
    }
}


class RegionalStatsTable extends DashboardChartBase {
    stats: Array<any>;
    colors = ['#00c5dc', '#f4516c', '#34bfa3', '#ffb822'];
    customColors = [
        { name: '1', value: '#00c5dc' },
        { name: '2', value: '#f4516c' },
        { name: '3', value: '#34bfa3' },
        { name: '4', value: '#ffb822' },
        { name: '5', value: '#00c5dc' }
    ];

    curve: any = curveBasis;

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init() {
        this.reload();
    }

    formatData(): any {
        for (let j = 0; j < this.stats.length; j++) {
            let stat = this.stats[j];

            let series = [];
            for (let i = 0; i < stat.change.length; i++) {
                series.push({
                    name: i + 1,
                    value: stat.change[i]
                });
            }

            stat.changeData = [
                {
                    'name': j + 1,
                    'series': series
                }
            ];

        }
    }

    reload() {
        this.showLoading();
        this._dashboardService
            .getRegionalStats()
            .subscribe(result => {
                this.stats = result.stats;
                this.formatData();
                this.hideLoading();
            });
    }
}

class GeneralStatsPieChart extends DashboardChartBase {

    public data = [];

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init(transactionPercent, newVisitPercent, bouncePercent) {
        this.data = [
            {
                'name': 'Operations',
                'value': transactionPercent
            }, {
                'name': 'New Visits',
                'value': newVisitPercent
            }, {
                'name': 'Bounce',
                'value': bouncePercent
            }];

        this.hideLoading();
    }

    reload() {
        this.showLoading();
        this._dashboardService
            .getGeneralStats()
            .subscribe(result => {
                this.init(result.transactionPercent, result.newVisitPercent, result.bouncePercent);
            });
    }
}

class DailySalesLineChart extends DashboardChartBase {

    chartData: any[];
    scheme: any = {
        name: 'green',
        selectable: true,
        group: 'Ordinal',
        domain: [
            '#34bfa3'
        ]
    };

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init(data) {
        this.chartData = [];
        for (let i = 0; i < data.length; i++) {
            this.chartData.push({
                name: i + 1,
                value: data[i]
            });
        }
    }

    reload() {
        this.showLoading();
        this._dashboardService
            .getSalesSummary(SalesSummaryDatePeriod.Monthly)
            .subscribe(result => {
                this.init(result.salesSummary);
                this.hideLoading();
            });
    }
}

class ProfitSharePieChart extends DashboardChartBase {

    chartData: any[] = [];
    scheme: any = {
        name: 'custom',
        selectable: true,
        group: 'Ordinal',
        domain: [
            '#00c5dc', '#ffb822', '#716aca'
        ]
    };

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init(data: number[]) {

        let formattedData = [];
        for (let i = 0; i < data.length; i++) {
            formattedData.push({
                'name': this.getChartItemName(i),
                'value': data[i]
            });
        }

        this.chartData = formattedData;
    }

    getChartItemName(index: number) {
        if (index === 0) {
            return 'Product Sales';
        }

        if (index === 1) {
            return 'Online Courses';
        }

        if (index === 2) {
            return 'Custom Development';
        }

        return 'Other';
    }
}

class DashboardHeaderStats extends DashboardChartBase {

    
    totalProfit = 0; totalProfitCounter = 0;
    newFeedbacks = 0; newFeedbacksCounter = 0;
    newOrders = 0; newOrdersCounter = 0;
    newUsers = 0; newUsersCounter = 0;

    totalProfitChange = 76; totalProfitChangeCounter = 0;
    newFeedbacksChange = 85; newFeedbacksChangeCounter = 0;
    newOrdersChange = 45; newOrdersChangeCounter = 0;
    newUsersChange = 57; newUsersChangeCounter = 0;

    /*
    init(totalProfit, newFeedbacks, newOrders, newUsers) {
        this.totalProfit = totalProfit;
        this.newFeedbacks = newFeedbacks;
        this.newOrders = newOrders;
        this.newUsers = newUsers;
        this.hideLoading();
    }
    */

    newIncidents = 0; newIncidentsCounter = 0;
    newWorkOrders = 0; newWorkOrdersCounter = 0;
    newQuotations = 0; newQuotationsCounter = 0;
    newEstimates = 0; newEstimatesCounter = 0;
    
    newIncidentsChange = 0; newIncidentsChangeCounter = 0; newIncidentProgressBar = "";
    newWorkOrdersChange = 0; newWorkOrdersChangeCounter = 0; newWorkOrdersProgressBar = "";
    newQuotationsChange = 0; newQuotationsChangeCounter = 0; newQuotationsProgressBar = "";
    newEstimatesChange = 0; newEstimatesChangeCounter = 0; newEstimatesProgressBar = "";

    init(newIncidents, newIncidentsChange, newWorkOrders, newWorkOrdersChange, newQuotations, newQuotationsChange, newEstimates, newEstimatesChange) {
        this.newIncidents = newIncidents;
        this.newWorkOrders = newWorkOrders;
        this.newQuotations = newQuotations;
        this.newEstimates = newEstimates;
        this.newIncidentsChange = newIncidentsChange;
        this.newIncidentProgressBar = Math.abs(this.newIncidentsChange).toString() + "%";
        this.newWorkOrdersChange = newWorkOrdersChange;
        this.newWorkOrdersProgressBar = Math.abs(this.newWorkOrdersChange).toString() + "%";
        this.newQuotationsChange = newQuotationsChange;
        this.newQuotationsProgressBar = Math.abs(this.newQuotationsChange).toString() + "%";
        this.newEstimatesChange = newEstimatesChange;
        this.newEstimatesProgressBar = Math.abs(this.newEstimatesChange).toString() + "%";
        this.hideLoading();
    }
}

class MemberActivityTable extends DashboardChartBase {

    memberActivities: Array<any>;

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init() {
        this.reload();
    }

    reload() {
        this.showLoading();
        this._dashboardService
            .getMemberActivity()
            .subscribe(result => {
                this.memberActivities = result.memberActivities;
                this.hideLoading();
            });
    }
}

