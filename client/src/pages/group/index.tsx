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
    name: "פרטי ההרכב",
    route: "info",
    Component: GroupInfoPage,
  },
  {
    name: "יצירות",
    route: "sheet-music",
    Component: GroupSheetMusicPage,
  },
  {
    name: "הופעות",
    route: "concerts",
    Component: GroupConcertsPage,
  },
];

export default function GroupPage() {
  const { groupPage } = useParams<orch.RouteMatch>();

  const page = groupPages.find(_ => _.route === groupPage);

  return !page ? null : <page.Component />;
}
