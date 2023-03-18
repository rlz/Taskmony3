import { getCookie } from "../../utils/cookies";
import { TNotification } from "../../utils/types";
import {
  GET_NOTIFICATIONS_REQUEST,
  GET_NOTIFICATIONS_SUCCESS,
  GET_NOTIFICATIONS_FAILED,
  RESET_COUNT,
} from "../actions/notifications";

type TNotificationsState = {
  loading: boolean;
  error: boolean;
  success: boolean;
  notifications: Array<TNotification>;
  newCount: number;
};

export const initialState: TNotificationsState = {
  loading: false,
  error: false,
  success: false,
  notifications: [],
  newCount: 0,
};
export const notificationsReducer = (
  state = initialState,
  action:
    | {
        type:
          | typeof GET_NOTIFICATIONS_REQUEST
          | typeof GET_NOTIFICATIONS_FAILED
          | typeof RESET_COUNT;
      }
    | {
        type: typeof GET_NOTIFICATIONS_SUCCESS;
        notifications: Array<TNotification>;
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
      let lastNotif = getCookie("lastNotification");
      let lastOldIndex = 0;
      action.notifications.map((notif, index) => {
        if (notif.id == lastNotif) lastOldIndex = index;
      });
      return {
        ...state,
        loading: false,
        success: true,
        notifications: action.notifications,
        newCount: lastOldIndex,
      };
    }

    case GET_NOTIFICATIONS_FAILED: {
      return {
        ...state,
        loading: false,
        error: true,
      };
    }
    case RESET_COUNT: {
      return {
        ...state,
        newCount: 0,
      };
    }
    default: {
      return state;
    }
  }
};
