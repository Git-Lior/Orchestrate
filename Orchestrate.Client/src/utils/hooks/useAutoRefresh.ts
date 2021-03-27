import { useEffect, useRef, useState } from "react";
import { usePromiseStatus } from "./usePromiseStatus";

type Result<T> = [T | undefined, boolean, orch.Error | undefined];

export function useAutoRefresh<T>(
  itemProvider: () => Promise<T>,
  timeout: number = 10000
): Result<T> {
  const [refresher, setRefresher] = useState<{}>({});
  const [item, setItem] = useState<T>();
  const isMounted = useRef<boolean>(true);
  const [loading, error, setPromise] = usePromiseStatus();

  useEffect(() => {
    return () => {
      isMounted.current = false;
    };
  }, []);

  useEffect(() => {
    if (!isMounted.current) return;

    setPromise(itemProvider().then(item => isMounted.current && setItem(item))).then(() => {
      if (isMounted.current) setTimeout(() => setRefresher({}), timeout);
    });
  }, [refresher]);

  return [item, loading, error];
}
