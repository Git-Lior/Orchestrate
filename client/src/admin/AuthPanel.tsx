import React, { useCallback, useEffect } from "react";

import makeStyles from "@material-ui/core/styles/makeStyles";
import TextField from "@material-ui/core/TextField";
import Typography from "@material-ui/core/Typography";
import Card from "@material-ui/core/Card";
import Button from "@material-ui/core/Button";
import { useApiFetch, useInputState, usePromiseStatus } from "utils/hooks";

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
  const apiFetch = useApiFetch(undefined, "/auth");
  const [adminTokenInput, setAdminTokenInputValue] = useInputState();
  const [tokenLoading, tokenError, setTokenPromise] = usePromiseStatus();

  useEffect(() => {
    if (tokenError) setAdminTokenInputValue(undefined);
  }, [tokenError]);

  const onSubmit = useCallback(() => {
    if (!adminTokenInput) return;

    return setTokenPromise(
      apiFetch(
        "admin",
        { method: "POST", body: JSON.stringify(adminTokenInput) },
        "text"
      ).then(token => onTokenRecieved(token))
    );
  }, [adminTokenInput, apiFetch, onTokenRecieved, setTokenPromise]);

  const onKeyPress = useCallback(
    (e: React.KeyboardEvent<any>) => {
      if (e.key === "Enter") onSubmit();
    },
    [onSubmit]
  );

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
          onKeyPress={onKeyPress}
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
            {tokenError.error}
          </Typography>
        )}
      </Card>
    </div>
  );
}
