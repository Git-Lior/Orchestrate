import React, { useCallback, useState } from "react";

export function useApiPromise(text: boolean = false) {
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>();

  const promiseAction = useCallback(async (promise: Promise<Response>) => {
    setError(undefined);
    setLoading(true);

    try {
      const result = await promise;
      if (!result.ok) throw await result.text();

      if (text) return await result.text();
      else return await result.json();
    } catch (err) {
      setError(err?.message ?? err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  return [loading, error, promiseAction] as const;
}
