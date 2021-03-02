import { useCallback, useEffect, useMemo, useState } from "react";

import { useApiFetch } from "./useApiFetch";

type CRUDItem = { id: number };

type CRUDApi<TData, TPayload> = [
  items: TData[] | undefined,
  refresh: () => Promise<void>,
  change: <THasResult extends boolean>(
    item: orch.OptionalId<TPayload>,
    hasResult?: THasResult
  ) => Promise<THasResult extends true ? TData : void>,
  remove: (id: number) => Promise<void>,
  setQuery: (query?: any) => void
];

export function useCRUDApi<TData extends CRUDItem, TPayload = TData>(
  token: string,
  apiRoute: string,
  initialQuery?: any
): CRUDApi<TData, TPayload> {
  const [items, setItems] = useState<TData[]>();
  const [query, setQuery] = useState<any>(initialQuery);
  const apiFetch = useApiFetch({ token }, apiRoute);

  const queryStr = useMemo(
    () =>
      query &&
      Object.entries(query)
        .filter(v => !!v[1])
        .map(([k, v]) => `${k}=${v}`)
        .join("&"),
    [query]
  );

  const refresh = useCallback(async () => {
    const items: TData[] = await apiFetch(!queryStr ? "" : `?${queryStr}`);
    setItems(items);
  }, [apiFetch, queryStr]);

  useEffect(() => {
    refresh();
  }, [queryStr, refresh]);

  const add = useCallback(
    async (item: orch.OptionalId<TPayload>, hasResult?: boolean) => {
      const result = await apiFetch(
        "",
        { method: "POST", body: JSON.stringify(item) },
        hasResult ? "json" : "none"
      );
      refresh();
      return result;
    },
    [items, apiFetch, refresh]
  );

  const update = useCallback(
    async (item: TPayload & CRUDItem, hasResult?: boolean) => {
      const result = await apiFetch(
        item.id.toString(),
        {
          method: "PUT",
          body: JSON.stringify(item),
        },
        hasResult ? "json" : "none"
      );
      refresh();
      return result;
    },
    [items, apiFetch, refresh]
  );

  const change = useCallback(
    (item: orch.OptionalId<TPayload>, hasResult?: boolean) =>
      item.id ? update(item as any, hasResult) : add(item, hasResult),
    [add, update]
  );

  const remove = useCallback(
    async (id: number) => {
      await apiFetch(id.toString(), { method: "DELETE" }, "none");
      await refresh();
    },
    [items, apiFetch, refresh]
  );

  return [items, refresh, change, remove, setQuery];
}
