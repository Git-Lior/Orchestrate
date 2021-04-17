import React, { useCallback, useEffect, useRef, useState } from "react";

import Container from "@material-ui/core/Container";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";
import makeStyles from "@material-ui/core/styles/makeStyles";

import { ErrorText } from "utils/components";
import { useApiFetch, usePromiseStatus } from "utils/hooks";

import logo from "assets/logo.png";

import styles from "./styles";
import LoginForm from "./LoginForm";
import ChangePasswordForm from "./ChangePasswordForm";

const useStyles = makeStyles(styles);

interface LoginProps {
  onLogin: (user: orch.User, remember: boolean) => void;
}

function LoginPage({ onLogin }: LoginProps) {
  const classes = useStyles();
  const [loading, error, setPromise] = usePromiseStatus();
  const [user, setUser] = useState<orch.User>();
  const apiFetch = useApiFetch(user, "/auth");
  const loginOptions = useRef<{ password: string; rememberMe: boolean }>();

  useEffect(() => {
    if (user && !user.isPasswordTemporary) onLogin(user, !!loginOptions.current?.rememberMe);
  }, [onLogin, user]);

  const handleLogin = useCallback(
    (email: string, password: string, rememberMe: boolean) => {
      loginOptions.current = { password, rememberMe };
      return setPromise(
        apiFetch("login", {
          method: "POST",
          body: JSON.stringify({ email, password }),
        }).then(setUser)
      );
    },
    [setPromise, apiFetch]
  );

  const handleChangePassword = useCallback(
    (newPassword: string, confirmPassword: string) => {
      async function changePassword() {
        if (newPassword !== confirmPassword) throw { error: "Passwords must match" } as orch.Error;

        await apiFetch(
          "changePassword",
          {
            method: "POST",
            body: JSON.stringify({ oldPassword: loginOptions.current?.password, newPassword }),
          },
          "none"
        );

        setUser(u => ({ ...u!, isPasswordTemporary: false }));
      }

      setPromise(changePassword());
    },
    [apiFetch, setPromise]
  );

  return (
    <div className={classes.container}>
      <img src={logo} className={classes.appLogo} alt="Orchestrate" />
      <Container maxWidth="sm">
        <Paper className={classes.loginArea} elevation={5}>
          {user?.isPasswordTemporary ? (
            <ChangePasswordForm user={user} onChangePassword={handleChangePassword} />
          ) : (
            <LoginForm onLogin={handleLogin} />
          )}
          {loading && (
            <Typography variant="body1" className={classes.loginMessage}>
              {!user ? "Logging in..." : "Changing Password..."}
            </Typography>
          )}
          {error && <ErrorText error={error} className={classes.loginError} />}
        </Paper>
      </Container>
    </div>
  );
}

export default LoginPage;
