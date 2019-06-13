import { Observable, of } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Action } from '@ngrx/store';

import { environment } from '../../environments/environment';
import { mockActionPlans } from '../mockData/mockActionPlans';
import {
    ActionPlanActionTypes, GetActionPlan, GetActionPlanError, GetActionPlansError, LoadActionPlans,
    PutActionPlan, PutActionPlanError, PutActionPlanSuccess, UpdateActionPlan
} from '../reducers/action-plan/action-plan.actions';
import { ActionPlan } from '../reducers/action-plan/action-plan.model';

@Injectable()
export class ActionPlanEffects {
    @Effect()
    getActionPlans$: Observable<Action> = this.actions$.pipe(
        ofType(ActionPlanActionTypes.GetActionPlans),
        mergeMap(action =>
            // environment.mockData
            // ? new Observable<Action>((sub: Subscriber<Action>) =>
            //     sub.next(new LoadActionPlans({ actionPlans: mockActionPlans }))
            //   )
            // :
            this.http.get(`${environment.baseUrl}${environment.apiUrl}actionplans`).pipe(
                map(
                    (actionPlans: ActionPlan[]) =>
                        actionPlans
                            ? new LoadActionPlans({ actionPlans })
                            : new GetActionPlansError()
                ),
                catchError(() => of(new GetActionPlansError()))
            )
        )
    )

    @Effect()
    getActionPlan$: Observable<Action> = this.actions$.pipe(
        ofType(ActionPlanActionTypes.GetActionPlan),
        mergeMap((action: GetActionPlan) =>
            // environment.mockData
            //   ? new Observable<Action>((sub: Subscriber<Action>) =>
            //       sub.next(
            //         new UpdateActionPlan({
            //           actionPlan: {
            //             id: mockActionPlans[0].actionPlanId,
            //             changes: mockActionPlans[0],
            //           },
            //         })
            //       )
            //     )
            //   :
            this.http
                .get(`${environment.baseUrl}${environment.apiUrl}actionplans/${action.payload.actionPlanId}`)
                .pipe(
                    map(
                        (actionPlan: ActionPlan) =>
                            actionPlan
                                ? new UpdateActionPlan({
                                    actionPlan: {
                                        id: actionPlan.actionPlanId,
                                        changes: actionPlan,
                                    },
                                })
                                : new GetActionPlanError()
                    ),
                    catchError(() => of(new GetActionPlansError()))
                )
        )
    )

    @Effect()
    updateActionPlan$: Observable<Action> = this.actions$.pipe(
        ofType(ActionPlanActionTypes.PutActionPlan),
        mergeMap((action: PutActionPlan) =>
            this.http.put(`${environment.baseUrl}${environment.apiUrl}actionplans`, action.payload.actionPlan)
                .pipe(
                    map((actionPlan: ActionPlan) =>
                        actionPlan ?
                            new PutActionPlanSuccess({ actionPlan }) :
                            new PutActionPlanError({ actionPlan: action.payload.actionPlan })),
                    catchError(() => of(new PutActionPlanError({ actionPlan: action.payload.actionPlan })))
                )
        )
    )

    @Effect()
    updateActionPlanSuccess$: Observable<Action> = this.actions$.pipe(
        ofType(ActionPlanActionTypes.PutActionPlanSuccess),
        map((action: PutActionPlanSuccess) => new UpdateActionPlan({
            actionPlan: {
                id: action.payload.actionPlan.actionPlanId,
                changes: action.payload.actionPlan,
            },
        }))
    )

    constructor (private actions$: Actions, private http: HttpClient) { }
}
