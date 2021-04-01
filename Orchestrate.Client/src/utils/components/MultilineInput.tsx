import React, { useCallback, useMemo } from "react";

import TextField, { TextFieldProps } from "@material-ui/core/TextField";

interface Props {
  label: string;
  value: string;
  variant?: TextFieldProps["variant"];
  onChange: (e: React.ChangeEvent<HTMLTextAreaElement>, isValid: boolean) => void;
  rows?: number;
  rowsMax?: number;
  maxLineBreaks?: number;
  maxLength?: number;
}

export function MultilineInput({
  value,
  onChange,
  label,
  rows,
  rowsMax,
  variant,
  maxLineBreaks,
  maxLength,
}: Props) {
  const exceedingLineBreaks = useMemo(
    () => (text: string) => !!maxLineBreaks && hasExceedingLineBreaks(text, maxLineBreaks),
    [maxLineBreaks]
  );
  const exceedingLength = useMemo(() => (text: string) => !!maxLength && text.length > maxLength, [
    maxLength,
  ]);

  const hasError = useMemo(
    () => (text: string) => exceedingLineBreaks(text) || exceedingLength(text),
    [exceedingLineBreaks, exceedingLength]
  );

  const onInputChange = useCallback(e => onChange(e, !hasError(e.target.value)), [
    onChange,
    hasError,
  ]);

  return (
    <TextField
      label={label}
      fullWidth
      multiline
      rows={rows}
      rowsMax={rowsMax}
      error={hasError(value)}
      helperText={
        exceedingLineBreaks(value)
          ? `you can use at most ${maxLineBreaks} line breaks`
          : maxLength
          ? `${value.length}/${maxLength}`
          : undefined
      }
      variant={variant}
      value={value}
      onChange={onInputChange}
    />
  );
}

function hasExceedingLineBreaks(text: string, maxLineBreaks: number) {
  return text.split("").filter(_ => _ === "\n").length > maxLineBreaks;
}
