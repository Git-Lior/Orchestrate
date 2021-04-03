import React, { useCallback, useState } from "react";
import classnames from "classnames";

import { makeStyles } from "@material-ui/core/styles";
import Button from "@material-ui/core/Button";
import SaveIcon from "@material-ui/icons/Save";
import CloseIcon from "@material-ui/icons/Close";

import { usePromiseStatus } from "utils/hooks";

import CardLayout from "./CardLayout";
import CardInfoInput from "./CardInfoInput";
import { MultilineInput } from "utils/components";

const useStyles = makeStyles({
  cardActions: {
    display: "flex",
    flexDirection: "column",
    justifyContent: "space-evenly",
    height: "100%",
  },
  saveButton: {
    transition: "opacity 0.2s ease-in",
  },
  buttonDisabled: {
    opacity: 0.6,
    pointerEvents: "none",
  },
});

interface Props {
  concert?: orch.ConcertData;
  onEditDone: (concert: orch.OptionalId<orch.ConcertData>) => Promise<any>;
  onEditCancel: () => void;
}

export function EditedConcertCard({ concert, onEditDone, onEditCancel }: Props) {
  const classes = useStyles();

  const [loading, error, setPromise] = usePromiseStatus();
  const [[description, isDescriptionValid], setDescription] = useState<[string, boolean]>([
    concert?.description ?? "",
    true,
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
      right={
        <div className={classes.cardActions}>
          <Button variant="outlined" size="small" startIcon={<CloseIcon />} onClick={onEditCancel}>
            Cancel
          </Button>
          <Button
            variant="contained"
            color="primary"
            size="small"
            startIcon={<SaveIcon />}
            className={classnames(classes.saveButton, {
              [classes.buttonDisabled]:
                !updatedData.date || !updatedData.location || !isDescriptionValid,
            })}
            onClick={doneHandler}
          >
            Save
          </Button>
        </div>
      }
    />
  );
}
