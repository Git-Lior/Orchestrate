import React from "react";

import Autocomplete from "@material-ui/lab/Autocomplete";

import { DialogRow, DialogRowProps } from "./DialogRow";
import { textAutocompleteOptions } from "../textAutocompleteOptions";
import { AsyncAutocomplete } from "../AsyncAutocomplete";

interface Props<TItem, TKey extends keyof TItem> extends DialogRowProps<TItem, TKey> {
  freeSolo?: TItem[TKey] extends string ? boolean : never;
  options?: TItem[TKey][] | undefined;
  optionsProvider?: (text: string) => Promise<TItem[TKey][]>;
  getOptionLabel?: (option: TItem[TKey]) => string;
}

export function AutocompleteDialogRow<TItem, TKey extends keyof TItem>({
  freeSolo,
  options,
  optionsProvider,
  getOptionLabel,
  ...rowProps
}: Props<TItem, TKey>) {
  const otherProps = (value: TItem[TKey]) => ({
    freeSolo,
    value,
    getOptionLabel,
  });

  return (
    <DialogRow {...rowProps}>
      {({ value, onChange }) =>
        optionsProvider ? (
          <AsyncAutocomplete
            optionsProvider={optionsProvider}
            {...otherProps(value)}
            onChange={(_, v) => onChange(v as any)}
          />
        ) : (
          <Autocomplete
            {...textAutocompleteOptions(options)}
            {...otherProps(value)}
            onChange={(_, v) => onChange(v as any)}
          />
        )
      }
    </DialogRow>
  );
}
