import React, { useCallback, useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { generatePath, useHistory, useParams } from "react-router";

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

import { useApiFetch } from "utils/hooks";
import { groupPages, defaultGroupPage } from "./group";

const useStyles = makeStyles(layoutStyles);

interface Props {
  user: orch.User;
  onLogout: () => void;
  page: React.ComponentType<orch.PageProps>;
}

const GROUP_ROUTE_PATH = "/group/:groupId/:groupPage";

export default function Layout({ user, onLogout, page: Page }: Props) {
  const classes = useStyles();
  const apiFetch = useApiFetch(user, "/groups");
  const history = useHistory<orch.group.RouteParams>();

  const { groupId, groupPage } = useParams<orch.group.RouteParams>();
  const [groups, setGroups] = useState<orch.GroupData[]>();
  const [group, setGroup] = useState<orch.Group>();

  useEffect(() => {
    (async function () {
      setGroups(await apiFetch(""));
    })();
  }, [apiFetch]);

  useEffect(() => {
    (async function () {
      setGroup(undefined);
      if (groupId) setGroup(await apiFetch(groupId.toString()));
    })();
  }, [groupId, apiFetch]);

  const userInfo: orch.group.UserInfo | undefined = useMemo(
    () =>
      group && {
        manager: group.manager.id === user.id,
        director: group.directors.some(_ => _.id === user.id),
        roles: group.roles
          .map(({ members, ...role }) =>
            members.some(_ => _.id === user.id) ? role : (null as any)
          )
          .filter(Boolean),
      },
    [group, user.id]
  );

  const setGroupId = useCallback(
    (e: React.ChangeEvent<any>) =>
      history.push(
        generatePath(GROUP_ROUTE_PATH, {
          groupId: e.target.value,
          groupPage: defaultGroupPage,
        })
      ),
    [history]
  );

  const setGroupPage = useCallback(
    (_, value: string) =>
      history.push(generatePath(GROUP_ROUTE_PATH, { groupId, groupPage: value })),
    [history, groupId]
  );

  return (
    <>
      <AppBar position="static">
        <Toolbar className={classes.toolbar}>
          <Link to="/" className={classes.noLineHeight}>
            <img src={smallLogo} className={classes.appLogo} alt="Orchestrate" />
          </Link>
          <Typography>Group:</Typography>
          <Select
            className={classes.groupSelect}
            value={(groups && groupId) || ""}
            onChange={setGroupId}
            placeholder="Select group..."
            disabled={groups?.length === 0}
          >
            {!groups && <MenuItem disabled>Loading Groups...</MenuItem>}
            {groups?.map(({ id, name }) => (
              <MenuItem key={id} value={id}>
                {name}
              </MenuItem>
            ))}
          </Select>
          <div className={classes.spacer} />
          <Tabs
            className={classes.groupTabs}
            disabled={!groupId}
            value={groupPage ?? false}
            onChange={setGroupPage}
            indicatorColor="secondary"
          >
            {groupPages.map(_ => (
              <Tab disableRipple key={_.route} value={_.route} label={_.name} />
            ))}
          </Tabs>
          <div className={classes.spacer} />
          <Typography className={classes.username}>
            {user.firstName} {user.lastName}
          </Typography>
          <IconButton color="inherit">
            <Badge
              badgeContent={10}
              overlap="circle"
              color="secondary"
              anchorOrigin={{ vertical: "top", horizontal: "right" }}
            >
              <NotificationsIcon />
            </Badge>
          </IconButton>
          <Tooltip title="Log out">
            <IconButton edge="end" onClick={onLogout} color="inherit">
              <ExitToAppIcon />
            </IconButton>
          </Tooltip>
        </Toolbar>
      </AppBar>
      <div className={classes.pageContent}>
        <Page user={user} groups={groups} group={group} userInfo={userInfo} setGroup={setGroup} />
      </div>
    </>
  );
}
