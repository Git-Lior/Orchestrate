import { useCallback, useEffect, useRef, useState } from "react";

type PromiseStatus = readonly [
  loading: boolean,
  error: orch.Error | undefined,
  setPromise: (promise: Promise<any>) => Promise<void>,
  clearError: () => void
];

export function usePromiseStatus(): PromiseStatus {
  const isMounted = useRef<boolean>(true);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<orch.Error>();

  useEffect(() => {
    return () => {
      isMounted.current = false;
    };
  }, []);

  const clearError = useCallback(() => setError(undefined), [setError]);

  const setPromise = useCallback(
    async (promise: Promise<any>) => {
      setError(undefined);
      setLoading(true);

      try {
        await promise;
      } catch (err) {
        if (isMounted.current) setError(err);
      } finally {
        if (isMounted.current) setLoading(false);
      }
    },
    [isMounted]
  );

  return [loading, error, setPromise, clearError];
}
