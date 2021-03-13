import React, { useCallback, useState } from "react";

import { makeStyles } from "@material-ui/core/styles";
import Card from "@material-ui/core/Card";
import Divider from "@material-ui/core/Divider";
import IconButton from "@material-ui/core/IconButton";
import EditIcon from "@material-ui/icons/Edit";
import DoneIcon from "@material-ui/icons/Done";
import DeleteIcon from "@material-ui/icons/Delete";
import CloseIcon from "@material-ui/icons/Close";

import { usePromiseStatus } from "utils/hooks";

import CardInfoInput from "./CardInfoInput";
import CardInfo from "./CardInfo";

const useStyles = makeStyles({
  root: { height: "20rem", display: "flex", padding: "1.5rem" },
  actions: { marginBottom: "3px" },
  cardInfo: { flex: 1 },
  divider: { width: "2px", margin: "1.5rem" },
  cardContent: { flex: 3 },
});

interface Props {
  editMode?: boolean;
  concert?: orch.Concert;
  userInfo: orch.group.UserInfo;
  onEditRequested?: (concert: orch.Concert) => void;
  onEditDone?: (concert: orch.OptionalId<orch.ConcertData>) => Promise<any>;
  onEditCancel?: () => void;
  onDelete?: (concertId: number) => Promise<any>;
  onAttendanceChange?: (concert: orch.Concert, attending: boolean) => Promise<any>;
}

export default function ConcertCard({
  editMode,
  userInfo,
  concert,
  onEditRequested,
  onEditCancel,
  onEditDone,
  onDelete,
  onAttendanceChange,
}: Props) {
  const classes = useStyles();

  const [updatedData, setUpdatedData] = useState<orch.OptionalId<orch.ConcertData>>();

  const [loading, error, setPromise] = usePromiseStatus();

  const deleteHandler = useCallback(() => onDelete && setPromise(onDelete(concert!.id)), [
    onDelete,
    concert?.id,
    setPromise,
  ]);
  const editHandler = useCallback(() => onEditRequested?.(concert!), [onEditRequested, concert]);
  const doneHandler = useCallback(() => {
    if (onEditDone && updatedData) setPromise(onEditDone({ id: concert?.id, ...updatedData }));
  }, [concert, updatedData, setPromise, onEditDone]);

  const changeAttendance = useCallback(
    (value: boolean) => onAttendanceChange && setPromise(onAttendanceChange(concert!, value)),
    []
  );

  //        <AvatarGroup max={4}>
  //           {concert.attendingUsers.map(user => (
  //             <UserAvatar key={user.id} user={user} />
  //           ))}
  //         </AvatarGroup>
  //         <AvatarGroup max={4}>
  //           {concert.notAttendingUsers.map(user => (
  //             <UserAvatar key={user.id} user={user} />
  //           ))}
  //         </AvatarGroup>

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
      <Card className={classes.root}>
        <div className={classes.cardInfo}>
          {editMode ? (
            <CardInfoInput concert={concert} onDataUpdated={setUpdatedData} />
          ) : (
            <CardInfo
              userInfo={userInfo}
              concert={concert!}
              onAttendanceChange={changeAttendance}
            />
          )}
        </div>
        <Divider flexItem orientation="vertical" className={classes.divider} />
        <div className={classes.cardContent}></div>
      </Card>
    </>
  );
}
