import React, { useCallback, useEffect, useState } from "react";

import { makeStyles } from "@material-ui/core/styles";
import Divider from "@material-ui/core/Divider";
import IconButton from "@material-ui/core/IconButton";
import EditIcon from "@material-ui/icons/Edit";
import DoneIcon from "@material-ui/icons/Done";
import DeleteIcon from "@material-ui/icons/Delete";
import CloseIcon from "@material-ui/icons/Close";
import TextField from "@material-ui/core/TextField";
import Button from "@material-ui/core/Button";
import Typography from "@material-ui/core/Typography";
import ListItem from "@material-ui/core/ListItem";
import ListItemText from "@material-ui/core/ListItemText";
import AvatarGroup from "@material-ui/lab/AvatarGroup";

import { AppTheme } from "AppTheme";
import { usePromiseStatus } from "utils/hooks";
import { ListInput, UserAvatar } from "utils/components";

import CardInfoInput from "./CardInfoInput";
import CardInfo from "./CardInfo";
import Paper from "@material-ui/core/Paper";

const useStyles = makeStyles<AppTheme, Props>(theme => ({
  cardContainer: {
    height: "20rem",
    display: "flex",
    padding: "1.5rem",
    backgroundColor: `${theme.palette.background.paper} !important`,
  },
  actions: { marginBottom: "3px" },
  cardInfo: { width: "25%" },
  divider: { width: "2px", margin: "1.5rem" },
  cardContent: { overflow: "hidden", flex: 1, display: "flex", flexDirection: "column" },
  concertDescription: { width: "100%", flex: 1 },
  moreInfo: { display: "flex", justifyContent: "space-between", alignItems: "center" },
  compositionsInput: { width: "100%" },
}));

interface Props {
  concert: orch.Concert;
  userInfo: orch.group.UserInfo;
  compositionProvider?: (title: string) => Promise<orch.CompositionData[]>;
  onEditRequested?: (concert: orch.Concert) => void;
  onDelete?: (concertId: number) => Promise<any>;
  onAttendanceChange?: (concert: orch.Concert, attending: boolean) => Promise<any>;
  onCompositionAdded?: (concert: orch.Concert, composition: orch.CompositionData) => Promise<any>;
  onCompositionRemoved?: (concert: orch.Concert, composition: orch.CompositionData) => Promise<any>;
}

export default function ConcertCard(props: Props) {
  const {
    userInfo,
    concert,
    compositionProvider,
    onEditRequested,
    onDelete,
    onAttendanceChange,
    onCompositionAdded,
    onCompositionRemoved,
  } = props;
  const classes = useStyles(props);

  const [loading, error, setPromise] = usePromiseStatus();

  const onAccept = useCallback(() => {
    if (onAttendanceChange && !concert?.attending) setPromise(onAttendanceChange(concert!, true));
  }, [concert, setPromise, onAttendanceChange]);
  const onReject = useCallback(() => {
    if (onAttendanceChange && concert?.attending !== false)
      setPromise(onAttendanceChange(concert!, false));
  }, [concert, setPromise, onAttendanceChange]);

  const deleteHandler = useCallback(() => onDelete && setPromise(onDelete(concert!.id)), [
    onDelete,
    concert?.id,
    setPromise,
  ]);
  const editHandler = useCallback(() => onEditRequested?.(concert!), [onEditRequested, concert]);

  const compositionAddedHandler = useCallback(
    (composition: orch.CompositionData) => onCompositionAdded?.(concert!, composition)!,
    [concert]
  );
  const compositionRemovedHandler = useCallback(
    (composition: orch.CompositionData) => onCompositionRemoved?.(concert!, composition)!,
    [concert]
  );

  return (
    <>
      {userInfo.manager && (
        <div className={classes.actions}>
          {concert?.id && (
            <IconButton size="small" onClick={deleteHandler}>
              <DeleteIcon fontSize="inherit" />
            </IconButton>
          )}
          <IconButton size="small" onClick={editHandler}>
            <EditIcon fontSize="inherit" />
          </IconButton>
        </div>
      )}
      <Paper className={classes.cardContainer}>
        <div className={classes.cardInfo}>
          <CardInfo concert={concert!} />
        </div>
        <Divider flexItem orientation="vertical" className={classes.divider} />
        {concert && (
          <div className={classes.cardContent}>
            <div className={classes.concertDescription}>
              <Typography variant="body1">{concert.description}</Typography>
            </div>
            <div className={classes.moreInfo}>
              <div>
                {userInfo.roles.length > 0 && (
                  <>
                    <Button variant="contained" size="small" onClick={onAccept}>
                      Accept
                    </Button>
                    <Button variant="contained" size="small" onClick={onReject}>
                      Reject
                    </Button>
                  </>
                )}
              </div>
              <div>
                {userInfo.manager && (
                  <>
                    <div>
                      <Typography variant="body1">Attending:</Typography>
                      <AvatarGroup max={4}>
                        {concert.attendingUsers.map(user => (
                          <UserAvatar key={user.id} user={user} />
                        ))}
                      </AvatarGroup>
                    </div>
                    <div>
                      <Typography variant="body1">Not attending:</Typography>
                      <AvatarGroup max={4}>
                        {concert.notAttendingUsers.map(user => (
                          <UserAvatar key={user.id} user={user} />
                        ))}
                      </AvatarGroup>
                    </div>
                  </>
                )}
              </div>
            </div>
          </div>
        )}
      </Paper>
    </>
  );
}

function getCompositionText({ title, composer, genre }: orch.CompositionData) {
  return `${title} by ${composer} (${genre})`;
}
