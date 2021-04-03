import React, { useCallback, useState } from "react";

import { fade, makeStyles } from "@material-ui/core/styles";
import EditIcon from "@material-ui/icons/Edit";
import DeleteIcon from "@material-ui/icons/Delete";
import Button from "@material-ui/core/Button";
import Typography from "@material-ui/core/Typography";
import ListItem from "@material-ui/core/ListItem";
import ListItemText from "@material-ui/core/ListItemText";
import AvatarGroup from "@material-ui/lab/AvatarGroup";
import Link from "@material-ui/core/Link";
import Dialog from "@material-ui/core/Dialog";
import IconButton from "@material-ui/core/IconButton";
import { red, green } from "@material-ui/core/colors";

import { AppTheme } from "AppTheme";
import { usePromiseStatus } from "utils/hooks";
import { ListInput, UserAvatar } from "utils/components";

import CardInfo from "./CardInfo";
import CardLayout from "./CardLayout";

function buttonCss(color: string, active: boolean = false) {
  return {
    borderColor: color,
    "&:hover": { backgroundColor: fade(color, 0.2) },
    ...(!active ? {} : { color }),
  };
}

const useStyles = makeStyles<AppTheme, Props>({
  compositionsInput: { width: "100%" },
  cardCenter: {
    padding: "2rem 0",
    height: "100%",
    display: "flex",
    flexDirection: "column",
    justifyContent: "space-between",
  },
  description: { whiteSpace: "pre-wrap" },
  italic: { fontStyle: "italic" },
  centerBottom: { display: "flex", justifyContent: "space-between", alignItems: "center" },
  compositions: { maxWidth: "15rem" },
  compositionsText: { cursor: "pointer" },
  attendance: { display: "flex", "& > :not(:last-child)": { marginRight: "2rem" } },
  avatarGroup: { "& > *": { marginLeft: "-1rem" } },
  actions: { height: "100%", display: "flex", flexDirection: "column", justifyContent: "center" },
  actionsContent: {
    flex: 1,
    display: "flex",
    flexDirection: "column",
    justifyContent: "space-evenly",
  },
  acceptButton: props => buttonCss(green[500], !!props.concert.attending),
  declineButton: props => buttonCss(red[500], !props.concert.attending),
  managerActions: { position: "absolute", right: "1rem", top: "1rem" },
  compositionsDialog: {},
  dialogContainer: { padding: "2rem" },
  dialogTitle: { marginBottom: "1rem" },
});

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
  const [compositionsDialogOpen, setCompositionsDialogOpen] = useState<boolean>(false);

  const onAccept = useCallback(() => {
    if (onAttendanceChange && !concert.attending) setPromise(onAttendanceChange(concert, true));
  }, [concert, setPromise, onAttendanceChange]);
  const onDecline = useCallback(() => {
    if (onAttendanceChange && concert.attending !== false)
      setPromise(onAttendanceChange(concert, false));
  }, [concert, setPromise, onAttendanceChange]);

  const deleteHandler = useCallback(() => onDelete && setPromise(onDelete(concert.id)), [
    onDelete,
    concert.id,
    setPromise,
  ]);
  const editHandler = useCallback(() => onEditRequested?.(concert), [onEditRequested, concert]);

  const compositionsClickHandler = useCallback(() => {
    setCompositionsDialogOpen(true);
  }, []);

  const closeCompositionsDialog = useCallback(() => setCompositionsDialogOpen(false), []);

  const compositionAddedHandler = useCallback(
    (composition: orch.CompositionData) => onCompositionAdded?.(concert, composition)!,
    [concert]
  );
  const compositionRemovedHandler = useCallback(
    (composition: orch.CompositionData) => onCompositionRemoved?.(concert, composition)!,
    [concert]
  );

  return (
    <>
      <CardLayout
        left={<CardInfo concert={concert} />}
        center={
          <div className={classes.cardCenter}>
            <div>
              {concert.description ? (
                <Typography variant="body1" className={classes.description}>
                  {concert.description}
                </Typography>
              ) : (
                <Typography variant="body1" className={classes.italic}>
                  No description available
                </Typography>
              )}
            </div>
            <div className={classes.centerBottom}>
              {(userInfo.manager || concert.compositions.length > 0) && (
                <div className={classes.compositions}>
                  <Link
                    variant="body1"
                    color="textSecondary"
                    underline="always"
                    onClick={compositionsClickHandler}
                    className={classes.compositionsText}
                  >
                    {concert.compositions.length > 0
                      ? `${concert.compositions.length} compositions - click here to view`
                      : "Click to add compositions"}
                  </Link>
                </div>
              )}
              {userInfo.manager && (
                <div className={classes.attendance}>
                  <div>
                    <Typography variant="body1">Accepted</Typography>
                    <AvatarGroup max={4} className={classes.avatarGroup}>
                      {concert.attendingUsers.map(user => (
                        <UserAvatar key={user.id} user={user} small />
                      ))}
                    </AvatarGroup>
                    {concert.attendingUsers.length === 0 && (
                      <Typography variant="body1" color="textSecondary" className={classes.italic}>
                        0 members
                      </Typography>
                    )}
                  </div>
                  <div>
                    <Typography variant="body1">Declined</Typography>
                    <AvatarGroup max={4} className={classes.avatarGroup}>
                      {concert.notAttendingUsers.map(user => (
                        <UserAvatar key={user.id} user={user} small />
                      ))}
                    </AvatarGroup>
                    {concert.notAttendingUsers.length === 0 && (
                      <Typography variant="body1" color="textSecondary" className={classes.italic}>
                        0 members
                      </Typography>
                    )}
                  </div>
                </div>
              )}
            </div>
          </div>
        }
        right={
          userInfo.roles.length > 0 && (
            <div className={classes.actions}>
              <div className={classes.actionsContent}>
                <Button
                  variant="outlined"
                  size="small"
                  onClick={onAccept}
                  className={classes.acceptButton}
                >
                  Accept
                </Button>
                <Button
                  variant="outlined"
                  size="small"
                  onClick={onDecline}
                  className={classes.declineButton}
                >
                  Decline
                </Button>
              </div>
            </div>
          )
        }
      >
        {userInfo.manager && (
          <div className={classes.managerActions}>
            <IconButton size="small" onClick={editHandler}>
              <EditIcon fontSize="small" />
            </IconButton>
            <IconButton size="small" onClick={deleteHandler}>
              <DeleteIcon fontSize="small" />
            </IconButton>
          </div>
        )}
      </CardLayout>
      <Dialog
        open={compositionsDialogOpen}
        onClose={closeCompositionsDialog}
        className={classes.compositionsDialog}
        color="secondary"
      >
        <div className={classes.dialogContainer}>
          <Typography variant="h5" className={classes.dialogTitle}>
            Concert compositions
          </Typography>
          <ListInput
            placeholder="Add composition..."
            className={classes.compositionsInput}
            readonly={!userInfo.manager}
            items={concert?.compositions}
            onAdded={compositionAddedHandler}
            onRemoved={compositionRemovedHandler}
            optionsProvider={compositionProvider!}
            getOptionLabel={getCompositionText}
          >
            {c => (
              <ListItem key={c.id}>
                <ListItemText primary={c.title} secondary={`by ${c.composer} (${c.genre})`} />
              </ListItem>
            )}
          </ListInput>
        </div>
      </Dialog>
    </>
  );
}

function getCompositionText({ title, composer, genre }: orch.CompositionData) {
  return `${title} by ${composer} (${genre})`;
}
