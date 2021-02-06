import React, { useCallback } from "react";

export function useApiFetch(user: orch.User, baseUrl: string = "") {
  const apiFetch = useCallback(
    (url: string, init: RequestInit = {}) => {
      return fetch(`/api${baseUrl}${url}`, {
        ...init,
        headers: {
          ...(init.headers || {}),
          authorization: `Bearer ${user.token}`,
        },
      }).then(_ => _.json());
    },
    [user]
  );

  return apiFetch;
}
