import React, { useCallback, useEffect, useState } from "react";
import { Redirect, Route, Switch, useHistory, useLocation } from "react-router-dom";

import LoginPage from "pages/login";
import GroupPage from "pages/group";
import HomePage from "pages/home";
import Layout from "pages/Layout";
import PageNotFound from "pages/PageNotFound";

import { useApiFetch, useLocalStorage } from "utils/hooks";

const TOKEN_STORAGE_KEY = "user_token";

function App() {
  const history = useHistory();
  const { pathname } = useLocation();
  const [user, setUser] = useState<orch.User>();
  const [storedToken, setStoredToken] = useLocalStorage(TOKEN_STORAGE_KEY);
  const apiFetch = useApiFetch(user ?? { token: storedToken }, "/auth");

  useEffect(() => {
    if (storedToken) apiFetch("info").then(u => setUser({ ...u, token: storedToken }));
  }, []);

  const logoutUser = useCallback(() => {
    setUser(undefined);
    setStoredToken(undefined);
    history.push("/");
  }, [setUser, setStoredToken, history]);

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
      if (remember) setStoredToken(result.token);
    },
    [setUser, setStoredToken]
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
        path="/group/:groupId/:groupPage?"
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
