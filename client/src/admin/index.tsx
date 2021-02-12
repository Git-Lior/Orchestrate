import React, { useEffect, useState } from "react";

import { makeStyles } from "@material-ui/core";

import { useCRUDApi } from "utils/hooks";

import AuthPanel from "./AuthPanel";
import UsersTable from "./UsersTable";
import GroupsTable from "./GroupsTable";
import Typography from "@material-ui/core/Typography";

export default function AdminApp() {
  const [adminToken, setAdminToken] = useState<string>();

  if (!adminToken) return <AuthPanel onTokenRecieved={setAdminToken} />;

  return <AdminAppContent token={adminToken} />;
}

const useStyles = makeStyles({
  adminPanel: {
    display: "flex",
    flexDirection: "column",
    width: "100%",
    height: "100%",
    padding: "2rem",
  },
  panelTitle: { textAlign: "center", marginBottom: "2rem" },
  tables: {
    flex: 1,
    display: "flex",
    "& > *": {
      flex: 1,
      "&:not(:last-child)": { marginRight: "2em" },
    },
  },
});

interface ContentProps {
  token: string;
}

function AdminAppContent({ token }: ContentProps) {
  const classes = useStyles();
  const [users, refreshUsers, changeUser, deleteUser] = useCRUDApi<orch.UserData>(token, "/users");
  const [groups, refreshGroups, changeGroup, deleteGroup] = useCRUDApi<
    orch.GroupData,
    orch.GroupPayload
  >(token, "/groups");

  useEffect(() => {
    if (!refreshUsers || !refreshGroups) return;

    (async function fetchFromApi() {
      await Promise.all([refreshUsers(), refreshGroups()]);
    })();
  }, [refreshUsers, refreshGroups]);

  return (
    <div className={classes.adminPanel}>
      <Typography variant="h4" color="primary" className={classes.panelTitle}>
        Admin Panel
      </Typography>
      <div className={classes.tables}>
        <UsersTable users={users} onUserChange={changeUser} onUserDelete={deleteUser} />
        <GroupsTable
          groups={groups}
          users={users}
          onGroupChange={changeGroup}
          onGroupDelete={deleteGroup}
        />
      </div>
    </div>
  );
}
