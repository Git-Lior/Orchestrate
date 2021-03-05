import React from "react";

import Avatar from "@material-ui/core/Avatar";
import makeStyles from "@material-ui/core/styles/makeStyles";
import { AppTheme } from "AppTheme";

const useStyles = makeStyles<AppTheme, Props>(({ palette }) => {
  const bgColor = palette.background.default;
  const textColor = ({ color }: Props) => palette[color ?? "primary"].main;

  return {
    avatar: props => ({
      backgroundColor: props.invertColors ? bgColor : textColor(props),
      color: props.invertColors ? textColor(props) : bgColor,
    }),
  };
});

type ColorsWithMain<T> = {
  [K in keyof T]: T[K] extends { main: string } ? K : never;
}[keyof T];

interface Props {
  user: orch.UserData;
  color?: ColorsWithMain<AppTheme["palette"]>;
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
