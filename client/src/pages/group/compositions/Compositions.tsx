import React, { useCallback, useEffect, useState } from "react";
import { debounce } from "throttle-debounce";

import makeStyles from "@material-ui/core/styles/makeStyles";
import Typography from "@material-ui/core/Typography";
import Card from "@material-ui/core/Card";
import Checkbox from "@material-ui/core/Checkbox";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Select from "@material-ui/core/Select";
import MenuItem from "@material-ui/core/MenuItem";
import TextField from "@material-ui/core/TextField";

import { useInputState } from "utils/hooks";
import compositionsStyles from "./Compositions.styles";
import { MOCK_GENRES } from "./mock";

import ResultsTable from "./CompositionsTable";

const useStyles = makeStyles(compositionsStyles);

interface Props {
  initialQuery: orch.compositions.Query;
  compositions: orch.Composition[];
  onCompositionSelected: (composition: orch.Composition) => void;
  onQueryChange: (query: orch.compositions.Query) => void;
}

export default function CompositionsPanel({
  initialQuery,
  compositions,
  onCompositionSelected,
  onQueryChange,
}: Props) {
  const classes = useStyles();

  const [onlyInConcert, setOnlyInConcert] = useState<boolean>(initialQuery.onlyInConcert);
  const [genres, setGenres] = useState<string[]>([]);
  const [genreFilter, setGenreFilter] = useInputState(initialQuery.genre);
  const [titleFilter, setTitleFilter] = useInputState(initialQuery.title);

  const onlyInConcertChange = useCallback((_, checked: boolean) => setOnlyInConcert(checked), [
    setOnlyInConcert,
  ]);

  useEffect(() => {
    setTimeout(() => setGenres(MOCK_GENRES), 1000);
  }, []);

  useEffect(
    debounce(250, () => {
      onQueryChange({ genre: genreFilter, title: titleFilter, onlyInConcert });
    }),
    [genreFilter, titleFilter, onlyInConcert]
  );

  return (
    <div>
      <Typography variant="h4" className={classes.panelTitle}>
        Compositions
      </Typography>
      <div className={classes.panelsContainer}>
        <Card className={classes.filtersPanel}>
          <Typography variant="h5">Filter results</Typography>

          <FormControlLabel
            control={
              <Checkbox
                disableRipple
                color="primary"
                checked={onlyInConcert}
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
            <Select value={genreFilter} onChange={setGenreFilter as any} fullWidth>
              {genres.map(v => (
                <MenuItem key={v} value={v}>
                  {v}
                </MenuItem>
              ))}
            </Select>
          </div>
        </Card>
        <Card className={classes.resultsPanel}>
          <ResultsTable items={compositions} onSelected={onCompositionSelected} />
        </Card>
      </div>
    </div>
  );
}
