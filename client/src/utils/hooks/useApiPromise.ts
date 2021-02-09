import { useCallback, useState } from "react";

export function usePromiseStatus() {
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>();

  const clearError = useCallback(() => setError(undefined), [setError]);

  const promiseAction = useCallback(async (promise: Promise<any>) => {
    setError(undefined);
    setLoading(true);

    try {
      return await promise;
    } catch (err) {
      setError(err?.message ?? err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  return [loading, error, promiseAction, clearError] as const;
}

export function useApiPromise(text: boolean = false) {
  const [loading, error, promiseAction, clearError] = usePromiseStatus();

  const extendedPromiseAction = useCallback(
    (promise: Promise<Response>) => {
      return promiseAction(
        promise.then(async _ => {
          if (!_.ok) throw await _.text();
          return await (text ? _.text() : _.json());
        })
      );
    },
    [text, promiseAction]
  );

  return [loading, error, extendedPromiseAction, clearError] as const;
}
