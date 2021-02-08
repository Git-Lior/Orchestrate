import React, { useCallback, useMemo, useState } from "react";

import makeStyles from "@material-ui/core/styles/makeStyles";
import IconButton from "@material-ui/core/IconButton";
import Typography from "@material-ui/core/Typography";
import Paper from "@material-ui/core/Paper";
import DeleteIcon from "@material-ui/icons/Delete";
import EditIcon from "@material-ui/icons/Edit";
import AddIcon from "@material-ui/icons/Add";
import TextField from "@material-ui/core/TextField";
import Fab from "@material-ui/core/Fab";
import Button from "@material-ui/core/Button";
import CircularProgress from "@material-ui/core/CircularProgress";

import { DataGrid, ColDef, CellParams } from "@material-ui/data-grid";

import { useInputState, usePromiseStatus } from "utils/hooks";

const useStyles = makeStyles({
  container: { display: "flex", flexDirection: "column", alignItems: "center" },
  panelTitle: { margin: "2rem 0" },
  searchBar: { marginBottom: "1rem" },
  addButton: { float: "right" },
  tableContainer: { width: 702, margin: "0 5rem" },
  usersTable: { height: 400 },
  editUserContainer: {},
  editRow: {},
  editTitle: {},
});

const DATA_COLUMNS: ColDef[] = [
  { field: "id", headerName: "ID", width: 75 },
  { field: "firstName", headerName: "First Name", width: 150 },
  { field: "lastName", headerName: "Last Name", width: 150 },
  { field: "email", headerName: "Email", width: 200 },
];

const EMPTY_USER: orch.UserData = {
  id: NaN,
  firstName: "",
  lastName: "",
  email: "",
};

interface Props {
  users?: orch.UserData[];
  onUserAdd: (data: orch.UserData) => Promise<void>;
  onUserChange: (data: orch.UserData) => Promise<void>;
  onUserDelete: (data: orch.UserData) => Promise<void>;
}

export default function UsersPanel({ users, onUserAdd, onUserChange, onUserDelete }: Props) {
  const classes = useStyles();
  const [editedUser, setEditedUser] = useState<orch.UserData>();
  const [userFilter, setUserFilter] = useInputState();
  const [editLoading, editError, setEditPromise] = usePromiseStatus();

  const columns = useMemo(
    () => [
      ...DATA_COLUMNS,
      {
        field: "",
        headerName: "Actions",
        width: 125,
        disableClickEventBubbling: true,
        sortable: false,
        renderCell: (params: CellParams) => (
          <>
            <IconButton aria-label="delete" onClick={() => onUserDelete(params.row as any)}>
              <DeleteIcon />
            </IconButton>
            <IconButton aria-label="edit" onClick={() => setEditedUser(params.row as any)}>
              <EditIcon />
            </IconButton>
          </>
        ),
      },
    ],
    [onUserDelete, setEditedUser]
  );

  const filteredUsers = useMemo(() => {
    if (!users || !userFilter) return users;

    return users.filter(({ id, email, lastName, firstName }) =>
      [id.toString(), firstName, lastName, email].some(_ => _.toString().includes(userFilter))
    );
  }, [users, userFilter]);

  const addUser = useCallback(() => setEditedUser(EMPTY_USER), [setEditedUser]);

  const setFirstName = useCallback(
    (e: React.ChangeEvent<any>) => setEditedUser({ ...editedUser!, firstName: e.target.value }),
    [editedUser, setEditedUser]
  );

  const setLastName = useCallback(
    (e: React.ChangeEvent<any>) => setEditedUser({ ...editedUser!, lastName: e.target.value }),
    [editedUser, setEditedUser]
  );

  const setEmail = useCallback(
    (e: React.ChangeEvent<any>) => setEditedUser({ ...editedUser!, email: e.target.value }),
    [editedUser, setEditedUser]
  );

  const onEditDone = useCallback(() => {
    const action = editedUser!.id ? onUserChange : onUserAdd;
    setEditPromise(action(editedUser!)).then(() => setEditedUser(undefined));
  }, [editedUser, setEditPromise, setEditedUser, onUserAdd, onUserChange]);

  return (
    <div className={classes.container}>
      <Typography variant="h4" color="primary" className={classes.panelTitle}>
        Users
      </Typography>
      <div className={classes.tableContainer}>
        <TextField
          label="search..."
          className={classes.searchBar}
          value={userFilter}
          onChange={setUserFilter}
        />
        <Fab color="primary" size="medium" onClick={addUser} className={classes.addButton}>
          <AddIcon />
        </Fab>
        <Paper className={classes.usersTable}>
          <DataGrid
            loading={!users}
            rows={filteredUsers ?? []}
            columns={columns}
            autoPageSize
            disableColumnMenu
            disableSelectionOnClick
          />
        </Paper>
      </div>
      {editedUser && (
        <div className={classes.editUserContainer}>
          <Typography variant="h5">Edit User</Typography>
          <div className={classes.editRow}>
            <Typography variant="body1" className={classes.editTitle}>
              First Name
            </Typography>
            <TextField value={editedUser.firstName} onChange={setFirstName} />
          </div>
          <div className={classes.editRow}>
            <Typography variant="body1" className={classes.editTitle}>
              Last Name
            </Typography>
            <TextField value={editedUser.lastName} onChange={setLastName} />
          </div>
          <div className={classes.editRow}>
            <Typography variant="body1" className={classes.editTitle}>
              Email Name
            </Typography>
            <TextField value={editedUser.email} onChange={setEmail} />
          </div>
          <Button disabled={editLoading} variant="contained" color="primary" onClick={onEditDone}>
            {editedUser.id ? "Save" : "Create"}
          </Button>
          {editLoading && <CircularProgress color="primary" />}
        </div>
      )}
    </div>
  );
}
