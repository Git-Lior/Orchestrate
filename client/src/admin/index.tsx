import React, { useState } from "react";

import { makeStyles } from "@material-ui/core";

import AuthPanel from "./AuthPanel";
import UsersPanel from "./UsersPanel";
import GroupsPanel from "./GroupsPanel";

const useStyles = makeStyles({
  adminPanel: {},
  usersPanel: {},
  groupsPanel: {},
});

export default function AdminApp() {
  const classes = useStyles();
  const [adminToken, setAdminToken] = useState<string>();
  const [] = useState<orch.UserData[]>();

  if (!adminToken) return <AuthPanel onTokenRecieved={setAdminToken} />;

  return (
    <div className={classes.adminPanel}>
      <div className={classes.usersPanel}>
        <UsersPanel />
      </div>
      <div className={classes.groupsPanel}>
        <GroupsPanel />
      </div>
    </div>
  );
}
