import React from "react";

import { makeStyles } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import PlaceIcon from "@material-ui/icons/Place";

import { getDateText, getTimeText } from "utils/general";

const useStyles = makeStyles({
  root: {
    height: "100%",
    display: "flex",
    flexDirection: "column",
    justifyContent: "space-around",
    alignItems: "center",
  },
  concertTime: {},
  concertLocation: { display: "flex", width: "100%" },
});

interface Props {
  concert: orch.Concert;
}

export default function CardInfo({ concert }: Props) {
  const classes = useStyles();

  return (
    <div className={classes.root}>
      <div className={classes.concertTime}>
        <Typography variant="body1">{getDateText(concert.date)}</Typography>
        <Typography variant="body1">{getTimeText(concert.date)}</Typography>
      </div>
      <div className={classes.concertLocation}>
        <PlaceIcon />
        <Typography variant="h6">{concert.location}</Typography>
      </div>
    </div>
  );
}
