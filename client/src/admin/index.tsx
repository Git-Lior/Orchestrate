import React, { useCallback, useEffect, useState } from "react";

import { makeStyles } from "@material-ui/core";

import AuthPanel from "./AuthPanel";
import UsersTable from "./UsersTable";
import GroupsPanel from "./GroupsPanel";
import { useApiFetch } from "utils/hooks";

const useStyles = makeStyles({
  adminPanel: { display: "flex", height: "100%" },
  usersPanel: { flex: 1 },
  groupsPanel: { flex: 1 },
});

export default function AdminApp() {
  const classes = useStyles();
  const [adminToken, setAdminToken] = useState<string>();
  const [users, setUsers] = useState<orch.UserData[]>();
  // const [groups, setGroups] = useState<orch.admin.GroupData[]>();
  const apiFetch = useApiFetch({ token: adminToken! });

  useEffect(() => {
    async function fetchFromApi() {
      if (!adminToken) return;

      const users: orch.UserData[] = await apiFetch("/users");
      setUsers(users);
    }

    fetchFromApi();
  }, [adminToken]);

  const addUser = useCallback(
    async (data: orch.UserData) => {
      const user: orch.UserData = await apiFetch("/users", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
      });

      setUsers([...users!, user]);
    },
    [users, adminToken]
  );

  const deleteUser = useCallback(
    async (data: orch.UserData) => {
      await apiFetch(`/users/${data.id}`, { method: "DELETE" }, true);
      setUsers(users!.filter(_ => _.id !== data.id));
    },
    [users, adminToken]
  );

  const changeUser = useCallback(
    async (data: orch.UserData) => {
      await apiFetch(
        `/users/${data.id}`,
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(data),
        },
        true
      );

      const userIndex = users!.findIndex(_ => _.id === data.id);
      const newUsers = [...users!];
      newUsers[userIndex] = data;

      setUsers(newUsers);
    },
    [users]
  );

  if (!adminToken) return <AuthPanel onTokenRecieved={setAdminToken} />;

  return (
    <div className={classes.adminPanel}>
      <div className={classes.usersPanel}>
        <UsersTable
          users={users}
          onUserAdd={addUser}
          onUserChange={changeUser}
          onUserDelete={deleteUser}
        />
      </div>
      <div className={classes.groupsPanel}>
        <GroupsPanel />
      </div>
    </div>
  );
}
