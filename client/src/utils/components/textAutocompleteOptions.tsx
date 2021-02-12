import React from "react";

import TextField from "@material-ui/core/TextField";

export function textAutocompleteOptions<T>(options: T[] | undefined) {
  return {
    fullWidth: true,
    clearOnBlur: true,
    handleHomeEndKeys: true,
    loading: !options,
    loadingText: "loading items...",
    options: options ?? [],
    renderInput: (params: any) => <TextField {...params} />,
  };
}
