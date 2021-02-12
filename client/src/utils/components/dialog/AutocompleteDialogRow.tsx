import React from "react";

import Autocomplete from "@material-ui/lab/Autocomplete";

import { DialogRow, DialogRowProps } from "./DialogRow";
import { textAutocompleteOptions } from "../textAutocompleteOptions";

interface Props<TItem, TKey extends keyof TItem> extends DialogRowProps<TItem, TKey> {
  freeSolo?: TItem[TKey] extends string ? boolean : never;
  options: TItem[TKey][] | undefined;
  getOptionLabel: (option: TItem[TKey]) => string;
}

export function AutocompleteDialogRow<TItem, TKey extends keyof TItem>({
  freeSolo,
  options,
  getOptionLabel,
  ...rowProps
}: Props<TItem, TKey>) {
  return (
    <DialogRow {...rowProps}>
      {({ value, onChange }) => (
        <Autocomplete
          {...textAutocompleteOptions(options)}
          freeSolo={freeSolo}
          value={value}
          getOptionLabel={getOptionLabel}
          // as any - value can be null (when clear) or string (when freeSolo)
          onChange={(_, value) => onChange(value as any)}
        />
      )}
    </DialogRow>
  );
}
