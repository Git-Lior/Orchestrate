import "./App.css";
import React, { useState } from "react";

import LoginPage from "pages/login";

function App() {
  const [user, setUser] = useState<orch.User | null>(null);

  return !user ? (
    <LoginPage onLogin={setUser} />
  ) : (
    <div>ברוך הבא, {`${user.firstName} ${user.lastName}`}!</div>
  );
}

export default App;
