import React from "react";

import { makeStyles } from "@material-ui/core/styles";
import Paper from "@material-ui/core/Paper";
import Divider from "@material-ui/core/Divider";

const useStyles = makeStyles({
  cardContainer: {
    position: "relative",
    height: "24rem",
    display: "flex",
    padding: "1.5rem",
    // backgroundColor: `${theme.palette.background.paper} !important`,
  },
  cardInfo: { width: "25%" },
  divider: { width: "2px", margin: "1.5rem" },
  cardContent: { overflow: "hidden", flex: 1, display: "flex", flexDirection: "column" },
  cardRight: { width: "10rem" },
});

interface Props {
  left: React.ReactNode;
  center: React.ReactNode;
  right?: React.ReactNode;
}

export default function CardLayout({
  left,
  center,
  right,
  children,
}: React.PropsWithChildren<Props>) {
  const classes = useStyles();

  return (
    <Paper className={classes.cardContainer}>
      <div className={classes.cardInfo}>{left}</div>
      <Divider flexItem orientation="vertical" className={classes.divider} />
      <div className={classes.cardContent}>{center}</div>
      {right && (
        <>
          <Divider flexItem orientation="vertical" className={classes.divider} />
          <div className={classes.cardRight}>{right}</div>
        </>
      )}
      {children}
    </Paper>
  );
}
