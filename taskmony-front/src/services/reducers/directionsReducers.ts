import { iteratorSymbol } from "immer/dist/internal";
import { isTemplateSpan } from "typescript";
import { TDirection, TTask } from "../../utils/types";
import {
  ADD_DIRECTION_FAILED,
  ADD_DIRECTION_REQUEST,
  ADD_DIRECTION_SUCCESS,
  ADD_USER_FAILED,
  ADD_USER_REQUEST,
  ADD_USER_SUCCESS,
  CHANGE_DIRECTION_DETAILS_FAILED,
  CHANGE_DIRECTION_DETAILS_REQUEST,
  CHANGE_DIRECTION_DETAILS_SUCCESS,
  GET_DIRECTIONS_FAILED,
  GET_DIRECTIONS_REQUEST,
  GET_DIRECTIONS_SUCCESS,
} from "../actions/directionsAPI";

type TDirectionsState = {
  items: Array<TDirection>;
  get_directions_loading: boolean;
  get_directions_error: boolean;
  add_direction_loading: boolean;
  add_direction_success: boolean;
  add_direction_error: boolean;
  change_details_loading: boolean;
  change_details_success: boolean;
  change_details_error: boolean;
};

export const directionsInitialState = {
  items: [],
  get_directions_loading: true,
  get_directions_error: false,
  add_direction_loading: false,
  add_direction_success: false,
  add_direction_error: false,
  change_details_loading: false,
  change_details_success: false,
  change_details_error: false,
};
export const directionsReducer = (
  state: TDirectionsState = directionsInitialState,
  action:
    | { type: typeof GET_DIRECTIONS_SUCCESS; items: Array<any> }
    | { type: typeof ADD_DIRECTION_SUCCESS; direction: any }
    | { type: typeof ADD_USER_SUCCESS; directionId: any, user: {displayName: string, id: string} }
    | { type: typeof CHANGE_DIRECTION_DETAILS_SUCCESS; directionId: string, details: string }

    | {
        type:
          | typeof GET_DIRECTIONS_REQUEST
          | typeof GET_DIRECTIONS_FAILED
          | typeof ADD_DIRECTION_REQUEST
          | typeof ADD_DIRECTION_FAILED
          | typeof ADD_USER_REQUEST
          | typeof ADD_USER_FAILED
          | typeof CHANGE_DIRECTION_DETAILS_REQUEST
          | typeof CHANGE_DIRECTION_DETAILS_FAILED
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
    case ADD_USER_REQUEST: {
      return {
        ...state,
        add_user_loading: true,
      };
    }
    case ADD_USER_SUCCESS: {
      return {
        ...state,
        items: state.items.map(item=>item.id==action.directionId? {...item,members:[...item.members,action.user]} : item),
        add_user_loading: false,
        add_user_success: true,
      };
    }
    case ADD_USER_FAILED: {
      return {
        ...state,
        add_user_loading: false,
        add_user_error: true,
      };
    }
      case CHANGE_DIRECTION_DETAILS_REQUEST: {
        return {
          ...state,
          change_details_loading: true,
        };
      }
      case CHANGE_DIRECTION_DETAILS_SUCCESS: {
        return {
          ...state,
          items: state.items.map(item=>item.id==action.directionId? {...item,details:action.details} : item),
          change_details_loading: false,
          change_details_success: true,
        };
      }
      case CHANGE_DIRECTION_DETAILS_FAILED: {
        return {
          ...state,
          change_details_loading: false,
          change_details_error: true,
          error: true,
        };
    }
    default: {
      return state;
    }
  }
};
