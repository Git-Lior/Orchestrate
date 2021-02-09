import { useCallback } from "react";

export function useApiFetch(user: { token: string }, baseUrl: string = "") {
  const apiFetch = useCallback(
    (url: string, init: RequestInit = {}, noJson?: boolean) => {
      return fetch(`/api${baseUrl}${url}`, {
        ...init,
        headers: {
          ...(init.headers || {}),
          authorization: `Bearer ${user.token}`,
        },
      })
        .then(async result => {
          if (!result.ok) throw await result.text();
          return result;
        })
        .then(result => (noJson ? result : result.json()));
    },
    [user.token, baseUrl]
  );

  return apiFetch;
}
