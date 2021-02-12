import React, { useCallback } from "react";

import Paper from "@material-ui/core/Paper";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";

import { usePromiseStatus } from "utils/hooks";

interface Props<TItem, TNewItem> {
  items: TItem[] | undefined;
  getListItem: (item: TItem) => React.ReactNode;
  onClick?: (item: TItem) => React.ReactNode;
  onAdded: (item: TNewItem) => Promise<any>;
  children: (props: { onAdded: (item: TNewItem) => Promise<any> }) => React.ReactNode;
}

export function ListInput<TItem extends { id: number }, TNewItem = orch.OptionalId<TItem>>({
  items,
  getListItem,
  onAdded,
  children,
}: Props<TItem, TNewItem>) {
  const [loading, error, setPromise] = usePromiseStatus();

  const onItemAdded = useCallback((item: TNewItem) => setPromise(onAdded(item)), [
    setPromise,
    onAdded,
  ]);

  return (
    <Paper>
      <List>
        {items?.map(item => (
          <ListItem key={item.id}>{getListItem(item)}</ListItem>
        ))}
      </List>
      {children({ onAdded: onItemAdded })}
    </Paper>
  );
}
