import React, { useCallback } from "react";

import makeStyles from "@material-ui/core/styles/makeStyles";
import TextField from "@material-ui/core/TextField";
import Typography from "@material-ui/core/Typography";
import Card from "@material-ui/core/Card";
import Button from "@material-ui/core/Button";
import { useApiPromise, useInputState } from "utils/hooks";

const useStyles = makeStyles({
  authContainer: {
    width: "100%",
    height: "100%",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
  },
  authPanel: {
    padding: "3em",
    maxWidth: "600px",
    display: "flex",
    flexDirection: "column",
  },
  submitButton: { margin: "1em 0", alignSelf: "center" },
});

interface Props {
  onTokenRecieved: (token: string) => void;
}

export default function AuthPanel({ onTokenRecieved }: Props) {
  const classes = useStyles();
  const [adminTokenInput, setAdminTokenInputValue] = useInputState();
  const [tokenLoading, tokenError, setTokenPromise] = useApiPromise(true);

  const onSubmit = useCallback(async () => {
    if (!adminTokenInput) return;

    const token: string = await setTokenPromise(
      fetch("/api/auth/admin", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(adminTokenInput),
      })
    );

    onTokenRecieved(token);
  }, [adminTokenInput]);

  return (
    <div className={classes.authContainer}>
      <Card className={classes.authPanel}>
        <Typography variant="h4">Admin panel authentication</Typography>
        <TextField
          type="password"
          label="password"
          fullWidth
          value={adminTokenInput}
          onChange={setAdminTokenInputValue}
        />
        <Button
          type="submit"
          variant="contained"
          color="primary"
          className={classes.submitButton}
          onClick={onSubmit}
        >
          Log in
        </Button>
        {tokenLoading && <Typography variant="body1">loading...</Typography>}
        {tokenError && (
          <Typography variant="body1" color="primary">
            {tokenError}
          </Typography>
        )}
      </Card>
    </div>
  );
}
