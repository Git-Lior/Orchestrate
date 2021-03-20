import React from "react";
import moment from "moment";

import { makeStyles } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import PlaceIcon from "@material-ui/icons/Place";

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
        <Typography variant="body1">{moment.unix(concert.date).format("DD/MM/yyyy")}</Typography>
        <Typography variant="body1">{moment.unix(concert.date).format("HH:mm")}</Typography>
      </div>
      <div className={classes.concertLocation}>
        <PlaceIcon />
        <Typography variant="h6">{concert.location}</Typography>
      </div>
    </div>
  );
}
