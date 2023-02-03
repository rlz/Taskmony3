import { iteratorSymbol } from "immer/dist/internal";
import { isTemplateSpan } from "typescript";
import { TTask } from "../../utils/types";
import {
  ADD_DIRECTION_FAILED,
  ADD_DIRECTION_REQUEST,
  ADD_DIRECTION_SUCCESS,
  GET_DIRECTIONS_FAILED,
  GET_DIRECTIONS_REQUEST,
  GET_DIRECTIONS_SUCCESS,
} from "../actions/directionsAPI";

type TDirectionsState = {
  items: Array<TTask>;
  get_directions_loading: boolean;
  get_directions_error: boolean;
  add_direction_loading: boolean;
  add_direction_success: boolean;
  add_direction_error: boolean;
};

export const directionsInitialState = {
  items: [],
  get_directions_loading: true,
  get_directions_error: false,
  add_direction_loading: false,
  add_direction_success: false,
  add_direction_error: false,
};
export const directionsReducer = (
  state: TDirectionsState = directionsInitialState,
  action:
    | { type: typeof GET_DIRECTIONS_SUCCESS; items: Array<any> }
    | { type: typeof ADD_DIRECTION_SUCCESS; direction: any }
    | {
        type:
          | typeof GET_DIRECTIONS_REQUEST
          | typeof GET_DIRECTIONS_FAILED
          | typeof ADD_DIRECTION_REQUEST
          | typeof ADD_DIRECTION_FAILED
          | typeof ADD_DIRECTION_SUCCESS;
      }
) => {
  switch (action.type) {
    case GET_DIRECTIONS_REQUEST: {
      return {
        ...state,
        get_directions_loading: true,
      };
    }
    case GET_DIRECTIONS_SUCCESS: {
      return {
        ...state,
        get_directions_loading: false,
        get_directions_error: false,
        items: action.items,
      };
    }
    case GET_DIRECTIONS_FAILED: {
      return {
        ...state,
        items: [],
        get_directions_loading: false,
        error: true,
      };
    }
    // case CHANGE_DIRECTIONS: {
    //   return {
    //     ...state,
    //     items: state.items.map(item=>item.id==action.direction.id? action.direction : item),
    //     error: true,
    //   };
    // }
    case ADD_DIRECTION_REQUEST: {
      return {
        ...state,
        add_direction_loading: true,
      };
    }
    case ADD_DIRECTION_SUCCESS: {
      return {
        ...state,
        items: [...state.items,action.direction],
        add_direction_loading: false,
        add_direction_success: true,
      };
    }
    case ADD_DIRECTION_FAILED: {
      return {
        ...state,
        add_direction_loading: false,
        add_direction_error: true,
        error: true,
      };
    }
    default: {
      return state;
    }
  }
};
