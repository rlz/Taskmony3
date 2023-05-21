import { TUser } from "../../utils/types";
import {
  USER_INFO_REQUEST,
  USER_INFO_SUCCESS,
  USER_INFO_FAILED,
  USERS_SUCCESS,
  USERS_REQUEST,
  USERS_FAILED,
  USERS_RESET,
  CHANGE_USER_NAME_SUCCESS,
  CHANGE_USER_EMAIL_SUCCESS,
} from "../actions/userInfo";

type TUserInfo = {
  user_info_loading: boolean;
  user_info_error: boolean;
  user_info_success: boolean;
  change_user_info_loading: boolean;
  change_user_info_error: boolean;
  change_user_info_success: boolean;
  users_loading: boolean;
  users_success: boolean;
  users_error: boolean;
  user: userInfoType;
  users: Array<TUser>;
};

type userInfoType = { displayName: string; email: string } | null;

export const initialState: TUserInfo = {
  user_info_loading: false,
  user_info_error: false,
  user_info_success: false,
  change_user_info_loading: false,
  change_user_info_error: false,
  change_user_info_success: false,
  users_loading: true,
  users_success: false,
  users_error: false,
  user: null,
  users: [],
};
export const userInfoReducer = (
  state = initialState,
  action:
    | {
        type:
          | typeof USER_INFO_REQUEST
          | typeof USER_INFO_FAILED
          | typeof USERS_REQUEST
          | typeof USERS_FAILED
          | typeof USERS_RESET;
      }
    | {
        type: typeof USER_INFO_SUCCESS;
        userInfo: userInfoType;
      }
    | {
        type:
          | typeof CHANGE_USER_EMAIL_SUCCESS
          | typeof CHANGE_USER_NAME_SUCCESS;
        payload: string;
      }
    | {
        type: typeof USERS_SUCCESS;
        users: Array<TUser>;
      }
) => {
  switch (action.type) {
    case USER_INFO_REQUEST: {
      return {
        ...state,
        user_info_loading: true,
        user_info_success: false,
        user_info_error: false,
      };
    }
    case USER_INFO_SUCCESS: {
      return {
        ...state,
        user_info_loading: false,
        user_info_success: true,
        user: action.userInfo,
      };
    }
    case USER_INFO_FAILED: {
      return {
        ...state,
        user_info_loading: false,
        user_info_error: true,
      };
    }
    case CHANGE_USER_NAME_SUCCESS: {
      return {
        ...state,
        user: { ...state.user, displayName: action.payload },
      };
    }
    case CHANGE_USER_EMAIL_SUCCESS: {
      return {
        ...state,
        user: { ...state.user, email: action.payload },
      };
    }
    case USERS_REQUEST: {
      return {
        ...state,
        users: [],
      };
    }
    case USERS_SUCCESS: {
      return {
        ...state,
        users_loading: false,
        users_success: true,
        users: action.users,
      };
    }
    case USERS_FAILED: {
      return {
        ...state,
        users_loading: false,
        users_error: true,
      };
    }
    case USERS_RESET: {
      return {
        ...state,
        users: [],
        users_loading: true,
        users_success: false,
        users_error: false,
      };
    }
    default: {
      return state;
    }
  }
};
