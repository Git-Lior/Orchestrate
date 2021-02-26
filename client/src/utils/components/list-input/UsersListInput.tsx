import React, { useCallback } from "react";

import ListItemAvatar from "@material-ui/core/ListItemAvatar";
import ListItemText from "@material-ui/core/ListItemText";

import { useInputState } from "utils/hooks";

import { ListInput } from "./ListInput";
import { getUserName, UserAvatar } from "../UserAvatar";
import { AsyncAutocomplete } from "../AsyncAutocomplete";

interface Props {
  users: orch.UserData[] | undefined;
  optionsProvider: (text: string) => Promise<orch.UserData[]>;
  disabled?: boolean;
  onAdded: (user: orch.UserData) => Promise<any>;
  onRemoved: (user: orch.UserData) => Promise<any>;
}

export function UsersListInput({ users, optionsProvider, ...listProps }: Props) {
  const [inputValue, setInputValue] = useInputState();

  const filteredOptionsProvider = useCallback(
    async text => {
      const options = await optionsProvider(text);
      return !users || !options ? options : options.filter(u => !users.some(_ => _.id === u.id));
    },
    [optionsProvider, users]
  );

  return (
    <ListInput {...listProps} items={users} getListItem={getUserListItem}>
      {({ onAdded }) => (
        <AsyncAutocomplete
          optionsProvider={filteredOptionsProvider}
          getOptionLabel={getUserName}
          value={null}
          inputValue={inputValue}
          onInputChange={setInputValue as any}
          onChange={(_, item) => {
            if (item) {
              setInputValue();
              onAdded(item);
            }
          }}
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
