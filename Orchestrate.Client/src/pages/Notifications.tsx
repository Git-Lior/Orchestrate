import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import moment from "moment";

import { makeStyles } from "@material-ui/core/styles";
import List from "@material-ui/core/List";
import Badge from "@material-ui/core/Badge";
import Popover from "@material-ui/core/Popover";
import IconButton from "@material-ui/core/IconButton";
import ListItemText from "@material-ui/core/ListItemText";
import NotificationsIcon from "@material-ui/icons/Notifications";
import ListItem from "@material-ui/core/ListItem";
import Typography from "@material-ui/core/Typography";

import { roleToText } from "utils/general";
import { useApiFetch, useAutoRefresh, useLocalStorage } from "utils/hooks";

const useStyles = makeStyles({
  popoverContent: { maxWidth: "30rem" },
  newNotification: { backgroundColor: "yellow" },
});

interface Props {
  user: orch.User;
}

export default function Notificatons({ user }: Props) {
  const classes = useStyles();

  const storageKey = useMemo(() => `${user.id}_notifications_last_update`, [user]);

  const apiFetch = useApiFetch(user, "/notifications");
  const iconRef = useRef<HTMLButtonElement>(null);
  const [menuOpen, setMenuOpen] = useState(false);
  const [lastUpdate, setLastUpdate] = useLocalStorage(storageKey);
  const [notifications, loading, error] = useAutoRefresh<orch.NotificationData[]>(() =>
    apiFetch("?lastUpdate=" + (lastUpdate ?? ""))
  );

  const newNotificationsIndex: number = useMemo(() => {
    if (!notifications) return 0;
    if (!lastUpdate) return notifications.length;

    const lastUpdateDate = moment(lastUpdate);
    const index = notifications.findIndex(_ => moment.unix(_.date).isBefore(lastUpdateDate));
    return index === -1 ? notifications.length : index;
  }, [lastUpdate, notifications]);

  useEffect(() => {
    return () => {
      if (menuOpen) localStorage.setItem(storageKey, new Date().toISOString());
    };
  }, []);

  const openMenu = useCallback(() => {
    setMenuOpen(true);
  }, []);

  const closeMenu = useCallback(() => {
    setLastUpdate(new Date().toISOString());
    setMenuOpen(false);
  }, [setLastUpdate]);

  const handleNotificationClick = useCallback((n: orch.NotificationData) => {}, []);

  return (
    <>
      <IconButton color="inherit" ref={iconRef} onClick={menuOpen ? closeMenu : openMenu}>
        <Badge
          badgeContent={newNotificationsIndex > 0 ? newNotificationsIndex : undefined}
          overlap="circle"
          color="secondary"
          anchorOrigin={{ vertical: "top", horizontal: "right" }}
        >
          <NotificationsIcon />
        </Badge>
      </IconButton>
      {notifications && (
        <Popover
          anchorEl={iconRef.current}
          anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
          transformOrigin={{ vertical: "top", horizontal: "center" }}
          getContentAnchorEl={null}
          open={menuOpen}
          onClose={closeMenu}
          classes={{ paper: classes.popoverContent }}
        >
          {notifications.length === 0 ? (
            <Typography variant="body1">No notifications</Typography>
          ) : (
            <List>
              {notifications.map((n, i) => (
                <ListItem
                  key={n.date}
                  onClick={() => handleNotificationClick(n)}
                  className={i < newNotificationsIndex ? classes.newNotification : undefined}
                >
                  {_isConcertNotification(n) ? (
                    <ListItemText
                      primary={`you have a concert coming up at ${moment
                        .unix(n.concert.date)
                        .format("HH:mm")}, click for more info`}
                      secondary="today"
                      // {moment(new Date(n.date)).format("DD/MM/yyyy")}
                    />
                  ) : (
                    <ListItemText
                      primary={`${
                        n.comments === 1 ? "A new comment was" : `${n.comments} new comments were`
                      } added to part "${roleToText(n.role)}" in composition "${
                        n.composition.title
                      }"`}
                      secondary={moment.unix(n.date).fromNow(true)}
                    />
                  )}
                </ListItem>
              ))}
            </List>
          )}
        </Popover>
      )}
    </>
  );
}

function _isConcertNotification(n: orch.NotificationData): n is orch.ConcertNotificationData {
  return !!(n as any).concert;
}
