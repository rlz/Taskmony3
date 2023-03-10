import { getCookie } from "../../utils/cookies";
import {
  GET_NOTIFICATIONS_REQUEST,
  GET_NOTIFICATIONS_SUCCESS,
  GET_NOTIFICATIONS_FAILED,
  RESET_COUNT
} from "../actions/notifications";

export const initialState = {
  loading: false,
  error: false,
  success: false,
  notifications: [],
  readTime: null,
  newCount: 0,
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
      let lastNotif = getCookie("lastNotification");
      let lastOldIndex = -1;
      action.notifications.map((notif,index)=>{
        if(notif.id == lastNotif) lastOldIndex = index;
      })
      return {
        ...state,
        loading: false,
        success: true,
        notifications: action.notifications,
        newCount: lastOldIndex
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
