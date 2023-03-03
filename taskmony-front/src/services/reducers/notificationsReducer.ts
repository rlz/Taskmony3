import {
  GET_NOTIFICATIONS_REQUEST,
  GET_NOTIFICATIONS_SUCCESS,
  GET_NOTIFICATIONS_FAILED,
  CHANGE_READ_TIME
} from "../actions/notifications";

export const initialState = {
  loading: false,
  error: false,
  success: false,
  notifications: [],
  readTime: null,
};
export const notificationsReducer = (
  state = initialState,
  action:
    | {
        type:
          | typeof GET_NOTIFICATIONS_REQUEST
          | typeof GET_NOTIFICATIONS_FAILED;
      }
    | {
        type: typeof GET_NOTIFICATIONS_SUCCESS;
        notifications: any;
      }
    | {
        type: typeof CHANGE_READ_TIME;
        readTime: any;
      }
) => {
  switch (action.type) {
    case GET_NOTIFICATIONS_REQUEST: {
      return {
        ...state,
        loading: true,
        success: false,
        error: false,
      };
    }
    case GET_NOTIFICATIONS_SUCCESS: {
      return {
        ...state,
        loading: false,
        success: true,
        notifications: action.notifications,
      };
    }
    case GET_NOTIFICATIONS_FAILED: {
      return {
        ...state,
        loading: false,
        error: true,
      };
    }
    case CHANGE_READ_TIME: {
        return {
          ...state,
          readTime: action.readTime,
        };
      }
    default: {
      return state;
    }
  }
};
