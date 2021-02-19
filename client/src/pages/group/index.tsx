import React from "react";
import { useParams } from "react-router-dom";

import { groupPages } from "./pages";
export { groupPages, defaultGroupPage } from "./pages";

export default function GroupPage({ user, group, userInfo, groups, setGroup }: orch.PageProps) {
  const { groupPage } = useParams<orch.group.RouteParams>();

  const page = groupPages.find(_ => _.route === groupPage);

  if (!group) return <div>loading group info...</div>;

  return !page ? null : (
    <page.Component
      user={user}
      groups={groups!}
      group={group}
      userInfo={userInfo!}
      setGroup={setGroup}
    />
  );
}
