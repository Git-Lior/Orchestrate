import React from "react";

import { makeStyles } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import CircularProgress from "@material-ui/core/CircularProgress";

const useStyles = makeStyles({
  loadingContainer: {
    width: "100%",
    height: "100%",
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    justifyContent: "center",
    opacity: "0.6",
  },
  loadingText: {
    marginBottom: "10rem",
  },
});

interface Props {
  text: string;
}

export function LoadingMessage({ text }: Props) {
  const classes = useStyles();
  return (
    <div className={classes.loadingContainer}>
      <Typography variant="h2" className={classes.loadingText}>
        {text}
      </Typography>
      <CircularProgress size="7rem" />
    </div>
  );
}
