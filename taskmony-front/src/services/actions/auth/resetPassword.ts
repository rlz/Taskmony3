import { Dispatch } from "redux";
import { checkResponse, getErrorMessages } from "../../../utils/api-utils";
import { BASE_URL } from "../../../utils/base-api-url";

export const RESET_PASSWORD_REQUEST = "RESET_PASSWORD_REQUEST";
export const RESET_PASSWORD_SUCCESS = "RESET_PASSWORD_SUCCESS";
export const RESET_PASSWORD_FAILED = "RESET_PASSWORD_FAILED";

const RESET_PASSWORD_URL = BASE_URL + "/password-reset";

export function resetPassword(email: string) {
  return function (dispatch: Dispatch) {
    dispatch({ type: RESET_PASSWORD_REQUEST });
    fetch(RESET_PASSWORD_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email: email,
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
            type: RESET_PASSWORD_SUCCESS,
          });
        } else {
          dispatch({
            type: RESET_PASSWORD_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: RESET_PASSWORD_FAILED,
          error: error.message,
        });
      });
  };
}

export const CHANGE_PASSWORD_REQUEST = "CHANGE_PASSWORD_REQUEST";
export const CHANGE_PASSWORD_SUCCESS = "CHANGE_PASSWORD_SUCCESS";
export const CHANGE_PASSWORD_FAILED = "CHANGE_PASSWORD_FAILED";

const CHANGE_PASSWORD_URL = BASE_URL + "/password-reset/reset";

export function changePassword(token: string, password: string) {
  return function (dispatch: Dispatch) {
    dispatch({ type: CHANGE_PASSWORD_REQUEST });
    fetch(CHANGE_PASSWORD_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        password: password,
        token: token,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_PASSWORD_SUCCESS,
          });
        } else {
          dispatch({
            type: CHANGE_PASSWORD_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_PASSWORD_FAILED,
        });
      });
  };
}
