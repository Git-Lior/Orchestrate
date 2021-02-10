import { useCallback } from "react";

type ResultTypes = "json" | "text" | "none";

export function useApiFetch(user?: { token: string }, baseUrl: string = "") {
  const apiFetch = useCallback(
    (url: string, init: RequestInit = {}, type: ResultTypes = "json") => {
      return fetch(`/api${baseUrl}${url}`, {
        ...init,
        headers: {
          ...(!user ? {} : { authorization: `Bearer ${user.token}` }),
          ...(!init.body ? {} : { "Content-Type": "application/json" }),
          ...(init.headers || {}),
        },
      })
        .then(async result => {
          if (!result.ok) throw await result.json();
          return result;
        })
        .then(result =>
          type === "json" ? result.json() : type === "text" ? result.text() : result
        );
    },
    [user?.token, baseUrl]
  );

  return apiFetch;
}
