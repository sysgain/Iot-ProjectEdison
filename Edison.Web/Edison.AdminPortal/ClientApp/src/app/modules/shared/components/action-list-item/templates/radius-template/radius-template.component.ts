import { Subject } from 'rxjs';

import { Component, EventEmitter, Input, OnChanges, OnInit } from '@angular/core';

import {
    ActionChangeType, ActionPlanColor, ActionPlanRadiusAction, ActionStatus, AddEditAction
} from '../../../../../../reducers/action-plan/action-plan.model';

@Component({
    selector: 'app-radius-template',
    templateUrl: './radius-template.component.html',
    styleUrls: [ './radius-template.component.scss' ]
})
export class RadiusTemplateComponent implements OnInit, OnChanges {
    @Input() context: ActionPlanRadiusAction;
    @Input() last: boolean;
    @Input() canEdit: boolean;
    @Input() actionChangeSubject: Subject<{ addEditAction: AddEditAction, actionChangeType: ActionChangeType }>;
    @Input() isCloseAction: boolean;

    activeColor: string;
    actionPlanColors = Object.keys(ActionPlanColor).map(key => (ActionPlanColor[ key ]).toLowerCase());
    actionStatus = ActionStatus;

    ngOnInit() { this.updateLocalColor(); }

    ngOnChanges() { this.updateLocalColor(); }

    updateLocalColor() { this.activeColor = this.context.parameters.color; }

    getBgColor() { return this.activeColor.toLowerCase(); }

    updateColor(color: string) {
        if (this.activeColor === color) {
            this.activeColor = 'off';
        } else {
            this.activeColor = color;
        }

        const addEditAction: AddEditAction = {
            actionChangedString: ActionChangeType.Edit,
            isCloseAction: this.isCloseAction,
            action: {
                ...this.context,
                parameters: {
                    ...this.context.parameters,
                    color: this.activeColor,
                }
            }
        };

        this.actionChangeSubject.next({ addEditAction, actionChangeType: ActionChangeType.Edit });
    }
}
