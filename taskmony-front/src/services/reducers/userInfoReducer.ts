import {
  USER_INFO_REQUEST,
  USER_INFO_SUCCESS,
  USER_INFO_FAILED,
  CHANGE_USER_INFO_REQUEST,
  CHANGE_USER_INFO_SUCCESS,
  CHANGE_USER_INFO_FAILED,
  USERS_SUCCESS,
  USERS_REQUEST,
  USERS_FAILED,
  USERS_RESET,
} from "../actions/userInfo";

type TUser = {
  user_info_loading: boolean;
  user_info_error: boolean;
  user_info_success: boolean;
  change_user_info_loading: boolean;
  change_user_info_error: boolean;
  change_user_info_success: boolean;
  user: any;
  users: Array<any>;
};

export const initialState = {
  user_info_loading: false,
  user_info_error: false,
  user_info_success: false,
  change_user_info_loading: false,
  change_user_info_error: false,
  change_user_info_success: false,
  user: null,
  users: [],
};
export const userInfoReducer = (
  state: TUser = initialState,
  action:
    | {
        type:
          | typeof USER_INFO_REQUEST
          | typeof USER_INFO_FAILED
          | typeof USERS_REQUEST
          | typeof USERS_FAILED
          | typeof USERS_RESET
          | typeof CHANGE_USER_INFO_REQUEST
          | typeof CHANGE_USER_INFO_FAILED;
      }
    | {
        type: typeof USER_INFO_SUCCESS | typeof CHANGE_USER_INFO_SUCCESS;
        userInfo: any;
      }
    | {
        type: typeof USERS_SUCCESS;
        users: any;
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
    case CHANGE_USER_INFO_REQUEST: {
      return {
        ...state,
        change_user_info_loading: true,
        change_user_info_error: false,
        change_user_info_success: false,
      };
    }
    case CHANGE_USER_INFO_SUCCESS: {
      return {
        ...state,
        change_user_info_loading: false,
        change_user_info_success: true,
        email: action.userInfo.email,
        name: action.userInfo.name,
      };
    }
    case CHANGE_USER_INFO_FAILED: {
      return {
        ...state,
        change_user_info_loading: false,
        change_user_info_error: true,
      };
    }
    case USERS_REQUEST: {
      return {
        ...state,
        users: [],
        users_loading: true,
        users_success: false,
        users_error: false,
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