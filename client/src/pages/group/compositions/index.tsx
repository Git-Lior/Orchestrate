import React, { useState, useEffect, useCallback } from "react";
import { Route, Switch, useHistory, useRouteMatch } from "react-router-dom";

import { MOCK_COMPOSITIONS } from "./mock";
import CompositionsPanel from "./CompositionsPanel";
import SheetMusicPanel from "./SheetMusicPanel";

type Props = orch.group.PageProps;

export default function GroupCompositionsPage({ user, userInfo }: Props) {
  const history = useHistory();
  const { path, url } = useRouteMatch();

  const [compositions, setCompositions] = useState<orch.Composition[] | null>(null);
  const [query, setQuery] = useState<orch.compositions.Query>({
    genre: "",
    title: "",
    onlyInConcert: false,
  });

  useEffect(() => {
    setTimeout(() => {
      setCompositions(MOCK_COMPOSITIONS);
    }, 1000);
  }, []);

  const setCompositionId = useCallback((c: orch.Composition) => history.push(`${url}/${c.id}`), [
    url,
  ]);

  if (!compositions) return <div>Loading compositions...</div>;

  return (
    <Switch>
      <Route
        exact
        path={`${path}/`}
        children={
          <CompositionsPanel
            initialQuery={query}
            onQueryChange={setQuery}
            compositions={compositions}
            onCompositionSelected={setCompositionId}
          />
        }
      />
      <Route
        exact
        path={`${path}/:compositionId/:?roleId`}
        children={<SheetMusicPanel user={user} userInfo={userInfo} />}
      />
    </Switch>
  );
}
