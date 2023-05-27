import { Dispatch } from "redux";
import { getErrorMessages } from "../../../utils/api-utils";
import { BASE_URL } from "../../../utils/base-api-url";

export const REGISTER_REQUEST = "REGISTER_REQUEST";
export const REGISTER_SUCCESS = "REGISTER_SUCCESS";
export const REGISTER_FAILED = "REGISTER_FAILED";

const REGISTER_URL = BASE_URL + "/api/account/register";

export function register(
  email: string,
  password: string,
  displayName: string,
  login: string
) {
  return function (dispatch: Dispatch) {
    dispatch({ type: REGISTER_REQUEST });
    fetch(REGISTER_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email: email,
        password: password,
        displayName: displayName,
        login: login,
      }),
    })
      .then(async (res) => {
        if (res.status === 204) return res;
        else {
          let data = await res.json();
          throw new Error(getErrorMessages(data));
        }
      })
      .then((res) => {
        if (res) {
          // Cookies.set("accessToken", res.accessToken, {
          //   expires: 1/48,
          // });
          // Cookies.set("refreshToken", res.refreshToken, {
          //   expires: 30,
          // });
          dispatch({
            type: REGISTER_SUCCESS,
          });
        } else {
          dispatch({
            type: REGISTER_FAILED,
            error: "something went wrong",
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: REGISTER_FAILED,
          error: error.message,
        });
      });
  };
}
