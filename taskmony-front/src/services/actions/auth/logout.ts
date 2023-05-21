import { getErrorMessages } from "../../../utils/api-utils";
import { BASE_URL } from "../../../utils/base-api-url";
import Cookies from 'js-cookie';
import { Dispatch } from "redux";

export const LOGOUT_REQUEST = "LOGOUT_REQUEST";
export const LOGOUT_SUCCESS = "LOGOUT_SUCCESS";
export const LOGOUT_FAILED = "LOGOUT_FAILED";

const URL = BASE_URL + "/auth/logout";

export function logout() {
  return function (dispatch: Dispatch) {
    dispatch({ type: LOGOUT_REQUEST });
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        token: Cookies.get("refreshToken"),
      }),
    })
    .then(async (res) => {
      let data = await res.json();      
      if (res.status !== 200) throw new Error(getErrorMessages(data));
      else return data;
    })
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
          error: error.message
        });
      });
  };
}
