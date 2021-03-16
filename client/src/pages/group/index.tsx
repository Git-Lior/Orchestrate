import React from "react";
import { Redirect, useParams } from "react-router-dom";

import { groupPages, defaultGroupPage } from "./pages";

export default function GroupPage({ user, group, userInfo, groups, setGroup }: orch.PageProps) {
  const { groupPage, groupId } = useParams<orch.group.RouteParams>();

  const page = groupPages.find(_ => _.route === groupPage);

  if (!group) return <div>loading group info...</div>;

  if (!page || !page.isEnabled(userInfo!))
    return <Redirect to={`/group/${groupId}/${defaultGroupPage}`} />;

  return (
    <page.Component
      user={user}
      groups={groups!}
      group={group}
      userInfo={userInfo!}
      setGroup={setGroup}
    />
  );
}

export { groupPages, defaultGroupPage } from "./pages";
