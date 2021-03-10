import React, { useCallback, useEffect, useState } from "react";
import { Redirect, Route, Switch, useLocation } from "react-router-dom";

import LoginPage from "pages/login";
import GroupPage from "pages/group";
import HomePage from "pages/home";
import Layout from "pages/Layout";
import PageNotFound from "pages/PageNotFound";

import { useApiFetch } from "utils/hooks";

const TOKEN_STORAGE_KEY = "user_token";

function App() {
  const { pathname } = useLocation();
  const [user, setUser] = useState<orch.User>();
  const apiFetch = useApiFetch(user, "/auth");

  useEffect(() => {
    const token = localStorage.getItem(TOKEN_STORAGE_KEY);
    if (token) {
      fetch("/api/auth/info", {
        headers: { authorization: "Bearer " + token },
      })
        .then(_ => _.json())
        .then(u => setUser({ ...u, token }));
    }
  }, []);

  const logoutUser = useCallback(() => {
    setUser(undefined);
    localStorage.removeItem(TOKEN_STORAGE_KEY);
  }, [setUser]);

  const changePassword = useCallback(
    async (oldPassword: string, newPassword: string) =>
      apiFetch(
        "changePassword",
        {
          method: "POST",
          body: JSON.stringify({ oldPassword, newPassword }),
        },
        "none"
      ).then(() => setUser(u => ({ ...u!, isPasswordTemporary: false }))),
    [apiFetch]
  );

  const onUserLogin = useCallback(
    (result: orch.User, remember: boolean) => {
      setUser(result);
      if (remember) localStorage.setItem(TOKEN_STORAGE_KEY, result.token);
    },
    [setUser]
  );

  if (!user) return <LoginPage onLogin={onUserLogin} />;

  return (
    <Switch>
      <Redirect from="/:url*(/+)" to={pathname.slice(0, -1)} />
      <Route
        exact
        path="/"
        children={
          <Layout
            user={user}
            onLogout={logoutUser}
            onPasswordChange={changePassword}
            page={HomePage}
          />
        }
      />
      <Route
        path="/group/:groupId/:groupPage"
        children={
          <Layout
            user={user}
            onLogout={logoutUser}
            onPasswordChange={changePassword}
            page={GroupPage}
          />
        }
      />
      <Route
        children={
          <Layout
            user={user}
            onLogout={logoutUser}
            onPasswordChange={changePassword}
            page={PageNotFound}
          />
        }
      />
    </Switch>
  );
}

export default App;
