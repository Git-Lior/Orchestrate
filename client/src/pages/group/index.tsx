import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useApiFetch } from "utils/hooks";

import { MOCK_USER_INFO } from "mocks";
import { groupPages } from "./pages";
export { groupPages, defaultGroupPage } from "./pages";

interface Props {
  user: orch.User;
}

export default function GroupPage({ user }: Props) {
  const apiFetch = useApiFetch(user, "/groups");
  const { groupId, groupPage } = useParams<orch.group.RouteParams>();
  const [userInfo, setUserInfo] = useState<orch.group.UserInfo | null>(MOCK_USER_INFO);

  useEffect(() => {
    /*
    setUserInfo(null);
    apiFetch(`/${groupId}/userInfo`).then(setUserInfo);
    */
  }, [groupId, apiFetch]);

  const page = groupPages.find(_ => _.route === groupPage);

  if (!userInfo) return <div>loading user info...</div>;

  return !page ? null : <page.Component user={user} userInfo={userInfo} />;
}
