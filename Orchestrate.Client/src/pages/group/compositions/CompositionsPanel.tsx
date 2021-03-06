import React, { useCallback, useEffect, useMemo, useState } from "react";
import { debounce } from "throttle-debounce";

import makeStyles from "@material-ui/core/styles/makeStyles";
import Typography from "@material-ui/core/Typography";
import Card from "@material-ui/core/Card";
import Checkbox from "@material-ui/core/Checkbox";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import TextField from "@material-ui/core/TextField";

import { useApiFetch, useCRUDApi, useInputState } from "utils/hooks";
import compositionsPanelStyles from "./CompositionsPanel.styles";

import {
  AsyncAutocomplete,
  AutocompleteDialogRow,
  ColDef,
  EditableTable,
  TextDialogRow,
} from "utils/components";
import { userToText } from "utils/general";

const COLUMNS: ColDef[] = [
  { field: "title", headerName: "Title", flex: 3 },
  { field: "composer", headerName: "Composer", flex: 1 },
  { field: "genre", headerName: "Genre", flex: 1 },
  {
    field: "uploader",
    headerName: "Uploaded by",
    flex: 1,
    valueGetter: _ => userToText(_.row.uploader),
  },
];

const NEW_COMPOSITON: orch.CompositionData = {
  title: "",
  composer: "",
  genre: "",
} as any;

const useStyles = makeStyles(compositionsPanelStyles);

interface Props {
  user: orch.User;
  group: orch.Group;
  userInfo: orch.group.UserInfo;
  initialQuery: orch.compositions.Query;
  onQueryChanged: (query: orch.compositions.Query) => void;
  onCompositionSelected: (composition: orch.CompositionData) => void;
}

export default function CompositionsPanel({
  user,
  group,
  userInfo,
  initialQuery,
  onQueryChanged,
  onCompositionSelected,
}: Props) {
  const classes = useStyles();

  const apiFetch = useApiFetch(user, `/groups/${group.id}/compositions`);
  const [onlyInUpcomingConcert, setOnlyInConcert] = useState<boolean>(!!initialQuery.onlyInConcert);
  const [genreFilter, setGenreFilter] = useState(initialQuery.genre);
  const [titleFilter, setTitleFilter] = useInputState(initialQuery.title);

  const query: orch.compositions.Query = useMemo(
    () => ({ genre: genreFilter, title: titleFilter, onlyInUpcomingConcert }),
    [genreFilter, titleFilter, onlyInUpcomingConcert]
  );

  const [
    compositions,
    _refreshComposition,
    changeComposition,
    removeComposition,
    setCompositionsQuery,
  ] = useCRUDApi<orch.CompositionData>(
    user.token,
    `/groups/${group.id}/compositions`,
    initialQuery
  );

  const onlyInConcertChange = useCallback((_, checked: boolean) => setOnlyInConcert(checked), [
    setOnlyInConcert,
  ]);

  const getGenres: (text: string) => Promise<string[]> = useCallback(
    text => apiFetch("genres" + (!text ? "" : "?query=" + text)),
    [apiFetch]
  );

  const debouncedGetConcerts = useMemo(() => debounce(250, setCompositionsQuery), [
    setCompositionsQuery,
  ]);

  useEffect(() => {
    onQueryChanged(query);
    debouncedGetConcerts(query);
  }, [debouncedGetConcerts, query, onQueryChanged]);

  return (
    <div className={classes.panelsContainer}>
      <Card className={classes.filtersPanel}>
        <Typography variant="h5" className={classes.filtersTitle}>
          Filter results
        </Typography>
        <FormControlLabel
          control={
            <Checkbox
              disableRipple
              color="primary"
              checked={onlyInUpcomingConcert}
              onChange={onlyInConcertChange}
            />
          }
          label="Show only compositions in upcoming concerts"
        />
        <div className={classes.filterRow}>
          <Typography>Title: </Typography>
          <TextField value={titleFilter} onChange={setTitleFilter} />
        </div>
        <div className={classes.filterRow}>
          <Typography>Genre: </Typography>
          <AsyncAutocomplete
            optionsProvider={getGenres}
            onChange={(_, value) => setGenreFilter(value ?? undefined)}
          />
        </div>
      </Card>
      <div className={classes.compositionsTable}>
        <EditableTable
          rowTypeName="Composition"
          disableActions={!userInfo.director}
          columns={COLUMNS}
          rows={compositions}
          emptyRow={NEW_COMPOSITON}
          onRowChange={changeComposition}
          onRowDelete={removeComposition}
          onRowClick={onCompositionSelected}
        >
          {rowProps => (
            <>
              <TextDialogRow fieldKey="title" label="Title" {...rowProps} />
              <TextDialogRow fieldKey="composer" label="Composer" {...rowProps} />
              <AutocompleteDialogRow
                fieldKey="genre"
                label="Genre"
                freeSolo
                optionsProvider={getGenres}
                {...rowProps}
              />
            </>
          )}
        </EditableTable>
      </div>
    </div>
  );
}
