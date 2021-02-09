import React from "react";

import { Typography } from "@material-ui/core";
import { makeStyles } from "@material-ui/core/styles";

const useStyles = makeStyles({
  formRow: { display: "flex", marginBottom: "1em" },
  formRowLabel: { marginRight: "10px", fontSize: "18px" },
});

type Props = React.PropsWithChildren<{ label: string }>;

export function DialogRow({ label, children }: Props) {
  const classes = useStyles();

  return (
    <div className={classes.formRow}>
      <Typography variant="body1" className={classes.formRowLabel}>
        {label}
      </Typography>
      {children}
    </div>
  );
}
