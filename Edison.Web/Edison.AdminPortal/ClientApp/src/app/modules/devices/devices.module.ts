import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { SharedModule } from '../shared/shared.module';
import { DeviceFiltersComponent } from './components/device-filters/device-filters.component';
import { DevicesComponent } from './pages/devices/devices.component';
import { DeviceTableComponent } from './components/device-table/device-table.component';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        FormsModule
    ],
    declarations: [
        DevicesComponent,
        DeviceFiltersComponent,
        DeviceTableComponent
    ],
    exports: [
        DevicesComponent
    ]
})
export class DevicesModule { }
