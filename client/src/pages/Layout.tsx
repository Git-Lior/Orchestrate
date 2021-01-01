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

import makeStyles from "@material-ui/core/styles/makeStyles";
import layoutStyles from "./Layout.styles";
import smallLogo from "assets/logo-small.png";

import { groupPages } from "./group";

const useStyles = makeStyles(layoutStyles);

type Props = React.PropsWithChildren<{
  user: orch.User;
  groups: orch.Group[];
  onLogout: () => void;
}>;

export default function Layout({ user, groups, onLogout, children }: Props) {
  const classes = useStyles();
  const history = useHistory();
  const { params } = useRouteMatch<orch.RouteMatch>();
  const groupId = Number(params.groupId) || 0;
  const groupPage = params.groupPage;

  const setGroup = useCallback(
    (e: React.ChangeEvent<any>) =>
      history.push(`/group/${e.target.value}/${groupPage ?? groupPages[0].route}`),
    [history, groupId]
  );

  const setGroupPage = useCallback(
    (_, value: string) => history.push(`/group/${groupId}/${value}`),
    [history, groupId]
  );

  return (
    <>
      <AppBar position="static">
        <Toolbar disableGutters className={classes.toolbar}>
          <Link to="/" className={classes.noLineHeight}>
            <img src={smallLogo} className={classes.appLogo} alt="Orchestrate" />
          </Link>
          <Select
            className={classes.groupSelect}
            value={groupId}
            displayEmpty
            disableUnderline
            onChange={setGroup}
          >
            <MenuItem value={0} disabled>
              בחר הרכב...
            </MenuItem>
            {groups.map(({ id, name }) => (
              <MenuItem key={id} value={id}>
                <Link to={`/group/${id}/${groupPage ?? groupPages[0].route}`}>{name}</Link>
              </MenuItem>
            ))}
          </Select>
          <Tabs
            className={classes.groupTabs}
            disabled={groupId === 0}
            value={groupPage}
            onChange={setGroupPage}
            indicatorColor="secondary"
          >
            {groupPages.map(_ => (
              <Tab
                disableRipple
                key={_.route}
                value={_.route}
                label={<Typography variant="h6">{_.name}</Typography>}
              />
            ))}
          </Tabs>
          <div className={classes.buttonsSpacer} />
          <Typography>
            {user.firstName} {user.lastName}
          </Typography>
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
