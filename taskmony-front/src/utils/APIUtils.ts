export async function checkResponse(response: Response) {
  const result = await response.json();
  if (result) return result;
  else return false;
}
export const nowDate = () => {
  const now = (new Date()).setSeconds(0);
  return (new Date(now)).toISOString();
}
