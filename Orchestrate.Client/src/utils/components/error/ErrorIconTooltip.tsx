import React from "react";

import Tooltip from "@material-ui/core/Tooltip";
import ErrorIcon from "@material-ui/icons/Error";

import { ErrorText } from "./ServerError";

interface Props {
  error: orch.Error;
  color?: "inherit" | "primary" | "secondary" | "action" | "disabled" | "error";
}

export function ErrorIconTootlip({ error, color }: Props) {
  return (
    <Tooltip placement="top" arrow title={<ErrorText error={error} />}>
      <ErrorIcon color={color} />
    </Tooltip>
  );
}
