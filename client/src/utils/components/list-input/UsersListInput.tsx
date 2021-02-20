import React, { useCallback, useEffect, useMemo, useState } from "react";
import { debounce } from "throttle-debounce";

import Autocomplete from "@material-ui/lab/Autocomplete";
import ListItemAvatar from "@material-ui/core/ListItemAvatar";
import ListItemText from "@material-ui/core/ListItemText";

import { useInputState } from "utils/hooks";

import { ListInput } from "./ListInput";
import { UserAvatar } from "../UserAvatar";
import { textAutocompleteOptions } from "../textAutocompleteOptions";

interface Props {
  users: orch.UserData[] | undefined;
  optionsProvider: () => Promise<orch.UserData[]>;
  disabled?: boolean;
  onAdded: (user: orch.UserData) => Promise<any>;
  onRemoved: (user: orch.UserData) => Promise<any>;
}

export function UsersListInput({ users, optionsProvider, ...listProps }: Props) {
  const [open, setOpen] = useState(false);
  const [inputValue, setInputValue] = useInputState();
  const [options, setOptions] = useState<orch.UserData[]>();

  const filteredOptions = useMemo(
    () => (!users || !options ? options : options.filter(u => !users.some(_ => _.id == u.id))),
    [users, options]
  );

  const debouncedOptionsProvider = useCallback(
    debounce(250, async (inputValue: string, optionsProvider: () => Promise<orch.UserData[]>) =>
      setOptions(await optionsProvider())
    ),
    []
  );

  useEffect(() => {
    if (open) debouncedOptionsProvider(inputValue, optionsProvider);
  }, [debouncedOptionsProvider, open, inputValue, optionsProvider]);

  return (
    <ListInput {...listProps} items={users} getListItem={getUserListItem}>
      {({ onAdded }) => (
        <Autocomplete
          {...textAutocompleteOptions(filteredOptions)}
          getOptionLabel={user => `${user.firstName} ${user.lastName}`}
          value={null}
          inputValue={inputValue}
          onInputChange={setInputValue as any}
          open={open}
          onOpen={() => setOpen(true)}
          onClose={() => setOpen(false)}
          onChange={(_, item) => {
            setInputValue();
            onAdded(item!);
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
