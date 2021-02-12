import React from "react";

import Avatar from "@material-ui/core/Avatar";
import makeStyles from "@material-ui/core/styles/makeStyles";
import { AppTheme } from "AppTheme";

const useStyles = makeStyles<AppTheme, Props>(({ palette: { primary, background } }) => ({
  avatar: props => ({
    backgroundColor: props.invertColors ? background.default : primary.main,
    color: props.invertColors ? primary.main : background.default,
  }),
}));

interface Props {
  user: orch.UserData;
  invertColors?: boolean;
}

export function UserAvatar(props: Props) {
  const classes = useStyles(props);

  const { firstName, lastName } = props.user;

  return (
    <Avatar className={classes.avatar}>
      {firstName[0].toUpperCase()}
      {lastName[0].toUpperCase()}
    </Avatar>
  );
}
