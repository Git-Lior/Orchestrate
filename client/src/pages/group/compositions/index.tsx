import React, { useState, useEffect, useCallback } from "react";
import { Route, Switch, useHistory, useRouteMatch } from "react-router-dom";

import { MOCK_COMPOSITIONS } from "./mock";
import CompositionsPanel from "./Compositions";

type Props = orch.group.PageProps;

export default function GroupCompositionsPage({ user, userInfo }: Props) {
  const history = useHistory();
  const { path, url } = useRouteMatch();

  const [compositions, setCompositions] = useState<orch.Composition[] | null>(null);
  const [activeComposition, setActiveComposition] = useState<orch.Composition | null>(null);
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

  useEffect(() => {
    if (activeComposition) history.push(`${url}/${activeComposition.id}`);
    else history.push(`${url}`);
  }, [activeComposition]);

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
            onCompositionSelected={setActiveComposition}
          />
        }
      />
      <Route exact path={`${path}/:compositionId`} children={null} />
      <Route path={`${path}/:compositionId/:roleId`} children={null} />
    </Switch>
  );
}
