export async function checkResponse(response: Response) {
  const result = await response.json();
  if (result) return result;
  else return false;
}
