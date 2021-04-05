import React, { useState } from "react";

import AuthPanel from "./AuthPanel";
import AdminPanel from "./AdminPanel";

export default function AdminApp() {
  const [adminToken, setAdminToken] = useState<string>();

  if (!adminToken) return <AuthPanel onTokenRecieved={setAdminToken} />;

  return <AdminPanel token={adminToken} />;
}
