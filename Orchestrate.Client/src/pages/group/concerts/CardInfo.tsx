import React from "react";
import moment from "moment";

import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import PlaceIcon from "@material-ui/icons/Place";
import EventIcon from "@material-ui/icons/Event";
import ScheduleIcon from "@material-ui/icons/Schedule";

import { getTimeText } from "utils/general";
import ListItemText from "@material-ui/core/ListItemText";

interface Props {
  concert: orch.Concert;
}

export default function CardInfo({ concert }: Props) {
  return (
    <List>
      <ListItem>
        <ListItemIcon>
          <EventIcon />
        </ListItemIcon>
        <ListItemText primary={moment.unix(concert.date).format("dddd, DD/MM/YYYY")} />
      </ListItem>
      <ListItem>
        <ListItemIcon>
          <ScheduleIcon />
        </ListItemIcon>
        <ListItemText primary={getTimeText(concert.date)} />
      </ListItem>
      <ListItem>
        <ListItemIcon>
          <PlaceIcon />
        </ListItemIcon>
        <ListItemText primary={concert.location} />
      </ListItem>
    </List>
  );
}
