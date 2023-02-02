import { Dispatch } from "redux";
import { checkResponse } from "../../../utils/APIUtils";
import { setCookie } from "../../../utils/cookies";
import { BASE_URL } from "../../../utils/data";

export const REGISTER_REQUEST = "REGISTER_REQUEST";
export const REGISTER_SUCCESS = "REGISTER_SUCCESS";
export const REGISTER_FAILED = "REGISTER_FAILED";

const REGISTER_URL = BASE_URL + "/api/account/register";

export function register(email : string, password : string, name : string) {
  return function (dispatch  : Dispatch) {
    dispatch({ type: REGISTER_REQUEST });
    fetch(REGISTER_URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        "email": email,
        "password": password,
        "displayName": name,
        "login": name
      }),
    })
      .then((res) => {
        console.log(res);
        if (res) {
          setCookie("accessToken", res.accessToken);
          setCookie("refreshToken", res.refreshToken);
          dispatch({
            type: REGISTER_SUCCESS,
          });
        } else {
          dispatch({
            type: REGISTER_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: REGISTER_FAILED,
        });
      });
  };
}
