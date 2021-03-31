import { useEffect, useRef, useState } from "react";
import { usePromiseStatus } from "./usePromiseStatus";

type Result<T> = [T | undefined, boolean, orch.Error | undefined];

export function useAutoRefresh<T>(
  itemProvider: () => Promise<T>,
  timeout: number = 10000
): Result<T> {
  const [item, setItem] = useState<T>();
  const isMounted = useRef<boolean>(true);
  const itemProviderRef = useRef(itemProvider);
  const [timeoutRef, setTimeoutRef] = useState<any>();
  const [loading, error, setPromise] = usePromiseStatus();

  useEffect(() => {
    itemProviderRef.current().then(setItem);
    return () => {
      isMounted.current = false;
    };
  }, []);

  useEffect(() => {
    itemProviderRef.current = itemProvider;
  }, [itemProvider]);

  useEffect(() => {
    if (!isMounted.current || loading || timeoutRef) return;

    setTimeoutRef(
      setTimeout(() => {
        setPromise(
          itemProviderRef.current().then(item => {
            if (!isMounted.current) return;
            setItem(item);
            setTimeoutRef(undefined);
          })
        );
      }, timeout)
    );
  }, [loading, timeoutRef]);

  return [item, loading, error];
}
