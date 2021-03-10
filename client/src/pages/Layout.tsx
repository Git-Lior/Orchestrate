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
import IconButton from "@material-ui/core/IconButton";
import Badge from "@material-ui/core/Badge";
import Menu from "@material-ui/core/Menu";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import ListItemText from "@material-ui/core/ListItemText";
import NotificationsIcon from "@material-ui/icons/Notifications";
import ExitToAppIcon from "@material-ui/icons/ExitToApp";
import MoreVertIcon from "@material-ui/icons/MoreVert";
import LockIcon from "@material-ui/icons/Lock";
import TextField from "@material-ui/core/TextField";

import makeStyles from "@material-ui/core/styles/makeStyles";
import layoutStyles from "./Layout.styles";
import smallLogo from "assets/logo-small.png";

import { useApiFetch, useInputState, usePromiseStatus } from "utils/hooks";
import { groupPages, defaultGroupPage } from "./group";
import { userToText } from "utils/general";
import { FormDialog } from "utils/components";

const useStyles = makeStyles(layoutStyles);

interface Props {
  user: orch.User;
  onLogout: () => void;
  onPasswordChange: (oldPassword: string, newPassword: string) => Promise<any>;
  page: React.ComponentType<orch.PageProps>;
}

const GROUP_ROUTE_PATH = "/group/:groupId/:groupPage";

export default function Layout({ user, onLogout, onPasswordChange, page: Page }: Props) {
  const classes = useStyles();
  const apiFetch = useApiFetch(user, "/groups");
  const history = useHistory<orch.group.RouteParams>();

  const { groupId, groupPage } = useParams<orch.group.RouteParams>();
  const [groups, setGroups] = useState<orch.GroupData[]>();
  const [group, setGroup] = useState<orch.Group>();
  const [menuAnchor, setMenuAnchor] = useState<Element>();
  const [changePasswordDialogOpen, setChangePasswordDialogOpen] = useState(false);

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

  const handleMenuClick = useCallback(
    (e: React.MouseEvent<any>) => setMenuAnchor(e.currentTarget),
    []
  );
  const handleMenuClose = useCallback(() => setMenuAnchor(undefined), []);

  const showChangePasswordDialog = useCallback(() => {
    setChangePasswordDialogOpen(true);
    setMenuAnchor(undefined);
  }, []);

  const hideChangePasswordDialog = useCallback(() => setChangePasswordDialogOpen(false), []);

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
              <Tab
                disabled={!userInfo || !_.isEnabled(userInfo)}
                disableRipple
                key={_.route}
                value={_.route}
                label={_.name}
              />
            ))}
          </Tabs>
          <div className={classes.spacer} />
          <Typography className={classes.username}>{userToText(user)}</Typography>
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
          <IconButton onClick={handleMenuClick}>
            <MoreVertIcon color="secondary" />
          </IconButton>
          <Menu anchorEl={menuAnchor} open={!!menuAnchor} onClose={handleMenuClose}>
            <MenuItem onClick={showChangePasswordDialog}>
              <ListItemIcon>
                <LockIcon fontSize="small" />
              </ListItemIcon>
              <ListItemText primary="Change Password" />
            </MenuItem>
            <MenuItem onClick={onLogout}>
              <ListItemIcon>
                <ExitToAppIcon fontSize="small" />
              </ListItemIcon>
              <ListItemText primary="Log Out" />
            </MenuItem>
          </Menu>
        </Toolbar>
      </AppBar>
      <PasswordDialog
        open={changePasswordDialogOpen}
        onClose={hideChangePasswordDialog}
        onPasswordChange={onPasswordChange}
      />
      <div className={classes.pageContent}>
        <Page user={user} groups={groups} group={group} userInfo={userInfo} setGroup={setGroup} />
      </div>
    </>
  );
}

interface PasswordDialogProps {
  open: boolean;
  onPasswordChange: (oldPassword: string, newPassword: string) => Promise<any>;
  onClose: () => void;
}

const usePasswordDialogStyles = makeStyles({
  formRow: { display: "flex", marginBottom: "1em" },
  formRowLabel: { marginRight: "10px", fontSize: "18px" },
});

function PasswordDialog({ open, onPasswordChange, onClose }: PasswordDialogProps) {
  const classes = usePasswordDialogStyles();

  const [oldPassword, setOldPassword] = useInputState();
  const [newPassword, setNewPassword] = useInputState();
  const [passwordLoading, passwordError, setPasswordLoading] = usePromiseStatus();

  const changePassword = useCallback(
    () =>
      setPasswordLoading(
        onPasswordChange(oldPassword, newPassword).then(() => {
          setOldPassword();
          setNewPassword();
          onClose();
        })
      ),
    [
      setPasswordLoading,
      onPasswordChange,
      oldPassword,
      newPassword,
      setOldPassword,
      setNewPassword,
      onClose,
    ]
  );

  return (
    <FormDialog
      title="Change Password"
      open={open}
      onClose={onClose}
      onDone={changePassword}
      loading={passwordLoading}
      error={passwordError}
    >
      <div className={classes.formRow}>
        <Typography variant="body1" className={classes.formRowLabel}>
          Old password
        </Typography>
        <TextField type="password" value={oldPassword} onChange={setOldPassword} />
      </div>
      <div className={classes.formRow}>
        <Typography variant="body1" className={classes.formRowLabel}>
          New password
        </Typography>
        <TextField type="password" value={newPassword} onChange={setNewPassword} />
      </div>
    </FormDialog>
  );
}
