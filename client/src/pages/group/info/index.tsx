import React, { useCallback } from "react";

import { makeStyles } from "@material-ui/core/styles";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";

import { useApiFetch } from "utils/hooks";

import DirectorsPanel from "./DirectorsPanel";
import AddRoleDialogButton from "./AddRoleDialogButton";
import RolesPanel from "./RolesPanel";
import UpdatesPanel from "./UpdatesPanel";

import changeGroupImage from "assets/change-group.png";

const useStyles = makeStyles({
  title: {
    marginBottom: "3rem",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    "& > :not(:first-child)": { marginLeft: "1rem" },
  },
  changeGroupImage: {
    position: "absolute",
    top: "5px",
    left: "100px",
    opacity: 0.8,
  },
  container: {
    display: "flex",
    height: "100%",
    "& > *": { padding: "0 2rem" },
  },
  sidePanel: { flex: "0 0 300px", display: "flex", flexDirection: "column" },
  directorsPanel: {
    height: "50%",
    minHeight: "20rem",
  },
  rolesPanel: {
    position: "relative",
    flex: 1,
    display: "flex",
    flexDirection: "column",
    overflow: "hidden",
  },
  updatesPanel: {
    flex: 1,
  },
  rolesPanelContent: { flex: 1 },
});

type Props = Required<orch.PageProps>;

export default function GroupInfoPage(props: Props) {
  const { user, userInfo, group, groups } = props;

  const classes = useStyles();
  const apiFetch = useApiFetch(user);

  const getAllUsers = useCallback(() => apiFetch(`/groups/${group.id}/users`), [
    apiFetch,
    group.id,
  ]);

  return (
    <>
      <Typography variant="h4" color="primary" className={classes.title}>
        Group Info - {group.name}
      </Typography>
      {groups.length > 1 && <img className={classes.changeGroupImage} src={changeGroupImage} />}
      <div className={classes.container}>
        <div className={classes.sidePanel}>
          <Typography variant="h5" className={classes.title}>
            Directors
          </Typography>
          <div className={classes.directorsPanel}>
            <DirectorsPanel {...props} getAllUsers={getAllUsers} />
          </div>
        </div>
        <div className={classes.rolesPanel}>
          <div className={classes.title}>
            <Typography variant="h5">Roles</Typography>
            {userInfo.manager && <AddRoleDialogButton {...props} />}
          </div>
          <Paper className={classes.rolesPanelContent}>
            <RolesPanel {...props} getAllUsers={getAllUsers} />
          </Paper>
        </div>
        {userInfo.manager && (
          <div className={classes.sidePanel}>
            <Typography variant="h5" className={classes.title}>
              Updates
            </Typography>
            <Paper className={classes.updatesPanel}>
              <UpdatesPanel user={user} group={group} />
            </Paper>
          </div>
        )}
      </div>
    </>
  );
}
