import React, { useCallback } from "react";

import { makeStyles } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import TextField from "@material-ui/core/TextField";

import { useInputState } from "utils/hooks";

interface Props {
  user: orch.UserData;
  onChangePassword: (newPassword: string, confirmPassword: string) => void;
}

const useStyles = makeStyles({
  loginTitle: {
    textAlign: "center",
  },
  submitButton: {
    marginTop: "15px",
  },
});

export default function ChangePasswordForm({ onChangePassword, user }: Props) {
  const classes = useStyles();
  const [newPassword, setNewPassword] = useInputState();
  const [confirmPassword, setConfirmPassword] = useInputState();

  const onChangePasswordClick = useCallback(() => onChangePassword(newPassword, confirmPassword), [
    onChangePassword,
    newPassword,
    confirmPassword,
  ]);

  return (
    <>
      <div className={classes.loginTitle}>
        <Typography variant="h4">Hey {user.firstName}, Welcome To Orchestrate!</Typography>
        <Typography variant="h6">Before you begin, choose a password of your own:</Typography>
      </div>
      <TextField
        value={newPassword}
        onChange={setNewPassword}
        required
        fullWidth
        autoFocus
        variant="outlined"
        margin="normal"
        label="New password"
        type="password"
      />
      <TextField
        value={confirmPassword}
        onChange={setConfirmPassword}
        required
        fullWidth
        variant="outlined"
        margin="normal"
        label="Confirm password"
        type="password"
      />
      <Button
        fullWidth
        variant="contained"
        color="primary"
        className={classes.submitButton}
        onClick={onChangePasswordClick}
      >
        Change Password
      </Button>
    </>
  );
}
