import { useCallback, useState } from "react";

export function usePromiseStatus() {
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<orch.Error>();

  const clearError = useCallback(() => setError(undefined), [setError]);

  const promiseAction = useCallback(async (promise: Promise<any>) => {
    setError(undefined);
    setLoading(true);

    try {
      return await promise;
    } catch (err) {
      setError(err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  return [loading, error, promiseAction, clearError] as const;
}
