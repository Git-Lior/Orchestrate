import React, { useCallback, useState } from "react";
import { Redirect, Route, Switch, useLocation } from "react-router-dom";

import LoginPage from "pages/login";
import GroupPage from "pages/group";
import HomePage from "pages/home";
import Layout from "pages/Layout";
import PageNotFound from "pages/PageNotFound";

const GROUPS: orch.Group[] = [
  {
    id: 123,
    name: "String Trio",
  },
  {
    id: 456,
    name: "Symphonic Orchestra",
  },
  {
    id: 789,
    name: "New Ensemble",
  },
];

function App() {
  const { pathname } = useLocation();
  const [user, setUser] = useState<orch.User | null>(null);

  const logoutUser = useCallback(() => setUser(null), [setUser]);

  if (!user) return <LoginPage onLogin={setUser} />;

  const withLayout = (c: React.ReactNode) => (
    <Layout user={user} groups={GROUPS} onLogout={logoutUser} children={c} />
  );

  return (
    <Switch>
      <Redirect from="/:url*(/+)" to={pathname.slice(0, -1)} />
      <Route exact path="/" children={withLayout(<HomePage />)} />
      <Route path="/group/:groupId/:groupPage" children={withLayout(<GroupPage user={user} />)} />
      <Route children={withLayout(<PageNotFound />)} />
    </Switch>
  );
}

export default App;
