import React from "react";

import ListItemAvatar from "@material-ui/core/ListItemAvatar";
import ListItemText from "@material-ui/core/ListItemText";

import { ListInput } from "./ListInput";
import { UserAvatar } from "../UserAvatar";
import { userToText } from "utils/general";

interface Props {
  users: orch.UserData[] | undefined;
  optionsProvider: (text: string) => Promise<orch.UserData[]>;
  readonly?: boolean;
  elevation?: number;
  onAdded: (user: orch.UserData) => Promise<any>;
  onRemoved: (user: orch.UserData) => Promise<any>;
}

export function UsersListInput({ users, optionsProvider, ...listProps }: Props) {
  return (
    <ListInput
      {...listProps}
      items={users}
      optionsProvider={optionsProvider}
      getOptionLabel={userToText}
    >
      {user => (
        <>
          <ListItemAvatar key={user.id}>
            <UserAvatar user={user} />
          </ListItemAvatar>
          <ListItemText primary={userToText(user)} />
        </>
      )}
    </ListInput>
  );
}
