import { checkResponse } from "../../../utils/APIUtils";
import { BASE_URL } from "../../../utils/data";
import { deleteCookie, getCookie, setCookie } from "../../../utils/cookies";
import { Dispatch } from "redux";

export const LOGOUT_REQUEST = "LOGOUT_REQUEST";
export const LOGOUT_SUCCESS = "LOGOUT_SUCCESS";
export const LOGOUT_FAILED = "LOGOUT_FAILED";

const URL = BASE_URL + "/auth/logout";

export function logout() {
  return function (dispatch  : Dispatch) {
    dispatch({ type: LOGOUT_REQUEST });
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        token: getCookie("refreshToken"),
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: LOGOUT_SUCCESS,
          });
        } else {
          dispatch({
            type: LOGOUT_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: LOGOUT_FAILED,
        });
      });
  };
}
