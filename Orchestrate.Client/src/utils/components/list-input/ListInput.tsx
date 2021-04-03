import React, { useCallback } from "react";
import classnames from "classnames";

import { makeStyles } from "@material-ui/core/styles";
import Paper from "@material-ui/core/Paper";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import Divider from "@material-ui/core/Divider";
import ListItemSecondaryAction from "@material-ui/core/ListItemSecondaryAction";

import { useInputState, usePromiseStatus } from "utils/hooks";
import { AsyncAutocomplete, AsyncAutocompleteProps } from "../AsyncAutocomplete";

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
  autocomplete: {
    padding: "1rem",
  },
});

interface Props<T> extends AsyncAutocompleteProps<T> {
  items: T[] | undefined;
  readonly?: boolean;
  elevation?: number;
  children: (item: T) => React.ReactNode;
  onAdded: (item: T) => Promise<any>;
  onRemoved: (item: T) => Promise<any>;
}

export function ListInput<T extends { id: number }>({
  items,
  readonly,
  className,
  elevation,
  children,
  onAdded,
  onRemoved,
  optionsProvider,
  ...autocompleteProps
}: Props<T>) {
  const classes = useStyles();
  const [inputValue, setInputValue] = useInputState();
  const [loading, error, setPromise] = usePromiseStatus();

  const filteredOptionsProvider = useCallback(
    async text => {
      const options = await optionsProvider(text);
      return !items || !options ? options : options.filter(u => !items.some(_ => _.id === u.id));
    },
    [optionsProvider, items]
  );

  const onItemAdded = useCallback((item: T) => setPromise(onAdded(item)), [setPromise, onAdded]);

  const onItemRemoved = useCallback((item: T) => setPromise(onRemoved(item)), [
    setPromise,
    onRemoved,
  ]);

  return (
    <Paper className={classnames(classes.container, className)} elevation={elevation}>
      <List className={classes.list}>
        {items?.map((item, i) => (
          <React.Fragment key={item.id}>
            {i > 0 && <Divider variant="middle" component="li" />}
            <ListItem>
              {children(item)}
              {!readonly && (
                <ListItemSecondaryAction>
                  <IconButton edge="end" aria-label="remove" onClick={() => onItemRemoved(item)}>
                    <CloseIcon />
                  </IconButton>
                </ListItemSecondaryAction>
              )}
            </ListItem>
          </React.Fragment>
        ))}
      </List>
      {!readonly && (
        <AsyncAutocomplete
          {...autocompleteProps}
          className={classes.autocomplete}
          variant="outlined"
          optionsProvider={filteredOptionsProvider}
          value={null}
          inputValue={inputValue}
          onInputChange={setInputValue as any}
          error={error}
          onChange={(_, item) => {
            if (item) {
              setInputValue();
              onItemAdded(item);
            }
          }}
        />
      )}
    </Paper>
  );
}
