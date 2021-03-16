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
import Accordion from "@material-ui/core/Accordion";
import AccordionSummary from "@material-ui/core/AccordionSummary";
import AccordionDetails from "@material-ui/core/AccordionDetails";
import ListItem from "@material-ui/core/ListItem";
import ListItemText from "@material-ui/core/ListItemText";
import AvatarGroup from "@material-ui/lab/AvatarGroup";

import { AppTheme } from "AppTheme";
import { usePromiseStatus } from "utils/hooks";
import { ListInput, UserAvatar } from "utils/components";

import CardInfoInput from "./CardInfoInput";
import CardInfo from "./CardInfo";

const useStyles = makeStyles<AppTheme, Props>(theme => ({
  root: { background: "none" },
  cardContainer: {
    height: "20rem",
    display: "flex",
    padding: "1.5rem",
    backgroundColor: `${theme.palette.background.paper} !important`,
    cursor: props => (!props.editMode ? "pointer" : "default !important"),
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
  editMode?: boolean;
  concert?: orch.Concert;
  userInfo: orch.group.UserInfo;
  compositionProvider?: (title: string) => Promise<orch.CompositionData[]>;
  onEditRequested?: (concert: orch.Concert) => void;
  onEditDone?: (concert: orch.OptionalId<orch.ConcertData>) => Promise<any>;
  onEditCancel?: () => void;
  onDelete?: (concertId: number) => Promise<any>;
  onAttendanceChange?: (concert: orch.Concert, attending: boolean) => Promise<any>;
  onCompositionAdded?: (concert: orch.Concert, composition: orch.CompositionData) => Promise<any>;
  onCompositionRemoved?: (concert: orch.Concert, composition: orch.CompositionData) => Promise<any>;
}

export default function ConcertCard(props: Props) {
  const {
    editMode,
    userInfo,
    concert,
    compositionProvider,
    onEditRequested,
    onEditCancel,
    onEditDone,
    onDelete,
    onAttendanceChange,
    onCompositionAdded,
    onCompositionRemoved,
  } = props;
  const classes = useStyles(props);

  const [updatedData, setUpdatedData] = useState<orch.OptionalId<orch.ConcertData>>(
    concert ?? ({} as any)
  );

  const [expanded, setExpanded] = useState(false);
  const [loading, error, setPromise] = usePromiseStatus();

  useEffect(() => {
    if (editMode) {
      setUpdatedData(concert ?? ({} as any));
      setExpanded(false);
    }
  }, [editMode]);

  const handleExpand = useCallback((_, expanded: boolean) => !editMode && setExpanded(expanded), [
    editMode,
  ]);

  const onDescriptionChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      setUpdatedData({ ...updatedData, description: e.target.value });
    },
    [updatedData]
  );

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
  const doneHandler = useCallback(() => {
    if (onEditDone && updatedData.date && updatedData.location)
      setPromise(onEditDone({ id: concert?.id, ...updatedData }));
  }, [concert, updatedData, setPromise, onEditDone]);

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
          {!editMode ? (
            <IconButton size="small" onClick={editHandler}>
              <EditIcon fontSize="inherit" />
            </IconButton>
          ) : (
            <>
              <IconButton size="small" onClick={doneHandler} disabled={!updatedData}>
                <DoneIcon fontSize="inherit" />
              </IconButton>
              <IconButton size="small" onClick={onEditCancel}>
                <CloseIcon fontSize="inherit" />
              </IconButton>
            </>
          )}
        </div>
      )}
      <Accordion expanded={expanded} onChange={handleExpand} className={classes.root}>
        <AccordionSummary className={classes.cardContainer}>
          <div className={classes.cardInfo}>
            {editMode ? (
              <CardInfoInput concert={concert} onDataUpdated={setUpdatedData} />
            ) : (
              <CardInfo concert={concert!} />
            )}
          </div>
          <Divider flexItem orientation="vertical" className={classes.divider} />
          {concert && (
            <div className={classes.cardContent}>
              <div className={classes.concertDescription}>
                {editMode ? (
                  <TextField
                    fullWidth
                    disabled={!userInfo.manager}
                    value={updatedData.description}
                    onChange={onDescriptionChange}
                  />
                ) : (
                  <Typography variant="body1">{concert.description}</Typography>
                )}
              </div>
              {!editMode && (
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
              )}
            </div>
          )}
        </AccordionSummary>
        <AccordionDetails>
          <ListInput
            className={classes.compositionsInput}
            disabled={!userInfo.manager}
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
        </AccordionDetails>
      </Accordion>
    </>
  );
}

function getCompositionText({ title, composer, genre }: orch.CompositionData) {
  return `${title} by ${composer} (${genre})`;
}
