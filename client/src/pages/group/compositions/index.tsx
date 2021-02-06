import React, { useState, useEffect, useCallback } from "react";
import { Route, Switch, useHistory, useRouteMatch } from "react-router-dom";

import { MOCK_COMPOSITIONS } from "mocks";
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

  const editComposition = useCallback(
    (c: orch.Composition) => history.push(`${url}/${c.id}/edit`),
    [url]
  );

  const deleteComposition = useCallback((c: orch.Composition) => {
    if (!window.confirm("Are you sure you want to delete this composition?")) return;
  }, []);

  return (
    <>
      <Switch>
        <Route
          exact
          path={`${path}/`}
          children={
            <CompositionsPanel
              isDirector={userInfo.director}
              initialQuery={query}
              onQueryChange={setQuery}
              compositions={compositions}
              onCompositionSelect={setCompositionId}
              onCompositionEdit={editComposition}
              onCompositionDelete={deleteComposition}
            />
          }
        />
        <Route exact path={`${path}/:compositionId/edit`} children={null} />
        <Route exact path={`${path}/create`} children={null} />
        <Route
          exact
          path={[`${path}/:compositionId/`, `${path}/:compositionId/:roleId`]}
          children={<SheetMusicPanel user={user} userInfo={userInfo} />}
        />
      </Switch>
    </>
  );
}
