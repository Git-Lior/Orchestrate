import React, { useState, useEffect, useCallback } from "react";
import { Route, Switch, useHistory, useRouteMatch } from "react-router-dom";

import Dialog from "@material-ui/core/Dialog";

import { MOCK_COMPOSITIONS } from "mocks";
import CompositionsPanel from "./CompositionsPanel";
import SheetMusicPanel from "./SheetMusicPanel";
import CompositionEditor, { EditedComposition } from "./CompositionEditor";

const EMPTY_COMPOSITION: EditedComposition = {
  title: "",
  genre: "",
  composer: "",
  sheetMusic: [],
};

type Props = Required<orch.PageProps>;

export default function GroupCompositionsPage({ user, userInfo }: Props) {
  const history = useHistory();
  const { path, url } = useRouteMatch();

  const [compositions, setCompositions] = useState<orch.Composition[] | null>(null);
  const [query, setQuery] = useState<orch.compositions.Query>({
    genre: "",
    title: "",
    onlyInConcert: false,
  });
  const [editedComposition, setEditedComposition] = useState<EditedComposition | null>(null);

  useEffect(() => {
    setTimeout(() => {
      setCompositions(MOCK_COMPOSITIONS);
    }, 1000);
  }, []);

  const setCompositionId = useCallback((c: orch.Composition) => history.push(`${url}/${c.id}`), [
    url,
  ]);

  const deleteComposition = useCallback((c: orch.Composition) => {
    if (!window.confirm("Are you sure you want to delete this composition?")) return;
  }, []);

  const createComposition = useCallback(() => setEditedComposition(EMPTY_COMPOSITION), []);

  const editComposition = useCallback((c: orch.Composition) => setEditedComposition(c), [url]);

  const cancelEdit = useCallback(() => {
    if (!window.confirm("Are you sure you want to cancel?")) return;
    setEditedComposition(null);
  }, [editedComposition]);

  const finishEdit = useCallback((c: EditedComposition) => {
    /* TODO: send to server */
    setEditedComposition(null);
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
              compositions={compositions}
              onQueryChange={setQuery}
              onAddComposition={createComposition}
              onCompositionSelect={setCompositionId}
              onCompositionEdit={editComposition}
              onCompositionDelete={deleteComposition}
            />
          }
        />
        <Route
          exact
          path={[`${path}/:compositionId/`, `${path}/:compositionId/:roleId`]}
          children={<SheetMusicPanel user={user} userInfo={userInfo} />}
        />
      </Switch>
      <Dialog open={!!editedComposition}>
        {!editedComposition ? (
          <div />
        ) : (
          <CompositionEditor
            initialData={editedComposition}
            onCancel={cancelEdit}
            onSubmit={finishEdit}
          />
        )}
      </Dialog>
    </>
  );
}
