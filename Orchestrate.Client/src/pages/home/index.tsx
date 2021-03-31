import React from "react";

import { makeStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
import Typography from "@material-ui/core/Typography";

import badNotes from "assets/bad-notes.png";

import { LoadingMessage } from "utils/components";

const useStyles = makeStyles({
  root: {
    height: "100%",
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    justifyContent: "space-evenly",
  },
  secondaryText: { textAlign: "center", marginTop: "2rem" },
  badNotesImage: { opacity: 0.6 },
});

export default function HomePage({ groups }: orch.PageProps) {
  const classes = useStyles();

  if (!groups) return <LoadingMessage text="Loading groups..." />;

  return (
    <Container maxWidth="md" className={classes.root}>
      <div>
        <Typography variant="h4" color="primary">
          It seems that you're not a member of a group :(
        </Typography>
        <Typography variant="h5" color="primary" className={classes.secondaryText}>
          please contact your group's manager
        </Typography>
      </div>
      <img src={badNotes} className={classes.badNotesImage} />
    </Container>
  );
}
