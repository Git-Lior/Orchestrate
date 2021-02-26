import { useCallback } from "react";

type ResultTypes = "json" | "text" | "none";

export function useApiFetch(user?: { token: string }, baseUrl: string = "") {
  const apiFetch = useCallback(
    (url: string, init: RequestInit = {}, type: ResultTypes = "json") => {
      const resolvedUrl = url.startsWith("/") ? url : `${baseUrl}/${url}`;
      return fetch("/api" + resolvedUrl, {
        ...init,
        headers: {
          ...(!user?.token ? {} : { authorization: `Bearer ${user.token}` }),
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
