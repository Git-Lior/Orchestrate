import React, { useCallback, useEffect, useMemo, useState } from "react";
import { generatePath, Redirect, useHistory, useRouteMatch } from "react-router";

import makeStyles from "@material-ui/core/styles/makeStyles";
import Card from "@material-ui/core/Card";
import Typography from "@material-ui/core/Typography";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemText from "@material-ui/core/ListItemText";
import ListItemSecondaryAction from "@material-ui/core/ListItemSecondaryAction";
import IconButton from "@material-ui/core/IconButton";
import PublishIcon from "@material-ui/icons/Publish";

import { useApiFetch } from "utils/hooks";
import { roleToText } from "utils/general";

import SheetMusicViewer from "./SheetMusicViewer";
import CircularProgress from "@material-ui/core/CircularProgress";

const useStyles = makeStyles({
  container: { display: "flex", height: "100%" },
  roles: { padding: "2rem", marginRight: "4rem", minWidth: "25rem" },
});

interface Props {
  user: orch.User;
  group: orch.Group;
  userInfo: orch.group.UserInfo;
}

type RouteParams = Required<orch.compositions.RouteParams>;

export default function SheetMusicsPanel({ user, group, userInfo }: Props) {
  const classes = useStyles();
  const history = useHistory();
  const { path, params } = useRouteMatch<RouteParams>();
  const apiFetch = useApiFetch(user, `/groups/${group.id}/compositions/${params.compositionId}`);

  const [composition, setComposition] = useState<orch.Composition>();
  const [uploadRoleIds, setUploadRoleIds] = useState<Set<number>>(new Set<number>());

  const availableRoleIds = useMemo(() => new Set<number>(composition?.roles.map(_ => _.id)), [
    composition,
  ]);

  const roles: orch.Role[] | undefined = useMemo(
    () => (userInfo.director ? group : composition)?.roles,
    [group, composition, userInfo.director]
  );

  useEffect(() => {
    (async function () {
      if (params.compositionId !== composition?.id?.toString())
        return setComposition(await apiFetch(""));
    })();
  }, [composition, params.compositionId, apiFetch]);

  const setRolePage = useCallback(
    (roleId: string) => history.push(generatePath(path, { ...params, roleId })),
    [history, path, params]
  );

  const onRoleClick = useCallback(
    (e: React.MouseEvent) => setRolePage((e.currentTarget as any).dataset.roleId),
    [setRolePage]
  );

  const onFileSelected = useCallback(
    async (role: orch.Role, e: React.ChangeEvent<HTMLInputElement>) => {
      const file = e.target.files?.[0];
      if (!file || uploadRoleIds.has(role.id)) return;

      const fileAlreadyExists = availableRoleIds.has(role.id);

      const data = new FormData();
      data.append("file", file);

      setUploadRoleIds(ids => new Set<number>(ids).add(role.id));

      try {
        await apiFetch(
          role.id.toString(),
          {
            method: "POST",
            body: data,
          },
          "none"
        );

        if (!fileAlreadyExists) setComposition(c => ({ ...c!, roles: [...c!.roles, role] }));

        setRolePage(role.id.toString());
      } finally {
        setUploadRoleIds(ids => {
          const set = new Set<number>(ids);
          set.delete(role.id);
          return set;
        });
      }
    },
    [apiFetch, uploadRoleIds, availableRoleIds, setRolePage]
  );

  if (!composition || !roles) return <div>loading composition...</div>;

  // ensure roleId appears in available roles
  if (params.roleId && !availableRoleIds.has(Number(params.roleId)))
    return <Redirect to={generatePath(path, { ...params, roleId: undefined })} />;

  // only director can see sheet musics landing page
  if (!userInfo.director && !params.roleId)
    return (
      <Redirect
        to={generatePath(path, { ...params, roleId: composition.roles[0].id.toString() })}
      />
    );

  return (
    <div className={classes.container}>
      {(userInfo.director || composition.roles.length > 1) && (
        <Card className={classes.roles}>
          <Typography variant="h5">Available Roles:</Typography>
          <List>
            {roles.map(role => (
              <ListItem
                key={role.id}
                button
                divider
                disabled={!availableRoleIds.has(role.id)}
                selected={params.roleId === role.id.toString()}
                data-role-id={role.id}
                onClick={onRoleClick}
              >
                <ListItemText primary={roleToText(role)} />
                {userInfo.director &&
                  (uploadRoleIds.has(role.id) ? (
                    <ListItemSecondaryAction>
                      <CircularProgress color="primary" size="3rem" />
                    </ListItemSecondaryAction>
                  ) : (
                    <ListItemSecondaryAction>
                      <input
                        type="file"
                        hidden
                        id={"sheet-music-upload-" + role.id}
                        accept="application/pdf"
                        onChange={e => onFileSelected(role, e)}
                      />
                      <label htmlFor={"sheet-music-upload-" + role.id}>
                        <IconButton edge="end" aria-label="upload" component="span">
                          <PublishIcon />
                        </IconButton>
                      </label>
                    </ListItemSecondaryAction>
                  ))}
              </ListItem>
            ))}
          </List>
        </Card>
      )}
      {params.roleId && (
        <SheetMusicViewer
          user={user}
          userInfo={userInfo}
          group={group}
          composition={composition}
          roleId={params.roleId}
          isUploading={uploadRoleIds.has(Number(params.roleId))}
        />
      )}
    </div>
  );
}
