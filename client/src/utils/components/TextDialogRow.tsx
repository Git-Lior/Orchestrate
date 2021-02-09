import React, { useCallback } from "react";

import TextField from "@material-ui/core/TextField";
import { DialogRow } from "./DialogRow";

interface Props<T, K> {
  label: string;
  value: T;
  fieldKey: K;
  onChange: (key: K, value: string) => void;
}

export function TextDialogRow<T, K extends keyof T>({
  label,
  fieldKey,
  value,
  onChange,
}: Props<T, K>) {
  const onValueChange = useCallback(
    (e: React.ChangeEvent<any>) => onChange(fieldKey, e.target.value),
    [onChange, value, fieldKey]
  );

  return (
    <DialogRow label={label}>
      <TextField value={value[fieldKey]} onChange={onValueChange} />
    </DialogRow>
  );
}
