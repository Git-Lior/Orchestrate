import React from "react";

import { makeStyles } from "@material-ui/core/styles";
import Dialog from "@material-ui/core/Dialog";
import IconButton from "@material-ui/core/IconButton";
import CloseIcon from "@material-ui/icons/Close";
import Typography, { TypographyProps } from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import CircularProgress from "@material-ui/core/CircularProgress";

const useStyles = makeStyles(theme => ({
  dialogContainer: {
    position: "relative",
    backgroundColor: theme.palette.background.default,
    borderRadius: 5,
    boxShadow: "0 0 20px",
    padding: "4em",
  },
  cancelEdit: { position: "absolute", left: 0, top: 0 },
  dialogTitle: { marginBottom: "1em", textAlign: "center" },
  doneButton: {},
  dialogStatus: {
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    overflow: "hidden",
  },
  dialogError: { width: "100%", maxHeight: 125 },
}));

interface Props {
  open: boolean;
  title: string;
  children: React.ReactNode;
  titleVariant?: TypographyProps["variant"];
  fullWidth?: boolean;
  loading?: boolean;
  error?: orch.Error;
  dialogButtons?: React.ReactNode;
  onClose: () => void;
  onDone: () => void;
}

export function FormDialog({
  open,
  title,
  titleVariant,
  fullWidth,
  loading,
  error,
  children,
  dialogButtons,
  onClose,
  onDone,
}: Props) {
  const classes = useStyles();

  return (
    <Dialog
      open={open}
      onEscapeKeyDown={onClose}
      fullWidth={fullWidth}
      maxWidth={fullWidth ? false : "sm"}
    >
      {!open ? (
        <div />
      ) : (
        <div className={classes.dialogContainer}>
          <IconButton
            disabled={loading}
            aria-label="close"
            className={classes.cancelEdit}
            onClick={onClose}
          >
            <CloseIcon />
          </IconButton>
          <Typography variant={titleVariant ?? "h4"} className={classes.dialogTitle}>
            {title}
          </Typography>
          {children}
          <div>
            <Button
              disabled={loading}
              className={classes.doneButton}
              variant="contained"
              color="primary"
              onClick={onDone}
            >
              Done
            </Button>
            {dialogButtons}
          </div>
          <div className={classes.dialogStatus}>
            {loading && <CircularProgress size="2rem" color="primary" />}
            {error && (
              <Typography variant="body1" color="primary" className={classes.dialogError}>
                {error.error}
              </Typography>
            )}
          </div>
        </div>
      )}
    </Dialog>
  );
}
