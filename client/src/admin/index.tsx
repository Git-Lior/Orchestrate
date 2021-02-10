import React, { useEffect, useState } from "react";

import { makeStyles } from "@material-ui/core";

import AuthPanel from "./AuthPanel";
import UsersTable from "./UsersTable";
import GroupsTable from "./GroupsTable";
import { useCRUDApi } from "utils/hooks";

export default function AdminApp() {
  const [adminToken, setAdminToken] = useState<string>();

  if (!adminToken) return <AuthPanel onTokenRecieved={setAdminToken} />;

  return <AdminAppContent token={adminToken} />;
}

const useStyles = makeStyles({
  adminPanel: { display: "flex", height: "100%" },
  dataTables: { display: "flex", flexDirection: "column", width: 700, padding: "2em" },
  spacer: { height: "2em" },
});

interface ContentProps {
  token: string;
}

function AdminAppContent({ token }: ContentProps) {
  const classes = useStyles();

  const [users, refreshUsers, addUser, changeUser, deleteUser] = useCRUDApi<orch.UserData>(
    token,
    "/users"
  );
  const [groups, refreshGroups, addGroup, changeGroup, deleteGroup] = useCRUDApi<orch.GroupData>(
    token,
    "/groups"
  );

  useEffect(() => {
    (async function fetchFromApi() {
      await Promise.all([refreshUsers(), refreshGroups()]);
    })();
  }, []);

  return (
    <div className={classes.adminPanel}>
      <div className={classes.dataTables}>
        <UsersTable
          users={users}
          onUserAdd={addUser}
          onUserChange={changeUser}
          onUserDelete={deleteUser}
        />
        <div className={classes.spacer} />
        <GroupsTable
          groups={groups}
          users={users}
          onGroupAdd={addGroup}
          onGroupChange={changeGroup}
          onGroupDelete={deleteGroup}
        />
      </div>
    </div>
  );
}
