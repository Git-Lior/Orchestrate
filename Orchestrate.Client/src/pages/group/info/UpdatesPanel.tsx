import React, { useEffect, useState } from "react";

import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemText from "@material-ui/core/ListItemText";
import CircularProgress from "@material-ui/core/CircularProgress";
import Divider from "@material-ui/core/Divider";

import { useApiFetch } from "utils/hooks";
import { getFullTimeText, userToText } from "utils/general";

interface Props {
  user: orch.User;
  group: orch.Group;
}

export default function UpdatesPanel({ user, group }: Props) {
  const [updates, setUpdates] = useState<orch.UpdateData[]>();

  const apiFetch = useApiFetch(user, `/groups/${group.id}/updates`);

  useEffect(() => {
    apiFetch("").then(setUpdates);
  }, [apiFetch]);

  return !updates ? (
    <CircularProgress />
  ) : (
    <List>
      {updates.map((u, i) => (
        <React.Fragment key={u.date}>
          {i > 0 && <Divider variant="middle" component="li" />}
          <ListItem>
            <ListItemText
              primary={
                _isConcertUpdate(u)
                  ? `${
                      u.attendance
                    } members updated their attendance to the concert on ${getFullTimeText(
                      u.concert.date
                    )} at ${u.concert.location}`
                  : `Director ${userToText(u.uploader)} created composition "${u.title}"`
              }
              secondary={getFullTimeText(u.date)}
            />
          </ListItem>
        </React.Fragment>
      ))}
    </List>
  );
}
function _isConcertUpdate(u: orch.UpdateData): u is orch.ConcertUpdateData {
  return !!(u as any).concert;
}
