import React, { useCallback, useState } from "react";

import { makeStyles } from "@material-ui/core/styles";
import Dialog from "@material-ui/core/Dialog";
import Typography from "@material-ui/core/Typography";

import { useCRUDApi } from "utils/hooks";

import AuthPanel from "./AuthPanel";
import UsersTable from "./UsersTable";
import GroupsTable from "./GroupsTable";
import Button from "@material-ui/core/Button";

type UserDataWithTempPassword = orch.OptionalId<orch.UserData> & { temporaryPassword?: string };

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
  dialog: { padding: "3rem", display: "flex", flexDirection: "column", alignItems: "center" },
  dialogTitle: { marginBottom: "2rem" },
  dialogPart: { width: "100%", marginBottom: "1rem" },
  dialogMailButton: { marginTop: "1rem" },
});

interface ContentProps {
  token: string;
}

function AdminAppContent({ token }: ContentProps) {
  const classes = useStyles();
  const [createdUser, setCreatedUser] = useState<Required<UserDataWithTempPassword>>();
  const [users, _refreshUsers, changeUser, deleteUser] = useCRUDApi<orch.UserData>(
    token,
    "/admin/users"
  );
  const [groups, _refreshGroups, changeGroup, deleteGroup, setUsersQuery] = useCRUDApi<
    orch.GroupData,
    orch.GroupPayload
  >(token, "/admin/groups");

  const setUserGroupFilter = useCallback(
    (group?: orch.GroupData) => setUsersQuery({ groupId: group?.id }),
    [setUsersQuery]
  );

  const onUserChange = useCallback(
    async (user: orch.OptionalId<orch.UserData>) => {
      const result: UserDataWithTempPassword = await changeUser(user, true);
      if (result.id && result.temporaryPassword) setCreatedUser(result as any);
    },
    [changeUser]
  );

  const clearCreatedUser = useCallback(() => setCreatedUser(undefined), []);

  const sendInviteMail = useCallback(() => window.open(getMailHref(createdUser), "_blank"), [
    createdUser,
  ]);

  return (
    <div className={classes.adminPanel}>
      <Typography variant="h4" color="primary" className={classes.panelTitle}>
        Admin Panel
      </Typography>
      <div className={classes.tables}>
        <UsersTable
          groups={groups}
          users={users}
          setGroupFilter={setUserGroupFilter}
          onUserChange={onUserChange}
          onUserDelete={deleteUser}
        />
        <GroupsTable
          groups={groups}
          users={users}
          onGroupChange={changeGroup}
          onGroupDelete={deleteGroup}
        />
      </div>
      <Dialog open={!!createdUser} onClose={clearCreatedUser}>
        <div className={classes.dialog}>
          <Typography variant="h4" color="primary" className={classes.dialogTitle}>
            New User Created
          </Typography>
          <div className={classes.dialogPart}>
            <Typography variant="body1">First Name: {createdUser?.firstName}</Typography>
            <Typography variant="body1">Last Name: {createdUser?.lastName}</Typography>
          </div>
          <div className={classes.dialogPart}>
            <Typography variant="h6">Email: {createdUser?.email}</Typography>
            <Typography variant="h6">
              Temporary Password: {createdUser?.temporaryPassword}
            </Typography>
          </div>
          <Button
            variant="contained"
            color="primary"
            className={classes.dialogMailButton}
            onClick={sendInviteMail}
          >
            Invite with email
          </Button>
        </div>
      </Dialog>
    </div>
  );
}

function getMailHref(user?: UserDataWithTempPassword): string {
  if (!user) return "";

  const subject = `Hey ${user.firstName}, welcome to Orchestrate!`;

  const body = `join us at: ${document.location.origin} \nyour temporary password is: ${user.temporaryPassword}`;

  return encodeURI(`mailto:${user.email}?subject=${subject}&body=${body}`);
}
