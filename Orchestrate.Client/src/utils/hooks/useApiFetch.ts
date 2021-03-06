import { useCallback } from "react";

type ResultTypes = "json" | "text" | "blob";

type ApiFetchFunc = <T extends ResultTypes | "none" = "json">(
  url: string,
  init?: RequestInit,
  type?: T
) => T extends "none" ? Promise<Response> : T extends ResultTypes ? ReturnType<Response[T]> : never;

export function useApiFetch(user?: { token?: string }, baseUrl: string = "") {
  const apiFetch: ApiFetchFunc = useCallback(
    (url, init = {}, type = "json" as any) => {
      const resolvedUrl = url.startsWith("/") ? url : `${baseUrl}/${url}`;
      return fetch("/api" + resolvedUrl, {
        ...init,
        headers: {
          ...(!user?.token ? {} : { authorization: `Bearer ${user.token}` }),
          ...(!init.body || typeof init.body !== "string"
            ? {}
            : { "Content-Type": "application/json" }),
          ...(init.headers || {}),
        },
      })
        .then(async result => {
          if (result.ok) return result;

          const errText = await result.text();
          let errJson: orch.Error = {
            error:
              result.status >= 500 ? "An error occured in the server, try again later" : errText,
          };

          try {
            errJson = JSON.parse(errText);
          } catch {}

          throw errJson;
        })
        .then(result => (type === "none" ? result : result[type as ResultTypes]())) as any;
    },
    [user?.token, baseUrl]
  );

  return apiFetch;
}
