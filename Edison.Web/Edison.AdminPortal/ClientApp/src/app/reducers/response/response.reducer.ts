import { createEntityAdapter, EntityAdapter, EntityState } from '@ngrx/entity';

import { ResponseActions, ResponseActionTypes } from './response.actions';
import { Response } from './response.model';

export interface State extends EntityState<Response> {
    // additional entities state properties
    activeResponse: Response;
    showSelectingLocation: boolean;
    showManageResponse: boolean;
}

export const adapter: EntityAdapter<Response> = createEntityAdapter<Response>({
    selectId: (response: Response) => response.responseId,
    sortComparer: (a, b) => a.name.localeCompare(b.name),
});

export const initialState: State = adapter.getInitialState({
    // additional entity state properties
    activeResponse: null,
    showSelectingLocation: false,
    showManageResponse: false,
});

export function reducer(
    state = initialState,
    action: ResponseActions
): State {
    switch (action.type) {
        case ResponseActionTypes.AddResponse: {
            return adapter.addOne(action.payload.response, state);
        }

        case ResponseActionTypes.SignalRNewResponse:
        case ResponseActionTypes.PostNewResponseSuccess:
        case ResponseActionTypes.UpsertResponse: {
            return adapter.upsertOne(action.payload.response, state);
        }

        case ResponseActionTypes.AddResponses: {
            return adapter.addMany(action.payload.responses, state);
        }

        case ResponseActionTypes.UpsertResponses: {
            return adapter.upsertMany(action.payload.responses, state);
        }

        case ResponseActionTypes.UpdateResponse:
        case ResponseActionTypes.SignalRUpdateResponse:
        case ResponseActionTypes.SignalRCloseResponse: {
            const newState = adapter.updateOne(action.payload.response, state);

            if (state.activeResponse && state.activeResponse.responseId === action.payload.response.id) {
                return {
                    ...newState,
                    activeResponse: selectAll(newState).find(response => response.responseId === action.payload.response.id),
                }
            }

            return newState;
        }

        case ResponseActionTypes.UpdateResponses: {
            return adapter.updateMany(action.payload.responses, state);
        }

        case ResponseActionTypes.DeleteResponse: {
            return adapter.removeOne(action.payload.id, state);
        }

        case ResponseActionTypes.DeleteResponses: {
            return adapter.removeMany(action.payload.ids, state);
        }

        case ResponseActionTypes.LoadResponses: {
            return adapter.addAll(action.payload.responses, state);
        }

        case ResponseActionTypes.ClearResponses: {
            return adapter.removeAll(state);
        }

        case ResponseActionTypes.SelectActiveResponse: {
            return {
                ...state,
                activeResponse: action.payload.response,
            };
        }

        case ResponseActionTypes.ShowSelectingLocation: {
            return {
                ...state,
                showSelectingLocation: action.payload.showSelectingLocation
            }
        }

        case ResponseActionTypes.ShowManageResponse: {
            return {
                ...state,
                showManageResponse: action.payload.showManageResponse
            }
        }

        default: {
            return state;
        }
    }
}

export const {
    selectIds,
    selectEntities,
    selectAll,
    selectTotal,
} = adapter.getSelectors();
