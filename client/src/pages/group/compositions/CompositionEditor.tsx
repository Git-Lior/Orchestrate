import React, { useCallback, useEffect, useState } from "react";

import makeStyles from "@material-ui/core/styles/makeStyles";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import TextField from "@material-ui/core/TextField";
import Autocomplete from "@material-ui/lab/Autocomplete";

import compositionEditorStyles from "./CompositionEditor.styles";
import { useInputState } from "utils/hooks";
import { MOCK_GENRES, MOCK_USER_INFO } from "mocks";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemText from "@material-ui/core/ListItemText";

export type EditedComposition = orch.CompositionData & { id?: number };

const useStyles = makeStyles(compositionEditorStyles);

interface Props {
  initialData: EditedComposition;
  onCancel: () => void;
  onSubmit: (data: EditedComposition) => void;
}

export default function CompositionEditor({ initialData, onSubmit, onCancel }: Props) {
  const classes = useStyles();

  const [genres, setGenres] = useState<string[]>();

  const [title, setTitle] = useInputState(initialData.title);
  const [composer, setComposer] = useInputState(initialData.composer);
  const [genre, setGenre] = useState<string | null>(initialData.genre);
  const [sheetMusic, setSheetMusic] = useState(initialData.sheetMusic);
  const [loadGroupRoles, setLoadGroupRoles] = useState(true);

  useEffect(() => {
    setGenres(MOCK_GENRES);
    /* apiFetch(...) */
    const groupRoles = MOCK_USER_INFO.roles;

    const sheetMusicMap: Record<number, string> = sheetMusic.reduce(
      (pv, v) => ({ ...pv, [v.role.id]: v.fileUrl }),
      {}
    );

    setSheetMusic(groupRoles.map(r => ({ role: r, fileUrl: sheetMusicMap[r.id] })));
    setLoadGroupRoles(false);
  }, []);

  const selectGenre = useCallback((_, value: string | null) => setGenre(value), [setGenre]);

  const onFilesSelected = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    // TODO
  }, []);

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
      <div className={classes.compositionInfo}>
        <div className={classes.formRow}>
          <Typography variant="body1" className={classes.formRowLabel}>
            Title
          </Typography>
          <TextField value={title} onChange={setTitle} />
        </div>
        <div className={classes.formRow}>
          <Typography variant="body1" className={classes.formRowLabel}>
            Composer
          </Typography>
          <TextField value={composer} onChange={setComposer} />
        </div>
        <div className={classes.formRow}>
          <Typography variant="body1" className={classes.formRowLabel}>
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
      <div className={classes.sheetMusic}>
        <Typography variant="h6">Sheet Music:</Typography>
        <List>
          {sheetMusic.map(sm => (
            <ListItem divider>
              <ListItemText>
                {sm.role.section} {sm.role.num}
              </ListItemText>
            </ListItem>
          ))}
        </List>
      </div>
      <div className={classes.actions}>
        <Button className={classes.doneButton} variant="outlined" color="primary" component="label">
          Upload Files
          <input type="file" hidden multiple accept="application/pdf" onChange={onFilesSelected} />
        </Button>
        <Button
          className={classes.doneButton}
          variant="contained"
          color="primary"
          onClick={onEditDone}
        >
          Done
        </Button>
      </div>
    </div>
  );
}
