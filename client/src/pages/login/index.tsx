import React, { useCallback, useState } from "react";

import Container from "@material-ui/core/Container";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";
import TextField from "@material-ui/core/TextField";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Button from "@material-ui/core/Button";
import makeStyles from "@material-ui/core/styles/makeStyles";

import styles from "./styles";
import { useInputState, useApiPromise } from "utils/hooks";
import logo from "assets/logo.png";

const useStyles = makeStyles(styles);

interface LoginProps {
  onLogin: (user: orch.User, remember: boolean) => void;
}

function LoginPage({ onLogin }: LoginProps) {
  const classes = useStyles();
  const [rememberMe, setRememberMe] = useState(false);
  const [email, setEmail] = useInputState();
  const [password, setPassword] = useInputState();
  const [loading, error, setLoginPromise] = useApiPromise();

  const _onFormSubmit = useCallback(
    async (e: React.FormEvent<any>) => {
      e.preventDefault();

      const result: orch.User = await setLoginPromise(
        fetch("/api/auth/login", {
          method: "POST",
          headers: { "content-type": "application/json" },
          body: JSON.stringify({ email, password }),
        })
      );

      onLogin(result, rememberMe);
    },
    [rememberMe, email, password, onLogin]
  );

  const changeRememberMe = useCallback((_, checked: boolean) => setRememberMe(checked), [
    setRememberMe,
  ]);

  return (
    <div className={classes.container}>
      <img src={logo} className={classes.appLogo} alt="Orchestrate" />
      <Container maxWidth="sm">
        <Paper className={classes.loginArea} elevation={5}>
          <Typography variant="h4" className={classes.loginTitle}>
            Log in
          </Typography>
          <form onSubmit={_onFormSubmit}>
            <TextField
              value={email}
              onChange={setEmail}
              required
              fullWidth
              autoFocus
              variant="outlined"
              margin="normal"
              id="email"
              name="email"
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
              id="password"
              name="password"
              label="Password"
              autoComplete="current-password"
            />
            <FormControlLabel
              control={<Checkbox value="remember" color="primary" onChange={changeRememberMe} />}
              label="Remember me"
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              className={classes.submitButton}
            >
              Log in
            </Button>
          </form>
          {loading && (
            <Typography variant="body1" className={classes.loginMessage}>
              Logging in...
            </Typography>
          )}
          {error && (
            <Typography variant="body1" color="error" className={classes.loginMessage}>
              {error}
            </Typography>
          )}
        </Paper>
      </Container>
    </div>
  );
}

export default LoginPage;
