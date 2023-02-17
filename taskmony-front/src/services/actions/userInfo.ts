import { checkResponse } from "../../utils/APIUtils";
import { BASE_URL } from "../../utils/data";
import { getCookie } from "../../utils/cookies";
import { refreshToken } from "./auth/refreshToken";
import { AnyAction, Dispatch } from "redux";
import { userAllQuery } from "../../utils/queries";

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
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },

      body: JSON.stringify({
        query: `{users{
          displayName
          email
        }}`,
      }),
    })
      .then((data) => {
        if (data.status == 401 || data.status == 403) {
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
        console.log(error);
        dispatch({
          type: USER_INFO_FAILED,
        });
      });
  };
}

export function getUser(login) {
  return function (dispatch: any) {
    dispatch({ type: USERS_REQUEST });
    console.log("getting user");
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },

      body: JSON.stringify({
        query: `{users(login:"${login}"){
          displayName
          id
        }}`,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          console.log(res);
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
        console.log(error);
        dispatch({
          type: USERS_FAILED,
        });
      });
  };
}

export const CHANGE_USER_INFO_REQUEST = "CHANGE_USER_INFO_REQUEST";
export const CHANGE_USER_INFO_SUCCESS = "CHANGE_USER_INFO_SUCCESS";
export const CHANGE_USER_INFO_FAILED = "CHANGE_USER_INFO_FAILED";

export function changeUserInfo(name: string, email: string, password: string) {
  return function (dispatch: Dispatch) {
    dispatch({ type: CHANGE_USER_INFO_REQUEST });
    let body;
    if (password === "")
      body = {
        email: email,
        name: name,
      };
    else {
      body = {
        email: email,
        name: name,
        password: password,
      };
    }
    const token = getCookie("accessToken") ? getCookie("accessToken") : "";
    fetch(URL, {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json",
        Authorization: token,
      },
      body: JSON.stringify(body),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_USER_INFO_SUCCESS,
            userInfo: res.user,
          });
        } else {
          dispatch({
            type: CHANGE_USER_INFO_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_USER_INFO_FAILED,
        });
      });
  };
}
