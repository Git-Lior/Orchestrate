import React, { useCallback, useEffect, useState } from "react";

import makeStyles from "@material-ui/core/styles/makeStyles";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import TextField from "@material-ui/core/TextField";
import Divider from "@material-ui/core/Divider";
import Autocomplete from "@material-ui/lab/Autocomplete";

import compositionEditorStyles from "./CompositionEditor.styles";
import { useInputState } from "utils/hooks";
import { MOCK_GENRES } from "mocks";

export type EditedComposition = orch.CompositionData & { id?: number };

const useStyles = makeStyles(compositionEditorStyles);

interface Props {
  initialData: EditedComposition;
  onCancel: () => void;
  onSubmit: (data: EditedComposition) => void;
}

export default function CompositionEditor({ initialData, onSubmit, onCancel }: Props) {
  const classes = useStyles();

  const [genres, setGenres] = useState<string[] | null>(null);

  const [title, setTitle] = useInputState(initialData.title);
  const [composer, setComposer] = useInputState(initialData.composer);
  const [genre, setGenre] = useState<string | null>(initialData.genre);
  const [sheetMusic, setSheetMusic] = useState(initialData.sheetMusic);

  useEffect(() => {
    setGenres(MOCK_GENRES);
  }, []);

  const selectGenre = useCallback((_, value: string | null) => setGenre(value), [setGenre]);

  const onEditDone = useCallback(() => {
    if (!genre) return;

    onSubmit({
      ...initialData,
      title,
      composer,
      genre,
      sheetMusic,
    });
  }, [initialData]);

  return (
    <div className={classes.container}>
      <IconButton aria-label="close" className={classes.cancelEdit} onClick={onCancel}>
        <CloseIcon />
      </IconButton>
      <Typography variant="h4" className={classes.title}>
        {!!initialData.id ? "Edit" : "Create"} Composition
      </Typography>
      <div className={classes.editorContainer}>
        <div className={classes.compositionInfo}>
          <div className={classes.formRow}>
            <Typography variant="h6" className={classes.formRowLabel}>
              Title
            </Typography>
            <TextField value={title} onChange={setTitle} />
          </div>
          <div className={classes.formRow}>
            <Typography variant="h6" className={classes.formRowLabel}>
              Composer
            </Typography>
            <TextField value={composer} onChange={setComposer} />
          </div>
          <div className={classes.formRow}>
            <Typography variant="h6" className={classes.formRowLabel}>
              Genre
            </Typography>
            <Autocomplete
              fullWidth
              freeSolo
              selectOnFocus
              clearOnBlur
              handleHomeEndKeys
              loading={!genres}
              loadingText="loading genres..."
              options={genres ?? []}
              value={genre}
              onChange={selectGenre}
              renderInput={params => <TextField {...params} />}
            />
          </div>
        </div>
        <Divider className={classes.editorDivider} orientation="vertical" flexItem />
        <div className={classes.sheetMusic}></div>
      </div>
      <Button
        className={classes.doneButton}
        variant="contained"
        color="primary"
        onClick={onEditDone}
      >
        Done
      </Button>
    </div>
  );
}
