import { Dispatch } from "redux";
import { checkResponse } from "../../../utils/APIUtils";
import Cookies from 'js-cookie';
import { BASE_URL } from "../../../utils/data";

export const REFRESH_TOKEN_REQUEST = "REFRESH_TOKEN_REQUEST";
export const REFRESH_TOKEN_SUCCESS = "REFRESH_TOKEN_SUCCESS";
export const REFRESH_TOKEN_FAILED = "REFRESH_TOKEN_FAILED";

const URL = BASE_URL + "/api/account/token/refresh";

export function refreshToken() {
  return function (dispatch: Dispatch) {
    dispatch({ type: REFRESH_TOKEN_REQUEST });
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        refreshToken: Cookies.get("refreshToken")
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          Cookies.set("accessToken", res.accessToken,{
            expires: 1/48,
          });
          Cookies.set("refreshToken", res.refreshToken,{
            expires: 30,
          });
          dispatch({
            type: REFRESH_TOKEN_SUCCESS,
          });
        } else {
          dispatch({
            type: REFRESH_TOKEN_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: REFRESH_TOKEN_FAILED,
        });
      });
  };
}
