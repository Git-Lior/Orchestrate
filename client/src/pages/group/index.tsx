import React from "react";
import { useParams } from "react-router-dom";

import GroupConcertsPage from "./concerts";
import GroupInfoPage from "./info";
import { GroupSheetMusicPage } from "./sheet-music";

interface PageInfo {
  name: string;
  route: string;
  Component: React.ComponentType;
}

export const groupPages: PageInfo[] = [
  {
    name: "Group Info",
    route: "info",
    Component: GroupInfoPage,
  },
  {
    name: "Compositions",
    route: "sheet-music",
    Component: GroupSheetMusicPage,
  },
  {
    name: "Concerts",
    route: "concerts",
    Component: GroupConcertsPage,
  },
];

export const defaultGroupPage = "info";

export default function GroupPage() {
  const { groupPage } = useParams<orch.GroupRouteParams>();

  const page = groupPages.find(_ => _.route === groupPage);

  return !page ? null : <page.Component />;
}
