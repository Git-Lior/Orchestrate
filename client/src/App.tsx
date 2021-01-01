import "./App.css";
import React, { useCallback, useState } from "react";
import { BrowserRouter, Route, Switch } from "react-router-dom";

import LoginPage from "pages/login";
import Layout from "pages/Layout";

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
    <Layout groups={GROUPS} onLogout={logoutUser}>
      {c}
    </Layout>
  );

  return (
    <BrowserRouter>
      <Switch>
        <Route exact path="/" children={withLayout(<div>Home Page</div>)} />
        <Route path="/group/:groupId/info" children={withLayout(<div>Group Info Page</div>)} />
        <Route
          path="/group/:groupId/sheet-music"
          children={withLayout(<div>Sheet Music Page</div>)}
        />
        <Route path="/group/:groupId/concerts" children={withLayout(<div>Concerts Page</div>)} />
      </Switch>
    </BrowserRouter>
  );
}

export default App;
