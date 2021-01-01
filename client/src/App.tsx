import React, { useCallback, useState } from "react";
import { BrowserRouter, Route, Switch } from "react-router-dom";

import LoginPage from "pages/login";
import GroupPage from "pages/group";
import HomePage from "pages/home";
import Layout from "pages/Layout";
import PageNotFound from "pages/PageNotFound";

const GROUPS: orch.Group[] = [
  {
    id: 123,
    name: "רביעיית כלי מיתר",
  },
  {
    id: 456,
    name: "תזמורת סמפונית",
  },
  {
    id: 789,
    name: "הרכב חדש",
  },
];

function App() {
  const [user, setUser] = useState<orch.User | null>(null);

  const logoutUser = useCallback(() => setUser(null), [setUser]);

  if (!user) return <LoginPage onLogin={setUser} />;

  const withLayout = (c: React.ReactNode) => (
    <Layout user={user} groups={GROUPS} onLogout={logoutUser} children={c} />
  );

  return (
    <BrowserRouter>
      <Switch>
        <Route exact path="/" children={withLayout(<HomePage />)} />
        <Route path="/group/:groupId/:groupPage" children={withLayout(<GroupPage />)} />
        <Route children={withLayout(<PageNotFound />)} />
      </Switch>
    </BrowserRouter>
  );
}

export default App;
