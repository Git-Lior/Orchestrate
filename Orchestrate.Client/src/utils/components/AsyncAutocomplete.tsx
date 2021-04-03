import React, { useEffect, useMemo, useState } from "react";
import { debounce } from "throttle-debounce";

import Autocomplete, { AutocompleteProps } from "@material-ui/lab/Autocomplete";

import { useInputState } from "utils/hooks";
import { textAutocompleteOptions } from "./textAutocompleteOptions";
import { TextFieldProps } from "@material-ui/core/TextField";

export interface AsyncAutocompleteProps<
  T,
  Multiple extends boolean | undefined = undefined,
  DisableClearable extends boolean | undefined = undefined,
  FreeSolo extends boolean | undefined = undefined
> extends Partial<AutocompleteProps<T, Multiple, DisableClearable, FreeSolo>> {
  optionsProvider: (text: string) => Promise<T[]>;
  variant?: TextFieldProps["variant"];
  error?: orch.Error;
}

export function AsyncAutocomplete<
  T,
  Multiple extends boolean | undefined = undefined,
  DisableClearable extends boolean | undefined = undefined,
  FreeSolo extends boolean | undefined = undefined
>({
  optionsProvider,
  inputValue,
  onInputChange,
  error,
  variant,
  placeholder,
  ...otherProps
}: AsyncAutocompleteProps<T, Multiple, DisableClearable, FreeSolo>) {
  const [open, setOpen] = useState(false);
  const [options, setOptions] = useState<T[]>();
  const [innerInputValue, setInnerInputValue] = useInputState();

  const textValue = useMemo(() => inputValue ?? innerInputValue, [inputValue, innerInputValue]);

  const debouncedOptionsProvider = useMemo(
    () =>
      debounce(250, async (text: string, optionsProvider: (text: string) => Promise<T[]>) =>
        setOptions(await optionsProvider(text))
      ),
    []
  );

  useEffect(() => {
    if (open) debouncedOptionsProvider(textValue, optionsProvider);
  }, [debouncedOptionsProvider, open, textValue, optionsProvider]);

  return (
    <Autocomplete<T, Multiple, DisableClearable, FreeSolo>
      {...textAutocompleteOptions(options, { size: "small", variant, placeholder })}
      {...otherProps}
      inputValue={textValue}
      onInputChange={(e, value, r) => {
        onInputChange?.(e, value, r);
        setInnerInputValue({ target: { value } });
      }}
      open={open}
      onOpen={() => setOpen(true)}
      onClose={() => setOpen(false)}
    />
  );
}
