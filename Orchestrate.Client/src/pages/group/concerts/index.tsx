import React, { useCallback, useEffect, useState } from "react";

import { makeStyles } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import Checkbox from "@material-ui/core/Checkbox";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Container from "@material-ui/core/Container";
import CircularProgress from "@material-ui/core/CircularProgress";
import Fab from "@material-ui/core/Fab";
import AddIcon from "@material-ui/icons/Add";

import { useApiFetch, useCRUDApi } from "utils/hooks";

import ConcertCard from "./ConcertCard";

const useStyles = makeStyles({
  header: { display: "flex", justifyContent: "space-between", marginBottom: "2rem" },
  content: {},
  newConcert: {},
});

type Props = Required<orch.PageProps>;

export default function GroupConcertsPage({ user, userInfo, group }: Props) {
  const classes = useStyles();

  const apiFetch = useApiFetch(user, `/groups/${group.id}/concerts`);
  const [editedConcert, setEditedConcert] = useState<orch.ConcertData>();
  const [hideNotAttending, setHideNotAttending] = useState(false);

  const clearEditedConcert = useCallback(() => setEditedConcert(undefined), []);
  const addNewConcert = useCallback(() => setEditedConcert({} as any), []);

  const [concerts, refreshConcerts, changeConcert, removeConcert, setConcertQuery] = useCRUDApi<
    orch.Concert,
    orch.ConcertData
  >(user.token, `/groups/${group.id}/concerts`, { hideNotAttending });

  useEffect(() => {
    if (!editedConcert?.id || !concerts) return;

    setEditedConcert(concerts.find(_ => _.id === editedConcert.id));
  }, [editedConcert, concerts]);

  const toggleNotAttending = useCallback(
    (_, checked: boolean) => {
      setHideNotAttending(checked);
      setConcertQuery({ hideNotAttending: checked });
    },
    [setConcertQuery]
  );

  const changeEditedConcert = useCallback(
    (concert: orch.OptionalId<orch.ConcertData>) => changeConcert(concert).then(clearEditedConcert),
    [clearEditedConcert]
  );

  const changeAttendance = useCallback(
    (concert: orch.ConcertData, attending: boolean) =>
      apiFetch(
        `${concert.id}/attendance`,
        {
          method: "POST",
          body: JSON.stringify(attending),
        },
        "none"
      ).then(refreshConcerts),
    [apiFetch, refreshConcerts]
  );

  const getCompositions = useCallback(
    (title: string) => apiFetch(`/groups/${group.id}/compositions?title=${title}`),
    [apiFetch, group.id]
  );

  const addComposition = useCallback(
    (concert: orch.Concert, composition: orch.CompositionData) =>
      apiFetch(
        `${concert.id}/compositions`,
        { method: "POST", body: JSON.stringify(composition.id) },
        "none"
      ).then(refreshConcerts),
    [apiFetch, refreshConcerts]
  );
  const removeComposition = useCallback(
    (concert: orch.Concert, composition: orch.CompositionData) =>
      apiFetch(`${concert.id}/compositions/${composition.id}`, { method: "DELETE" }, "none").then(
        refreshConcerts
      ),
    [apiFetch, refreshConcerts]
  );

  return (
    <Container maxWidth="md">
      <div className={classes.header}>
        <Typography variant="h4">Upcoming Concerts</Typography>
        {userInfo.manager ? (
          <Fab color="primary" size="medium" onClick={addNewConcert}>
            <AddIcon />
          </Fab>
        ) : (
          <FormControlLabel
            control={<Checkbox color="primary" value="concerts" onChange={toggleNotAttending} />}
            label="Hide rejected concerts"
          />
        )}
      </div>
      {editedConcert && !editedConcert.id && (
        <div className={classes.newConcert}>
          <ConcertCard
            editMode
            userInfo={userInfo}
            onEditCancel={clearEditedConcert}
            onEditDone={changeEditedConcert}
          />
        </div>
      )}
      <div className={classes.content}>
        {!concerts ? (
          <CircularProgress />
        ) : (
          concerts?.map(concert => (
            <ConcertCard
              key={concert.id}
              editMode={concert.id === editedConcert?.id}
              concert={concert}
              userInfo={userInfo}
              compositionProvider={getCompositions}
              onEditRequested={setEditedConcert}
              onEditCancel={clearEditedConcert}
              onEditDone={changeEditedConcert}
              onDelete={removeConcert}
              onAttendanceChange={changeAttendance}
              onCompositionAdded={addComposition}
              onCompositionRemoved={removeComposition}
            />
          ))
        )}
      </div>
    </Container>
  );
}
