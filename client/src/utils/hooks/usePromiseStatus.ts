import { useCallback, useState } from "react";

type PromiseStatus = readonly [
  loading: boolean,
  error: orch.Error | undefined,
  setPromise: (promise: Promise<any>) => Promise<void>,
  clearError: () => void
];

export function usePromiseStatus(): PromiseStatus {
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<orch.Error>();

  const clearError = useCallback(() => setError(undefined), [setError]);

  const setPromise = useCallback(async (promise: Promise<any>) => {
    setError(undefined);
    setLoading(true);

    try {
      await promise;
    } catch (err) {
      setError(err);
    } finally {
      setLoading(false);
    }
  }, []);

  return [loading, error, setPromise, clearError];
}
