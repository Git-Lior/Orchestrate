import GroupConcertsPage from "./concerts";
import GroupInfoPage from "./info";
import GroupCompositionsPage from "./compositions";

interface PageInfo {
  name: string;
  route: string;
  isEnabled: (userInfo: orch.group.UserInfo) => boolean;
  Component: React.ComponentType<Required<orch.PageProps>>;
}

export const groupPages: PageInfo[] = [
  {
    name: "Group Info",
    route: "info",
    Component: GroupInfoPage,
    isEnabled: () => true,
  },
  {
    name: "Compositions",
    route: "compositions",
    Component: GroupCompositionsPage,
    isEnabled: u => u.director || u.roles.length > 0,
  },
  {
    name: "Concerts",
    route: "concerts",
    Component: GroupConcertsPage,
    isEnabled: u => u.manager || u.roles.length > 0,
  },
];

export const defaultGroupPage = "info";
