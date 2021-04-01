import React, { useCallback, useState } from "react";

import { makeStyles } from "@material-ui/core/styles";
import Fab from "@material-ui/core/Fab";
import SendIcon from "@material-ui/icons/Send";

import { MultilineInput } from "utils/components";

const useStyles = makeStyles({
  newCommentContainer: {
    display: "flex",
    alignItems: "center",
    "& > :not(:last-child)": { marginRight: "2rem" },
  },
  sendCommentButton: { flex: "0 0 auto" },
});

interface Props {
  onAdd: (value: string) => Promise<any>;
  maxLength: number;
  maxLineBreaks: number;
}

const DEFAULT_VALUE: [string, boolean] = ["", false];

export default function AddNewComment({ onAdd, maxLength, maxLineBreaks }: Props) {
  const classes = useStyles();
  const [[text, isValid], setValue] = useState<[string, boolean]>(DEFAULT_VALUE);

  const handleInputChange = useCallback(
    (e: React.ChangeEvent<HTMLTextAreaElement>, isValid: boolean) =>
      setValue([e.target.value, isValid]),
    []
  );

  const handleFabClick = useCallback(
    () => isValid && onAdd(text).then(() => setValue(DEFAULT_VALUE)),
    [onAdd, text, isValid]
  );

  return (
    <div className={classes.newCommentContainer}>
      <MultilineInput
        label="Add comment"
        variant="outlined"
        rows={3}
        rowsMax={5}
        value={text}
        onChange={handleInputChange}
        maxLength={maxLength}
        maxLineBreaks={maxLineBreaks}
      />
      <Fab
        disabled={!text || !isValid}
        color="primary"
        size="medium"
        className={classes.sendCommentButton}
        onClick={handleFabClick}
      >
        <SendIcon />
      </Fab>
    </div>
  );
}
