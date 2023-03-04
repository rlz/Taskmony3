import { store } from "..";
import { refreshToken } from "../services/actions/auth/refreshToken";

export async function checkResponse(response: Response) {
  if (response.status == 401) store.dispatch(refreshToken());
  const result = await response.json();
  if (result) return result;
  else return false;
}
export const nowDate = () => {
  const now = new Date().setSeconds(0);
  return new Date(now).toISOString();
};
