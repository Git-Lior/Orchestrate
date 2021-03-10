import React, { useCallback, useState } from "react";

import makeStyles from "@material-ui/core/styles/makeStyles";
import TextField from "@material-ui/core/TextField";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";

import { useInputState } from "utils/hooks";

const useStyles = makeStyles({
  loginTitle: {
    textAlign: "center",
  },
  submitButton: {
    marginTop: "15px",
  },
});

interface Props {
  onLogin: (email: string, password: string, remember: boolean) => void;
}

export default function LoginForm({ onLogin }: Props) {
  const classes = useStyles();

  const [email, setEmail] = useInputState();
  const [password, setPassword] = useInputState();
  const [rememberMe, setRememberMe] = useState(false);

  const changeRememberMe = useCallback((_, checked: boolean) => setRememberMe(checked), [
    setRememberMe,
  ]);

  const onLoginClick = useCallback(() => {
    onLogin(email, password, rememberMe);
  }, [onLogin, email, password, rememberMe]);

  return (
    <>
      <Typography variant="h4" className={classes.loginTitle}>
        Log in
      </Typography>
      <TextField
        value={email}
        onChange={setEmail}
        required
        fullWidth
        autoFocus
        variant="outlined"
        margin="normal"
        autoComplete="email"
        label="Email address"
      />
      <TextField
        value={password}
        onChange={setPassword}
        type="password"
        required
        fullWidth
        variant="outlined"
        margin="normal"
        label="Password"
        autoComplete="current-password"
      />
      <FormControlLabel
        control={<Checkbox value="remember" color="primary" onChange={changeRememberMe} />}
        label="Remember me"
      />
      <Button
        fullWidth
        variant="contained"
        color="primary"
        className={classes.submitButton}
        onClick={onLoginClick}
      >
        Log in
      </Button>
    </>
  );
}
