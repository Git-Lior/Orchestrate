import { useCallback, useState } from "react";

import { useApiFetch } from "./useApiFetch";

type CRUDItem = { id: number };

type CRUDApi<TData, TPayload> = [
  items: TData[] | undefined,
  refresh: () => Promise<void>,
  change: (item: orch.OptionalId<TPayload>) => Promise<TData>,
  remove: (id: number) => Promise<void>
];

export function useCRUDApi<TData extends CRUDItem, TPayload = TData>(
  token: string,
  apiRoute: string
): CRUDApi<TData, TPayload> {
  const [items, setItems] = useState<TData[]>();
  const apiFetch = useApiFetch({ token }, apiRoute);

  const refresh = useCallback(async () => {
    const items: TData[] = await apiFetch("");
    setItems(items);
  }, [apiFetch]);

  const add = useCallback(
    async (item: orch.OptionalId<TPayload>) => {
      const result: TData = await apiFetch("", {
        method: "POST",
        body: JSON.stringify(item),
      });

      setItems([...items!, result]);
      return result;
    },
    [items, apiFetch]
  );

  const update = useCallback(
    async (item: TPayload & CRUDItem) => {
      const resultItem: TData = await apiFetch(item.id.toString(), {
        method: "PUT",
        body: JSON.stringify(item),
      });

      const itemIndex = items!.findIndex(_ => _.id === item.id);
      const newItems = [...items!];
      newItems[itemIndex] = resultItem;

      setItems(newItems);
      return resultItem;
    },
    [items, apiFetch]
  );

  const change = useCallback(
    (item: orch.OptionalId<TPayload>) => (item.id ? update(item as any) : add(item)),
    [add, update]
  );

  const remove = useCallback(
    async (id: number) => {
      await apiFetch(id.toString(), { method: "DELETE" }, "none");
      setItems(items!.filter(_ => _.id !== id));
    },
    [items, apiFetch]
  );

  return [items, refresh, change, remove];
}
