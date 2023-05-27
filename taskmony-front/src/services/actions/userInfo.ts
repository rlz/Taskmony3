import { checkResponse, getAccessToken } from "../../utils/api-utils";
import { BASE_URL } from "../../utils/base-api-url";
import { refreshToken } from "./auth/refreshToken";
import { Dispatch } from "redux";

const URL = BASE_URL + "/graphql";

export const USER_INFO_REQUEST = "USER_INFO_REQUEST";
export const USER_INFO_SUCCESS = "USER_INFO_SUCCESS";
export const USER_INFO_FAILED = "USER_INFO_FAILED";

export const USERS_REQUEST = "USERS_REQUEST";
export const USERS_SUCCESS = "USERS_SUCCESS";
export const USERS_FAILED = "USERS_FAILED";
export const USERS_RESET = "USERS_RESET";

export function getUserInfo() {
  return function (dispatch: any) {
    dispatch({ type: USER_INFO_REQUEST });
    getAccessToken()
      .then((cookie) =>
        fetch(URL, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + cookie,
          },

          body: JSON.stringify({
            query: `{users{
          displayName
          email
        }}`,
          }),
        })
      )
      .then((data) => {
        if (data.status === 401 || data.status === 403) {
          dispatch(refreshToken());
          return false;
        }
        return checkResponse(data);
      })
      .then((res) => {
        if (res) {
          dispatch({
            type: USER_INFO_SUCCESS,
            userInfo: res?.data?.users[0],
          });
        } else {
          dispatch({
            type: USER_INFO_FAILED,
          });
        }
      })
      .catch((error) => {
        //console.log(error);
        dispatch({
          type: USER_INFO_FAILED,
        });
      });
  };
}

export function getUser(login: string) {
  return function (dispatch: any) {
    dispatch({ type: USERS_REQUEST });
    //console.log("getting user");
    getAccessToken()
      .then((cookie) =>
        fetch(URL, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + cookie,
          },

          body: JSON.stringify({
            query: `{users(login:"${login}"){
          displayName
          id
        }}`,
          }),
        })
      )
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: USERS_SUCCESS,
            users: res?.data?.users,
          });
        } else {
          dispatch({
            type: USERS_FAILED,
          });
        }
      })
      .catch((error) => {
        //console.log(error);
        dispatch({
          type: USERS_FAILED,
        });
      });
  };
}

export const CHANGE_USER_NAME_REQUEST = "CHANGE_USER_NAME_REQUEST";
export const CHANGE_USER_NAME_SUCCESS = "CHANGE_USER_NAME_SUCCESS";
export const CHANGE_USER_NAME_FAILED = "CHANGE_USER_NAME_FAILED";
export const CHANGE_USER_EMAIL_REQUEST = "CHANGE_USER_EMAIL_REQUEST";
export const CHANGE_USER_EMAIL_SUCCESS = "CHANGE_USER_EMAIL_SUCCESS";
export const CHANGE_USER_EMAIL_FAILED = "CHANGE_USER_EMAIL_FAILED";
export const CHANGE_USER_PASSWORD_REQUEST = "CHANGE_USER_PASSWORD_REQUEST";
export const CHANGE_USER_PASSWORD_SUCCESS = "CHANGE_USER_PASSWORD_SUCCESS";
export const CHANGE_USER_PASSWORD_FAILED = "CHANGE_USER_PASSWORD_FAILED";

export function changeUserPassword(oldPassword: string, newPassword: string) {
  return function (dispatch: Dispatch) {
    dispatch({ type: CHANGE_USER_PASSWORD_REQUEST });
    getAccessToken()
      .then((cookie) =>
        fetch(URL, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + cookie,
          },
          body: JSON.stringify({
            query: `mutation {
          userSetPassword(oldPassword:"${oldPassword}",newPassword:"${newPassword}") 
        }`,
          }),
        })
      )
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_USER_PASSWORD_SUCCESS,
          });
        } else {
          dispatch({
            type: CHANGE_USER_PASSWORD_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_USER_PASSWORD_FAILED,
        });
      });
  };
}
export function changeUserName(name: string) {
  return function (dispatch: Dispatch) {
    dispatch({ type: CHANGE_USER_NAME_REQUEST });
    getAccessToken()
      .then((cookie) =>
        fetch(URL, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + cookie,
          },
          body: JSON.stringify({
            query: `mutation {
          userSetDisplayName(displayName:"${name}") 
        }`,
          }),
        })
      )
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_USER_NAME_SUCCESS,
            payload: name,
          });
        } else {
          dispatch({
            type: CHANGE_USER_NAME_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_USER_NAME_FAILED,
        });
      });
  };
}
export function changeUserEmail(email: string) {
  return function (dispatch: Dispatch) {
    dispatch({ type: CHANGE_USER_EMAIL_REQUEST });
    getAccessToken()
      .then((cookie) =>
        fetch(URL, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + cookie,
          },
          body: JSON.stringify({
            query: `mutation {
          userSetEmail(email:"${email}") 
        }`,
          }),
        })
      )
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_USER_EMAIL_SUCCESS,
            payload: email,
          });
        } else {
          dispatch({
            type: CHANGE_USER_EMAIL_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_USER_EMAIL_FAILED,
        });
      });
  };
}
