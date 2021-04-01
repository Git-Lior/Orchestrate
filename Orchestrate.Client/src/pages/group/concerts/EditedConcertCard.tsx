import React, { useCallback, useState } from "react";
import classnames from "classnames";

import TextField from "@material-ui/core/TextField";
import Button from "@material-ui/core/Button";
import SaveIcon from "@material-ui/icons/Save";
import CloseIcon from "@material-ui/icons/Close";

import { makeStyles } from "@material-ui/core/styles";
import { usePromiseStatus } from "utils/hooks";

import CardLayout from "./CardLayout";
import CardInfoInput from "./CardInfoInput";
import { MultilineInput } from "utils/components";

const useStyles = makeStyles({
  cardActions: {
    position: "absolute",
    bottom: "2rem",
    right: "2rem",
    "& > :not(:last-child)": { marginRight: "3rem" },
  },
  button: {
    transition: "opacity 0.2s ease-in",
  },
  buttonDisabled: {
    opacity: 0.6,
    pointerEvents: "none",
  },
});

interface Props {
  concert?: orch.Concert;
  onEditDone: (concert: orch.OptionalId<orch.ConcertData>) => Promise<any>;
  onEditCancel: () => void;
}

export function EditedConcertCard({ concert, onEditDone, onEditCancel }: Props) {
  const classes = useStyles();

  const [loading, error, setPromise] = usePromiseStatus();
  const [[description, isDescriptionValid], setDescription] = useState<[string, boolean]>([
    concert?.description ?? "",
    !!concert?.description,
  ]);
  const [updatedData, setUpdatedData] = useState<orch.OptionalId<orch.ConcertData>>(
    concert ?? ({} as any)
  );

  const onDescriptionChange = useCallback(
    (e: React.ChangeEvent<HTMLTextAreaElement>, isValid: boolean) => {
      setDescription([e.target.value, isValid]);
    },
    [updatedData]
  );

  const doneHandler = useCallback(() => {
    if (updatedData.date && updatedData.location && isDescriptionValid)
      setPromise(onEditDone({ id: concert?.id, ...updatedData, description }));
  }, [concert, updatedData, description, isDescriptionValid, setPromise, onEditDone]);

  return (
    <CardLayout
      left={<CardInfoInput concert={concert} onDataUpdated={setUpdatedData} />}
      center={
        <MultilineInput
          label="Description"
          rows={2}
          value={description}
          onChange={onDescriptionChange}
          maxLength={100}
          maxLineBreaks={1}
        />
      }
    >
      <div className={classes.cardActions}>
        <Button
          variant="contained"
          color="primary"
          startIcon={<SaveIcon />}
          className={classnames(classes.button, {
            [classes.buttonDisabled]:
              !updatedData.date || !updatedData.location || !isDescriptionValid,
          })}
          onClick={doneHandler}
        >
          Save
        </Button>
        <Button
          variant="contained"
          startIcon={<CloseIcon />}
          className={classes.button}
          onClick={onEditCancel}
        >
          Cancel
        </Button>
      </div>
    </CardLayout>
  );
}
