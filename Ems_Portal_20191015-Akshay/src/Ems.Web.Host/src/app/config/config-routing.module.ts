import { NgModule } from '@angular/core';

import { VendorChargeStatusesComponent } from './billing/vendorChargeStatuses/vendorChargeStatuses.component';
import { WorkOrderStatusesComponent } from './support/workOrderStatuses/workOrderStatuses.component';
import { WorkOrderActionsComponent } from './support/workOrderActions/workOrderActions.component';
import { CustomerTypesComponent } from './customers/customerTypes/customerTypes.component';
import { CustomerInvoiceStatusesComponent } from './billing/customerInvoiceStatuses/customerInvoiceStatuses.component';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { WorkOrderTypesComponent } from './support/workOrderTypes/workOrderTypes.component';
import { WorkOrderPrioritiesComponent } from './support/workOrderPriorities/workOrderPriorities.component';
import { RfqTypesComponent } from './quotations/rfqTypes/rfqTypes.component';
import { IncidentTypesComponent } from './support/incidentTypes/incidentTypes.component';
import { IncidentStatusesComponent } from './support/incidentStatuses/incidentStatuses.component';
import { IncidentPrioritiesComponent } from './support/incidentPriorities/incidentPriorities.component';
import { BillingRuleTypesComponent } from './billing/billingRuleTypes/billingRuleTypes.component';
import { SupportTypesComponent } from './support/supportTypes/supportTypes.component';
import { QuotationStatusesComponent } from './quotations/quotationStatuses/quotationStatuses.component';
import { ItemTypesComponent } from './quotations/itemTypes/itemTypes.component';
import { CurrenciesComponent } from './billing/currencies/currencies.component';
import { SsicCodesComponent } from './organizations/ssicCodes/ssicCodes.component';
import { UomsComponent } from './metrics/uoms/uoms.component';
import { AssetTypesComponent } from './assets/assetTypes/assetTypes.component';
import { BillingEventTypesComponent } from './billing/billingEventTypes/billingEventTypes.component';
import { AssetStatusesComponent } from './assets/assetStatuses/assetStatuses.component';
import { ConsumableTypesComponent } from './support/consumableTypes/consumableTypes.component';
import { AssetClassesComponent } from './assets/assetClasses/assetClasses.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'billing/vendorChargeStatuses', component: VendorChargeStatusesComponent, data: { permission: 'Pages.Configuration.VendorChargeStatuses' }  },
                    { path: 'support/workOrderStatuses', component: WorkOrderStatusesComponent, data: { permission: 'Pages.Configuration.WorkOrderStatuses' }  },
                    { path: 'support/workOrderActions', component: WorkOrderActionsComponent, data: { permission: 'Pages.Configuration.WorkOrderActions' }  },
                    { path: 'billing/customerInvoiceStatuses', component: CustomerInvoiceStatusesComponent, data: { permission: 'Pages.Configuration.CustomerInvoiceStatuses' }  },
                    { path: 'customers/customerTypes', component: CustomerTypesComponent, data: { permission: 'Pages.Configuration.CustomerTypes' }  },
                    { path: 'assets/assetClasses', component: AssetClassesComponent, data: { permission: 'Pages.Configuration.AssetClasses' } },
                    { path: 'assets/assetStatuses', component: AssetStatusesComponent, data: { permission: 'Pages.Configuration.AssetStatuses' } },
                    { path: 'assets/assetTypes', component: AssetTypesComponent, data: { permission: 'Pages.Configuration.AssetTypes' } },
                    { path: 'billing/billingEventTypes', component: BillingEventTypesComponent, data: { permission: 'Pages.Configuration.BillingEventTypes' } },
                    { path: 'billing/billingRuleTypes', component: BillingRuleTypesComponent, data: { permission: 'Pages.Configuration.BillingRuleTypes' } },
                    { path: 'billing/currencies', component: CurrenciesComponent, data: { permission: 'Pages.Configuration.Currencies' } },
                    { path: 'metrics/uoms', component: UomsComponent, data: { permission: 'Pages.Configuration.Uoms' } },
                    { path: 'organizations/ssicCodes', component: SsicCodesComponent, data: { permission: 'Pages.Configuration.SsicCodes' } },
                    { path: 'quotations/itemTypes', component: ItemTypesComponent, data: { permission: 'Pages.Configuration.ItemTypes' } },
                    { path: 'quotations/quotationStatuses', component: QuotationStatusesComponent, data: { permission: 'Pages.Configuration.QuotationStatuses' } },
                    { path: 'quotations/rfqTypes', component: RfqTypesComponent, data: { permission: 'Pages.Configuration.RfqTypes' } },
                    { path: 'support/consumableTypes', component: ConsumableTypesComponent, data: { permission: 'Pages.Configuration.ConsumableTypes' } },
                    { path: 'support/incidentPriorities', component: IncidentPrioritiesComponent, data: { permission: 'Pages.Configuration.IncidentPriorities' } },
                    { path: 'support/incidentStatuses', component: IncidentStatusesComponent, data: { permission: 'Pages.Configuration.IncidentStatuses' } },
                    { path: 'support/incidentTypes', component: IncidentTypesComponent, data: { permission: 'Pages.Configuration.IncidentTypes' } },
                    { path: 'support/supportTypes', component: SupportTypesComponent, data: { permission: 'Pages.Configuration.SupportTypes' } },
                    { path: 'support/workOrderPriorities', component: WorkOrderPrioritiesComponent, data: { permission: 'Pages.Configuration.WorkOrderPriorities' } },
                    { path: 'support/workOrderTypes', component: WorkOrderTypesComponent, data: { permission: 'Pages.Configuration.WorkOrderTypes' } }
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class ConfigRoutingModule {

    constructor(
        private router: Router
    ) {
        router.events.subscribe((event) => {
            if (event instanceof NavigationEnd) {
                window.scroll(0, 0);
            }
        });
    }
}
