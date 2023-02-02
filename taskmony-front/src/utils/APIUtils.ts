export async function checkResponse(response: Response) {
  if (!response.ok) return false;
  const result = await response.json();
  if (result && result.success) return result;
  else return false;
}
