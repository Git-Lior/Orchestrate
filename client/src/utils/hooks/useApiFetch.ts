import { useCallback } from "react";

export function useApiFetch(user: { token: string }, baseUrl: string = "") {
  const apiFetch = useCallback(
    (url: string, init: RequestInit = {}) => {
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
        .then(_ => _.json());
    },
    [user.token, baseUrl]
  );

  return apiFetch;
}
