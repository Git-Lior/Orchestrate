import React, { useCallback, useEffect, useState } from "react";
import { generatePath, useHistory, useRouteMatch } from "react-router";

import makeStyles from "@material-ui/core/styles/makeStyles";
import Card from "@material-ui/core/Card";
import Typography from "@material-ui/core/Typography";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemText from "@material-ui/core/ListItemText";
import ListItemSecondaryAction from "@material-ui/core/ListItemSecondaryAction";
import IconButton from "@material-ui/core/IconButton";
import DeleteIcon from "@material-ui/icons/Delete";

import sheetMusicPanelStyles from "./SheetMusicPanel.styles";
import { MOCK_COMPOSITIONS, MOCK_SHEET_MUSIC } from "mocks";
import { useApiFetch } from "utils/hooks";

const useStyles = makeStyles(sheetMusicPanelStyles);

interface Props {
  user: orch.User;
  userInfo: orch.group.UserInfo;
}

type RouteParams = Required<orch.compositions.RouteParams>;

export default function SheetMusicPanel({ user, userInfo }: Props) {
  const classes = useStyles();
  const apiFetch = useApiFetch(user);
  const history = useHistory();
  const { url, path, params } = useRouteMatch<RouteParams>();
  const [composition, setComposition] = useState<orch.Composition | null>(null);
  const [sheetMusic, setSheetMusic] = useState<orch.SheetMusic | null>(null);

  useEffect(() => {
    let newComposition = composition;
    if (params.compositionId !== newComposition?.id.toString()) {
      /* const newComposition = await apiFetch(`...`); */
      newComposition = MOCK_COMPOSITIONS.find(_ => _.id.toString() === params.compositionId)!;
      return setComposition(newComposition);
    }

    if (!params.roleId) return history.replace(`${url}/${newComposition.roles[0].id}`);

    /* const sheetMusic = await apiFetch(`...`); */
    const sheetMusic = MOCK_SHEET_MUSIC[params.roleId];
    setSheetMusic(sheetMusic);
  }, [composition, params.compositionId, params.roleId]);

  const onRoleClick = useCallback(
    (e: React.MouseEvent) => {
      const roleId = (e.currentTarget as any).dataset.roleId;
      history.push(generatePath(path, { ...params, roleId }));
    },
    [history, path, params]
  );

  const onDeleteClick = useCallback(() => {
    if (!window.confirm("Are you sure you want to delete this instrument?")) return;
  }, []);

  if (!composition) return <div>loading composition...</div>;

  const hasMultipleInstruments = userInfo.director || userInfo.roles.length > 1;

  return (
    <div className={classes.container}>
      {hasMultipleInstruments && (
        <Card className={classes.instruments}>
          <Typography variant="h5">Choose Instrument:</Typography>
          <List>
            {composition.roles.map(role => (
              <ListItem
                key={role.id}
                button
                divider
                selected={params.roleId === role.id.toString()}
                data-role-id={role.id}
                onClick={onRoleClick}
              >
                <ListItemText primary={`${role.section}${!role.num ? "" : " " + role.num}`} />
                {userInfo.director && (
                  <ListItemSecondaryAction>
                    <IconButton edge="end" aria-label="delete" onClick={onDeleteClick}>
                      <DeleteIcon />
                    </IconButton>
                  </ListItemSecondaryAction>
                )}
              </ListItem>
            ))}
          </List>
        </Card>
      )}
      {sheetMusic && (
        <>
          <Card className={classes.comments}>
            <Typography variant="body1">Comments</Typography>
          </Card>
          <Card className={classes.sheetMusicViewer}>
            <iframe src={sheetMusic.fileUrl} className={classes.sheetMusicFrame} />
          </Card>
        </>
      )}
    </div>
  );
}
