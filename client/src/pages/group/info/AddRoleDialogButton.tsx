import React, { useCallback, useEffect, useState } from "react";

import { lighten, makeStyles } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import IconButton from "@material-ui/core/IconButton";
import AddIcon from "@material-ui/icons/Add";
import TextField from "@material-ui/core/TextField";

import { useApiFetch, useInputState, usePromiseStatus } from "utils/hooks";
import { FormDialog } from "utils/components";

const useStyles = makeStyles(theme => ({
  addButton: {
    color: theme.palette.background.default,
    backgroundColor: theme.palette.primary.main,
    "&:hover": { backgroundColor: lighten(theme.palette.primary.main, 0.2) },
  },
  formRow: { display: "flex", marginBottom: "1em" },
  formRowLabel: { marginRight: "10px", fontSize: "18px" },
}));

type Props = Required<orch.PageProps>;

export default function AddRoleDialogButton({ user, group, setGroup }: Props) {
  const classes = useStyles();
  const apiFetch = useApiFetch(user, `/groups/${group.id}/roles`);
  const [loading, error, setPromise] = usePromiseStatus();

  const [addRoleOpen, setAddRoleOpen] = useState(false);
  const [newRoleSection, setNewRoleSection] = useInputState();
  const [newRoleNum, setNewRoleNum] = useInputState();

  const addRole = useCallback(() => {
    async function addRoleWorker() {
      if (!newRoleSection) throw { error: "Must supply section name" } as orch.Error;

      const newRole: orch.RoleWithMembers = await apiFetch("", {
        method: "POST",
        body: JSON.stringify({
          section: newRoleSection,
          num: newRoleNum || undefined,
        }),
      });

      setGroup(group => ({ ...group!, roles: [...group!.roles, newRole] }));

      setAddRoleOpen(false);
    }
    setPromise(addRoleWorker());
  }, [newRoleSection, newRoleNum, setGroup]);

  useEffect(() => {
    if (!addRoleOpen) {
      setNewRoleSection();
      setNewRoleNum();
    }
  }, [addRoleOpen]);

  const toggleAddRoleDialog = useCallback(() => setAddRoleOpen(v => !v), []);

  return (
    <>
      <IconButton size="small" className={classes.addButton} onClick={toggleAddRoleDialog}>
        <AddIcon />
      </IconButton>
      <FormDialog
        open={addRoleOpen}
        title="Add Role"
        loading={loading}
        error={error}
        onClose={toggleAddRoleDialog}
        onDone={addRole}
      >
        <div className={classes.formRow}>
          <Typography variant="body1" className={classes.formRowLabel}>
            Section
          </Typography>
          <TextField value={newRoleSection} onChange={setNewRoleSection} />
        </div>
        <div className={classes.formRow}>
          <Typography variant="body1" className={classes.formRowLabel}>
            Number (optional)
          </Typography>
          <TextField value={newRoleNum} onChange={setNewRoleNum} />
        </div>
      </FormDialog>
    </>
  );
}
