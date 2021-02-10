import { useCallback, useState } from "react";

import { useApiFetch } from "./useApiFetch";

type CRUDItem = { id: number };

type CRUDApi<T> = [
  items: T[] | undefined,
  refresh: () => Promise<void>,
  add: (item: T) => Promise<void>,
  change: (item: T) => Promise<void>,
  remove: (item: T) => Promise<void>
];

export function useCRUDApi<T extends CRUDItem>(token: string, apiRoute: string): CRUDApi<T> {
  const [items, setItems] = useState<T[]>();
  const apiFetch = useApiFetch({ token }, apiRoute);

  const refresh = useCallback(async () => {
    const items: T[] = await apiFetch("");
    setItems(items);
  }, [apiFetch]);

  const add = useCallback(
    async (item: T) => {
      const result: T = await apiFetch("", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(item),
      });

      setItems([...items!, result]);
    },
    [items]
  );

  const remove = useCallback(
    async (item: T) => {
      await apiFetch(`/${item.id}`, { method: "DELETE" }, "none");
      setItems(items!.filter(_ => _.id !== item.id));
    },
    [items]
  );

  const change = useCallback(
    async (item: T) => {
      await apiFetch(
        `/${item.id}`,
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(item),
        },
        "none"
      );

      const itemIndex = items!.findIndex(_ => _.id === item.id);
      const newItems = [...items!];
      newItems[itemIndex] = item;

      setItems(newItems);
    },
    [items]
  );

  return [items, refresh, add, change, remove];
}
