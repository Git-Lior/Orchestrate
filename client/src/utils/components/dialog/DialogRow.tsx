import React, { useCallback } from "react";

import { Typography } from "@material-ui/core";
import { makeStyles } from "@material-ui/core/styles";

const useStyles = makeStyles({
  formRow: { display: "flex", marginBottom: "1em" },
  formRowLabel: { marginRight: "10px", fontSize: "18px" },
});

export interface DialogRowProps<TItem, TKey extends keyof TItem = keyof TItem> {
  label: string;
  item: TItem;
  fieldKey: TKey;
  onFieldChange: (key: TKey, value: TItem[TKey]) => void;
}

interface DialogRowWithChildrenProps<TItem, TKey extends keyof TItem>
  extends DialogRowProps<TItem, TKey> {
  children: (props: {
    value: TItem[TKey];
    onChange: (value: TItem[TKey]) => void;
  }) => React.ReactNode;
}

export function DialogRow<TItem, TKey extends keyof TItem>({
  label,
  fieldKey,
  item,
  onFieldChange,
  children,
}: DialogRowWithChildrenProps<TItem, TKey>) {
  const classes = useStyles();

  const onValueChange = useCallback((value: TItem[TKey]) => onFieldChange(fieldKey, value), [
    fieldKey,
    onFieldChange,
  ]);

  return (
    <div className={classes.formRow}>
      <Typography variant="body1" className={classes.formRowLabel}>
        {label}
      </Typography>
      {children({ value: item[fieldKey], onChange: onValueChange })}
    </div>
  );
}
