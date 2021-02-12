import React, { useMemo } from "react";

import Autocomplete from "@material-ui/lab/Autocomplete";
import ListItemAvatar from "@material-ui/core/ListItemAvatar";
import ListItemText from "@material-ui/core/ListItemText";

import { ListInput } from "./ListInput";
import { UserAvatar } from "../UserAvatar";
import { textAutocompleteOptions } from "../textAutocompleteOptions";

interface Props {
  users: orch.UserData[] | undefined;
  selectedUsers: orch.UserData[] | undefined;
  onAdded: (user: orch.UserData) => Promise<any>;
}

export function UsersListInput({ users, selectedUsers, onAdded }: Props) {
  const remainingUsers = useMemo(
    () => (!users || !selectedUsers ? users : users.filter(u => !selectedUsers.includes(u))),
    [users, selectedUsers]
  );

  return (
    <ListInput items={selectedUsers} onAdded={onAdded} getListItem={getUserListItem}>
      {({ onAdded }) => (
        <Autocomplete
          {...textAutocompleteOptions(remainingUsers)}
          value={null}
          onChange={(_, item) => onAdded(item!)}
        />
      )}
    </ListInput>
  );
}

function getUserListItem(user: orch.UserData) {
  return (
    <>
      <ListItemAvatar key={user.id}>
        <UserAvatar user={user} />
      </ListItemAvatar>
      <ListItemText primary={`${user.firstName} ${user.lastName}`} />
    </>
  );
}
