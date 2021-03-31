import React, { useCallback } from "react";

import { UsersListInput } from "utils/components";
import { useApiFetch } from "utils/hooks";

interface Props extends Required<orch.PageProps> {
  getAllUsers: () => Promise<orch.UserData[]>;
}

export default function DirectorsPanel({ user, group, userInfo, setGroup, getAllUsers }: Props) {
  const apiFetch = useApiFetch(user, `/groups/${group.id}/directors`);

  const addDirector = useCallback(
    (director: orch.UserData) => {
      return apiFetch("", { method: "POST", body: JSON.stringify(director.id) }, "none").then(
        () => {
          setGroup(group => ({
            ...group!,
            directors: [...group!.directors, director],
          }));
        }
      );
    },
    [setGroup, apiFetch]
  );

  const removeDirector = useCallback(
    (director: orch.UserData) => {
      return apiFetch(director.id.toString(), { method: "DELETE" }, "none").then(() => {
        setGroup(group => ({
          ...group!,
          directors: group!.directors.filter(_ => _.id !== director.id),
        }));
      });
    },
    [setGroup, apiFetch]
  );

  return (
    <UsersListInput
      readonly={!userInfo.manager}
      users={group.directors}
      optionsProvider={getAllUsers}
      onAdded={addDirector}
      onRemoved={removeDirector}
    />
  );
}
