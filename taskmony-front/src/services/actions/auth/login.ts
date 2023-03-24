import { checkResponse, getErrorMessages } from "../../../utils/APIUtils";
import { BASE_URL } from "../../../utils/data";
import Cookies from "js-cookie";
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
      .then(async (res) => {
        let data = await res.json();      
        if (res.status != 200) throw new Error(getErrorMessages(data));
        else return data;
      })
      .then((res) => {
        if (res) {
          Cookies.set("accessToken", res.accessToken, {
            expires: 1 / 48,
          });
          Cookies.set("refreshToken", res.refreshToken, {
            expires: 30,
          });
          Cookies.set("id", res.userId, {
            expires: 30,
          });
          dispatch({
            type: LOGIN_SUCCESS,
          });
        } else {
          dispatch({
            type: LOGIN_FAILED,
            error: "something went wrong"
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: LOGIN_FAILED,
          error: error.message
        });
      });
  };
}
