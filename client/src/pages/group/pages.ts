import GroupConcertsPage from "./concerts";
import GroupInfoPage from "./info";
import GroupCompositionsPage from "./compositions";

export const groupPages: orch.group.PageInfo[] = [
  {
    name: "Group Info",
    route: "info",
    Component: GroupInfoPage,
  },
  {
    name: "Compositions",
    route: "compositions",
    Component: GroupCompositionsPage,
  },
  {
    name: "Concerts",
    route: "concerts",
    Component: GroupConcertsPage,
  },
];

export const defaultGroupPage = "info";
