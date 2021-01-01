import React, { useCallback } from "react";

import Container from "@material-ui/core/Container";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";
import TextField from "@material-ui/core/TextField";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Button from "@material-ui/core/Button";
import makeStyles from "@material-ui/core/styles/makeStyles";

import styles from "./styles";
import { useInputState } from "utils/hooks";
import logo from "assets/logo.png";

const useStyles = makeStyles(styles);

interface LoginProps {
  onLogin: (user: orch.User) => void;
}

function LoginPage({ onLogin }: LoginProps) {
  const classes = useStyles();
  const [email, setEmail] = useInputState();
  const [password, setPassword] = useInputState();

  const _onFormSubmit = useCallback(
    async (e: React.FormEvent<any>) => {
      e.preventDefault();

      const result = await fetch("/auth/login", {
        method: "POST",
        headers: new Headers({ "content-type": "application/json" }),
        body: JSON.stringify({ email, password }),
      });

      if (!result.ok) return;

      const user: orch.User = await result.json();
      onLogin(user);
    },
    [email, password, onLogin]
  );

  return (
    <div className={classes.container}>
      <img src={logo} className={classes.appLogo} alt="Orchestrate" />
      <Container maxWidth="sm">
        <Paper className={classes.loginArea} elevation={5}>
          <Typography variant="h4" align="center">
            התחברות
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
              label="כתובת מייל"
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
              label="סיסמה"
              autoComplete="current-password"
            />
            <FormControlLabel
              control={<Checkbox value="remember" color="primary" />}
              label="זכור אותי"
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              className={classes.submitButton}
            >
              התחבר
            </Button>
          </form>
        </Paper>
      </Container>
    </div>
  );
}

export default LoginPage;
