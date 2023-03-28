import { Dispatch } from "redux";
import { checkResponse, getErrorMessages } from "../../../utils/APIUtils";
import Cookies from 'js-cookie';
import { BASE_URL } from "../../../utils/data";

export const REGISTER_REQUEST = "REGISTER_REQUEST";
export const REGISTER_SUCCESS = "REGISTER_SUCCESS";
export const REGISTER_FAILED = "REGISTER_FAILED";

const REGISTER_URL = BASE_URL + "/api/account/register";

export function register(email: string, password: string, displayName: string, login: string) {
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
      let data = await res.json();      
      if (res.status != 200) throw new Error(getErrorMessages(data));
      else return data;
    })
      .then((res) => {
        if (res) {
          Cookies.set("accessToken", res.accessToken, {
            expires: 1/48,
          });
          Cookies.set("refreshToken", res.refreshToken, {
            expires: 30,
          });
          dispatch({
            type: REGISTER_SUCCESS,
          });
        } else {
          dispatch({
            type: REGISTER_FAILED,
            error: "something went wrong"
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: REGISTER_FAILED,
          error: error.message
        });
      });
  };
}
