import { combineLatest, Subscription } from 'rxjs';

import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { select, Store } from '@ngrx/store';

import { DeviceType } from '../../../../core/models/deviceType';
import { AppState } from '../../../../reducers';
import { AppPage, SetPageData } from '../../../../reducers/app/app.actions';
import { FocusDevices, TestDevice } from '../../../../reducers/device/device.actions';
import { devicesSelector } from '../../../../reducers/device/device.selectors';
import { Event } from '../../../../reducers/event/event.model';
import { eventsSelector } from '../../../../reducers/event/event.selectors';
import { Response } from '../../../../reducers/response/response.model';
import { responsesSelector } from '../../../../reducers/response/response.selectors';
import { ExpandedDevice } from '../../models/expanded-device.model';
import { FilterGroupModel } from '../../models/filter-group.model';

@Component({
    selector: 'app-devices',
    templateUrl: './devices.component.html',
    styleUrls: [ './devices.component.scss' ]
})
export class DevicesComponent implements OnInit, OnDestroy, AfterViewInit {
    @ViewChild('container') container: ElementRef;

    filters: FilterGroupModel[];
    filter: string = '';
    devices: ExpandedDevice[];
    combinedStream$: Subscription;
    deviceCount: number = 0;

    private showButtons: boolean = true;
    private showSoundSensors: boolean = true;
    private showSmartBulbs: boolean = true;
    private showOnline: boolean = true;
    private showOffline: boolean = true;

    constructor (private store: Store<AppState>) { }

    ngOnInit() {
        this._setupSubscriptions();
        this._setupFilters();

        this.store.dispatch(new SetPageData({ title: AppPage.Devices, showDownArrow: false, showReloadButton: true }))
    }

    ngAfterViewInit() {
        this.container.nativeElement.focus();
    }

    ngOnDestroy() {
        this.combinedStream$.unsubscribe();
    }

    focusDevice(device: ExpandedDevice) {
        this.store.dispatch(new FocusDevices({ devices: [ device ] }));
    }

    testDevice(device: ExpandedDevice) {
        this.store.dispatch(new TestDevice({ deviceId: device.deviceId }));
    }

    private _setupSubscriptions() {
        const devicesObs = this.store.pipe(select(devicesSelector));
        const responsesObs = this.store.pipe(select(responsesSelector));
        const eventsObs = this.store.pipe(select(eventsSelector));

        this.combinedStream$ = combineLatest(devicesObs, responsesObs, eventsObs)
            .subscribe(([ devices, responses, events ]) => {
                this.devices = devices;
                this.deviceCount = devices.length;
                this._expandDevices(responses, events);
                this._filterDevices();
            })
    }

    private _expandDevices(responses: Response[], events: Event[]) {
        this.devices = this.devices.map(device => {
            const result = { ...device };

            const recentEvent = events
                .filter(event => event.device.deviceId === device.deviceId)
                .sort((a, b) => new Date(b.startDate).getTime() - new Date(a.startDate).getTime())
                .slice(0, 1)[ 0 ];

            if (recentEvent) {
                result.event = recentEvent;
                const response = responses.find(
                    response =>
                        response.primaryEventClusterId === recentEvent.eventClusterId ||
                        response.eventClusterIds.some(eci => eci === recentEvent.eventClusterId)
                )

                if (response) {
                    result.response = response;
                }
            }

            result.fullLocationName = `${device.location1} - ${device.location2}, ${device.location3}`;


            return result;
        });
    }

    private _filterDevices() {
        let filterString: string = Object.values(DeviceType).reduce((arr, value) => arr += `${value} `, '');

        filterString += 'Online Offline ';

        if (!this.showButtons && filterString.indexOf(DeviceType.ButtonSensor) !== -1) {
            filterString = filterString.replace(`${DeviceType.ButtonSensor} `, '');
        } else {
            filterString += `${DeviceType.ButtonSensor} `;
        }

        if (!this.showSmartBulbs && filterString.indexOf(DeviceType.SmartBulb) !== -1) {
            filterString = filterString.replace(`${DeviceType.SmartBulb} `, '');
        } else {
            filterString += `${DeviceType.SmartBulb} `;
        }

        if (!this.showSoundSensors && filterString.indexOf(DeviceType.SoundSensor) !== -1) {
            filterString = filterString.replace(`${DeviceType.SoundSensor} `, '');
        } else {
            filterString += `${DeviceType.SoundSensor} `;
        }

        if (!this.showOnline && filterString.indexOf('Online') !== -1) {
            filterString = filterString.replace('Online ', '');
        } else {
            filterString += 'Online '
        }

        if (this.showOffline && filterString.indexOf('Offline') !== -1) {
            filterString = filterString.replace('Offline ', '');
        } else {
            filterString += 'Offline '
        }

        this.filter = filterString;
    }

    private _setupFilters() {
        const typeFilters: FilterGroupModel = {
            title: 'TYPE',
            filters: [
                {
                    title: 'Buttons',
                    onClick: (checked: boolean) => { this.showButtons = checked; this._filterDevices() },
                    iconClasses: 'app-icon medium-small grey button',
                    checked: true,
                },
                {
                    title: 'Sound Sensors',
                    onClick: (checked: boolean) => { this.showSoundSensors = checked; this._filterDevices() },
                    iconClasses: 'app-icon medium-small grey sound',
                    checked: true,
                },
                {
                    title: 'SmartBulbs',
                    onClick: (checked: boolean) => { this.showSmartBulbs = checked; this._filterDevices() },
                    iconClasses: 'app-icon medium-small grey light',
                    checked: true,
                }
            ],
        };

        const connFilters: FilterGroupModel = {
            title: 'CONNECTION',
            filters: [
                {
                    title: 'Online',
                    onClick: (checked: boolean) => { this.showOnline = checked; this._filterDevices() },
                    checked: true,
                    iconClasses: 'app-icon dot active'
                },
                {
                    title: 'Offline',
                    onClick: (checked: boolean) => { this.showOffline = checked; this._filterDevices() },
                    checked: true,
                    iconClasses: 'app-icon dot'
                }
            ]
        };

        this.filters = [ typeFilters, connFilters, ];
    }

}
