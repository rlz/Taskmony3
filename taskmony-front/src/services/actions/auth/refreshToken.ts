import { Dispatch } from "redux";
import { checkResponse } from "../../../utils/APIUtils";
import { getCookie, setCookie } from "../../../utils/cookies";
import { BASE_URL } from "../../../utils/data";

export const REFRESH_TOKEN_REQUEST = "REFRESH_TOKEN_REQUEST";
export const REFRESH_TOKEN_SUCCESS = "REFRESH_TOKEN_SUCCESS";
export const REFRESH_TOKEN_FAILED = "REFRESH_TOKEN_FAILED";

const URL = BASE_URL + "/auth/token";

export function refreshToken() {
  return function (dispatch: Dispatch) {
    dispatch({ type: REFRESH_TOKEN_REQUEST });
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
          setCookie("accessToken", res.accessToken);
          setCookie("refreshToken", res.refreshToken);
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
