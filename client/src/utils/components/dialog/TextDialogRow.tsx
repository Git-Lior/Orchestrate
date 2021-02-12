import React from "react";

import TextField from "@material-ui/core/TextField";
import { DialogRow, DialogRowProps } from "./DialogRow";

type StringKeys<T> = {
  [K in keyof T]: T[K] extends string ? K : never;
}[keyof T];

export function TextDialogRow<TItem, TKey extends StringKeys<TItem> = StringKeys<TItem>>(
  props: DialogRowProps<TItem, TKey>
) {
  return (
    <DialogRow {...props}>
      {({ value, onChange }) => (
        <TextField value={value} onChange={e => onChange(e.target.value as any)} />
      )}
    </DialogRow>
  );
}
