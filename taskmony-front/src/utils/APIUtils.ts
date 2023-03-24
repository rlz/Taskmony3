import Cookies from "js-cookie";
import { store } from "..";
import { refreshToken } from "../services/actions/auth/refreshToken";
import { BASE_URL } from "./data";

const URL = BASE_URL + "/api/account/token/refresh";

const checkToken = async() => {
  const currentToken = Cookies.get("accessToken");
  console.log(currentToken);
  if(currentToken === "pending") {
     window.setTimeout(()=>{const currentToken = checkToken()}, 100); 
  } else {
    return currentToken;
  }
}

export const getAccessToken = () => new Promise(async (resolve,reject) => {
  const currentToken = await checkToken();
  console.log(currentToken);
  if(currentToken !== undefined) resolve(currentToken);
  Cookies.set("accessToken","pending");
  fetch(URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      refreshToken: Cookies.get("refreshToken")
    }),
  }).then((res)=>res.json()).then((res) => {
    if (res) {
      Cookies.set("accessToken", res.accessToken,{
        expires: 1/48,
      });
      Cookies.set("refreshToken", res.refreshToken,{
        expires: 30,
      });
      resolve(res.accessToken);
    } else {
      reject();
    }
  })
});

export async function checkResponse(response: Response) {
  if (response.status == 401) store.dispatch(refreshToken());
  const result = await response.json();
  if (result) return result;
  else return false;
}
export const getErrorMessages = (data : {errors : Array<{message: string}>}) => data.errors.map(e=>e.message).join(",")
export const nowDate = () => {
  const now = new Date().setSeconds(0);
  return new Date(now).toISOString();
};
