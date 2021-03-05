import React, { useEffect, useState } from "react";

import { makeStyles } from "@material-ui/core/styles";
import Card from "@material-ui/core/Card";
import Typography from "@material-ui/core/Typography";
import { useApiFetch } from "utils/hooks";

const useStyles = makeStyles({
  comments: { flex: 2, marginRight: "3em" },
  sheetMusicViewer: { flex: 3 },
  sheetMusicFrame: { width: "100%", height: "100%", border: "none" },
});

interface Props {
  user: orch.User;
  group: orch.Group;
  userInfo: orch.group.UserInfo;
  composition: orch.Composition;
  roleId: string;
  isUploading: boolean;
}

export default function SheetMusicViewer({
  user,
  group,
  userInfo,
  composition,
  roleId,
  isUploading,
}: Props) {
  const classes = useStyles();

  const [fileUrl, setFileUrl] = useState<string>();
  const [comments, setComments] = useState<orch.SheetMusicComment[]>();

  const apiFetch = useApiFetch(
    user,
    `/groups/${group.id}/compositions/${composition.id}/roles/${roleId}`
  );

  useEffect(() => {
    return () => {
      if (fileUrl) URL.revokeObjectURL(fileUrl);
    };
  }, [fileUrl]);

  useEffect(() => {
    if (isUploading) return;

    setFileUrl(undefined);
    setComments(undefined);

    apiFetch("comments").then(setComments);
    apiFetch("file", {}, "blob").then(file => {
      setFileUrl(URL.createObjectURL(file));
    });
  }, [isUploading, apiFetch]);

  return (
    <>
      <Card className={classes.comments}>
        <Typography variant="body1">Comments</Typography>
      </Card>
      <Card className={classes.sheetMusicViewer}>
        {fileUrl && (
          <iframe title="sheet-music-file" src={fileUrl} className={classes.sheetMusicFrame} />
        )}
      </Card>
    </>
  );
}
