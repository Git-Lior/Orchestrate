import React, { useState, useCallback } from "react";
import { Route, Switch, useHistory, useRouteMatch } from "react-router-dom";

import CompositionsPanel from "./CompositionsPanel";
import SheetMusicPanel from "./SheetMusicPanel";

type Props = Required<orch.PageProps>;

export default function GroupCompositionsPage({ user, userInfo, group }: Props) {
  const history = useHistory();
  const { path, url } = useRouteMatch();

  const [query, setQuery] = useState<orch.compositions.Query>({});

  const setCompositionId = useCallback(
    (c: orch.CompositionData) => history.push(`${url}/${c.id}`),
    [url, history]
  );

  return (
    <Switch>
      <Route
        exact
        path={`${path}/`}
        children={
          <CompositionsPanel
            user={user}
            group={group}
            userInfo={userInfo}
            initialQuery={query}
            onQueryChanged={setQuery}
            onCompositionSelected={setCompositionId}
          />
        }
      />
      <Route
        exact
        path={[`${path}/:compositionId/`, `${path}/:compositionId/:roleId`]}
        children={<SheetMusicPanel user={user} group={group} userInfo={userInfo} />}
      />
    </Switch>
  );
}
