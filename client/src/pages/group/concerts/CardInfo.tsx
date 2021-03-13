import React, { useCallback } from "react";
import moment from "moment";

import { makeStyles } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import PlaceIcon from "@material-ui/icons/Place";
import Button from "@material-ui/core/Button";

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
  userInfo: orch.group.UserInfo;
  onAttendanceChange: (value: boolean) => void;
}

export default function CardInfo({ userInfo, concert, onAttendanceChange }: Props) {
  const classes = useStyles();

  const onAccept = useCallback(() => {
    if (!concert.attending) onAttendanceChange(true);
  }, [concert.attending, onAttendanceChange]);
  const onReject = useCallback(() => {
    if (concert.attending !== false) onAttendanceChange(false);
  }, [concert.attending, onAttendanceChange]);

  return (
    <div className={classes.root}>
      <div className={classes.concertTime}>
        <Typography variant="body1">{moment(concert.date).format("DD/MM/yyyy")}</Typography>
        <Typography variant="body1">{moment(concert.date).format("HH:mm")}</Typography>
      </div>
      <div className={classes.concertLocation}>
        <PlaceIcon />
        <Typography variant="body1">{concert.location}</Typography>
      </div>
      {userInfo.roles.length > 0 && (
        <div>
          <Button variant="contained" size="small" onClick={onAccept}>
            Accept
          </Button>
          <Button variant="contained" size="small" onClick={onReject}>
            Reject
          </Button>
        </div>
      )}
    </div>
  );
}
