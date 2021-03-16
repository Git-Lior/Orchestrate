import React from "react";

import { makeStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";

import badNotes from "assets/bad-notes.png";
import Typography from "@material-ui/core/Typography";

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

  if (!groups) return <div>loading groups...</div>;

  if (groups.length > 0) return null; // not supposed to happen

  return (
    <Container maxWidth="md" className={classes.root}>
      <div>
        <Typography variant="h4" color="primary">
          Look like you are not part of any group :(
        </Typography>
        <Typography variant="h5" color="primary" className={classes.secondaryText}>
          please contact your group's manager
        </Typography>
      </div>
      <img src={badNotes} className={classes.badNotesImage} />
    </Container>
  );
}
