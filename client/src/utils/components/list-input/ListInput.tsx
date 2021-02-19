import React, { useCallback } from "react";

import { makeStyles } from "@material-ui/core/styles";
import Paper from "@material-ui/core/Paper";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import ListItemSecondaryAction from "@material-ui/core/ListItemSecondaryAction";

import { usePromiseStatus } from "utils/hooks";

const useStyles = makeStyles({
  container: {
    height: "100%",
    display: "flex",
    flexDirection: "column",
  },
  list: {
    flex: 1,
    minHeight: 0,
    overflowY: "auto",
  },
});

interface Props<TItem, TNewItem> {
  items: TItem[] | undefined;
  disabled?: boolean;
  getListItem: (item: TItem) => React.ReactNode;
  onClick?: (item: TItem) => React.ReactNode;
  onAdded: (item: TNewItem) => Promise<any>;
  onRemoved: (item: TItem) => Promise<any>;
  children: (props: { onAdded: (item: TNewItem) => Promise<any> }) => React.ReactNode;
}

export function ListInput<TItem extends { id: number }, TNewItem = orch.OptionalId<TItem>>({
  items,
  disabled,
  getListItem,
  onAdded,
  onRemoved,
  children,
}: Props<TItem, TNewItem>) {
  const classes = useStyles();
  const [loading, error, setPromise] = usePromiseStatus();

  const onItemAdded = useCallback((item: TNewItem) => setPromise(onAdded(item)), [
    setPromise,
    onAdded,
  ]);

  const onItemRemoved = useCallback((item: TItem) => setPromise(onRemoved(item)), [
    setPromise,
    onRemoved,
  ]);

  return (
    <Paper className={classes.container}>
      <List className={classes.list}>
        {items?.map(item => (
          <ListItem key={item.id}>
            {getListItem(item)}
            {!disabled && (
              <ListItemSecondaryAction>
                <IconButton edge="end" aria-label="remove" onClick={() => onItemRemoved(item)}>
                  <CloseIcon />
                </IconButton>
              </ListItemSecondaryAction>
            )}
          </ListItem>
        ))}
      </List>
      {!disabled && children({ onAdded: onItemAdded })}
    </Paper>
  );
}
