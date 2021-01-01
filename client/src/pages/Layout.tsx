import "./Layout.css";

import React, { useCallback } from "react";
import { Link } from "react-router-dom";
import { useRouteMatch, useHistory } from "react-router";

import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import Select from "@material-ui/core/Select";
import MenuItem from "@material-ui/core/MenuItem";
import Tabs from "@material-ui/core/Tabs";
import Tab from "@material-ui/core/Tab";
import Typography from "@material-ui/core/Typography";
import NotificationsIcon from "@material-ui/icons/Notifications";
import IconButton from "@material-ui/core/IconButton";
import Badge from "@material-ui/core/Badge";
import ExitToAppIcon from "@material-ui/icons/ExitToApp";
import Tooltip from "@material-ui/core/Tooltip";

import smallLogo from "assets/logo-small.png";

type Props = React.PropsWithChildren<{
  groups: orch.Group[];
  onLogout: () => void;
}>;

const GROUP_PAGES: any[] = [
  {
    name: "פרטי ההרכב",
    route: "info",
  },
  {
    name: "יצירות",
    route: "sheet-music",
  },
  {
    name: "הופעות",
    route: "concerts",
  },
];

export default function Layout({ groups, onLogout, children }: Props) {
  const history = useHistory();
  const { params, url } = useRouteMatch<orch.RouteMatch>();
  const groupId = Number(params.groupId) || 0;
  const tabValue = url.split("/")[3];

  const _setPage = useCallback((_, value: string) => history.push(`/group/${groupId}/${value}`), [
    history,
    groupId,
  ]);

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Link to="/">
            <img src={smallLogo} className="layout-app-logo" alt="Orchestrate" />
          </Link>
          <Select className="layout-group-select" value={groupId} displayEmpty>
            <MenuItem value={0} disabled>
              בחר הרכב...
            </MenuItem>
            {groups.map(({ id, name }) => (
              <MenuItem key={id} value={id}>
                <Link to={`/group/${id}/${tabValue ?? GROUP_PAGES[0].route}`}>{name}</Link>
              </MenuItem>
            ))}
          </Select>
          <Tabs
            className="layout-group-tabs"
            disabled={groupId === 0}
            value={tabValue}
            onChange={_setPage}
            indicatorColor="secondary"
          >
            {GROUP_PAGES.map(_ => (
              <Tab
                disableRipple
                key={_.route}
                value={_.route}
                label={<Typography variant="h6">{_.name}</Typography>}
              />
            ))}
          </Tabs>
          <div className="layout-buttons-spacer" />
          <IconButton color="inherit">
            <Badge
              badgeContent={10}
              overlap="circle"
              color="secondary"
              anchorOrigin={{ vertical: "top", horizontal: "left" }}
            >
              <NotificationsIcon />
            </Badge>
          </IconButton>
          <Tooltip title="התנתק">
            <IconButton edge="end" onClick={onLogout} color="inherit">
              <ExitToAppIcon />
            </IconButton>
          </Tooltip>
        </Toolbar>
      </AppBar>
      {children}
    </>
  );
}
