import { checkResponse } from "../../../utils/APIUtils";
import { BASE_URL } from "../../../utils/data";
import { setCookie } from "../../../utils/cookies";
import { Dispatch } from "redux";

export const LOGIN_REQUEST = "LOGIN_REQUEST";
export const LOGIN_SUCCESS = "LOGIN_SUCCESS";
export const LOGIN_FAILED = "LOGIN_FAILED";

const URL = BASE_URL + "/api/account/login";

export function login(login: string, password: string) {
  return function (dispatch: Dispatch) {
    dispatch({ type: LOGIN_REQUEST });
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        login: login,
        password: password,
      }),
    })
      .then((res) => res.json())
      .then((res) => {
        console.log(res);
        if (res) {
          setCookie("accessToken", res.accessToken,{
            expires: 30 * 60,
          });
          setCookie("refreshToken", res.refreshToken,{
            expires: 30 * 24 * 60 * 60,
          });
          setCookie("id", res.userId);
          dispatch({
            type: LOGIN_SUCCESS,
          });
        } else {
          dispatch({
            type: LOGIN_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: LOGIN_FAILED,
        });
      });
  };
}
