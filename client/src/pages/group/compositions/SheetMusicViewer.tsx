import React, { useCallback, useEffect, useState } from "react";
import moment from "moment";

import { makeStyles } from "@material-ui/core/styles";
import Card from "@material-ui/core/Card";
import Typography from "@material-ui/core/Typography";
import TextField from "@material-ui/core/TextField";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemText from "@material-ui/core/ListItemText";
import ListItemAvatar from "@material-ui/core/ListItemAvatar";
import ListItemSecondaryAction from "@material-ui/core/ListItemSecondaryAction";
import IconButton from "@material-ui/core/IconButton";
import Fab from "@material-ui/core/Fab";
import SendIcon from "@material-ui/icons/Send";
import EditIcon from "@material-ui/icons/Edit";
import DoneIcon from "@material-ui/icons/Done";
import CloseIcon from "@material-ui/icons/Close";
import DeleteIcon from "@material-ui/icons/Delete";

import { useApiFetch, useInputState } from "utils/hooks";
import { UserAvatar } from "utils/components";

const MAX_MESSAGE_LENGTH = 300;
const MAX_LINE_BREAKS = 3;

const useStyles = makeStyles({
  commentsContainer: {
    flex: 2,
    marginRight: "4rem",
    padding: "2rem",
    display: "flex",
    flexDirection: "column",
  },
  comments: { flex: 1, paddingLeft: "15px", minHeight: 0, overflowY: "auto" },
  editIcon: {
    left: "-1.5rem",
    display: "flex",
    flexDirection: "column",
    width: "min-content",
  },
  commentText: { whiteSpace: "pre-wrap" },
  deletedComment: { opacity: 0.7 },
  deletedCommentText: { fontStyle: "italic" },
  newCommentContainer: {
    display: "flex",
    alignItems: "center",
    "& > :not(:last-child)": { marginRight: "2rem" },
  },
  sendCommentButton: { flex: "0 0 auto" },
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
  const [editedComment, setEditedComment] = useState<orch.SheetMusicComment>();
  const [newCommentText, setNewCommentText] = useInputState();

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

  const sendComment = useCallback(async () => {
    await apiFetch("comments", { method: "POST", body: JSON.stringify(newCommentText) }, "none");
    setComments(await apiFetch("comments"));
    setNewCommentText();
  }, [newCommentText, apiFetch, setNewCommentText]);

  const deleteComment = useCallback(
    async (comment: orch.SheetMusicComment) => {
      await apiFetch(`comments/${comment.id}`, { method: "DELETE" }, "none");
      setComments(await apiFetch("comments"));
    },
    [apiFetch]
  );

  const onEditedCommentChange = useCallback(
    (e: React.ChangeEvent<HTMLTextAreaElement>) =>
      setEditedComment(c => ({ ...c!, content: e.target.value })),
    []
  );

  const onEditCancel = useCallback(() => setEditedComment(undefined), []);
  const onEditDone = useCallback(async () => {
    if (!editedComment) return;

    const commentContent = editedComment.content.trim();

    if (
      !commentContent ||
      commentContent.length > MAX_MESSAGE_LENGTH ||
      hasExceedingLineBreaks(commentContent)
    )
      return;

    await apiFetch(
      `comments/${editedComment.id}`,
      { method: "PUT", body: JSON.stringify(commentContent) },
      "none"
    );
    setComments(await apiFetch("comments"));
    setEditedComment(undefined);
  }, [editedComment, apiFetch]);

  return (
    <>
      <Card className={classes.commentsContainer}>
        <Typography variant="h5">Comments</Typography>
        <List className={classes.comments}>
          {comments?.map(comment => {
            const { id, user: messageUser, content, createdAt, updatedAt } = comment;
            const isEditing = editedComment?.id === id;
            const isDeleted = !comment.content;
            return (
              <ListItem key={id} className={isDeleted ? classes.deletedComment : undefined}>
                {user.id === messageUser.id && !isDeleted && (
                  <ListItemSecondaryAction className={classes.editIcon}>
                    {!isEditing ? (
                      <>
                        <IconButton
                          size="small"
                          disableRipple
                          onClick={() => setEditedComment(comment)}
                        >
                          <EditIcon fontSize="inherit" />
                        </IconButton>
                        <IconButton
                          size="small"
                          disableRipple
                          onClick={() => deleteComment(comment)}
                        >
                          <DeleteIcon fontSize="inherit" />
                        </IconButton>
                      </>
                    ) : (
                      <>
                        <IconButton
                          size="small"
                          disableRipple
                          disabled={
                            isEditing &&
                            (!editedComment?.content ||
                              editedComment.content.length > MAX_MESSAGE_LENGTH)
                          }
                          onClick={onEditDone}
                        >
                          <DoneIcon fontSize="inherit" />
                        </IconButton>
                        <IconButton size="small" disableRipple onClick={onEditCancel}>
                          <CloseIcon fontSize="inherit" />
                        </IconButton>
                      </>
                    )}
                  </ListItemSecondaryAction>
                )}
                <ListItemAvatar>
                  <UserAvatar user={messageUser} />
                </ListItemAvatar>
                {isEditing ? (
                  <CommentBox
                    label="Edit comment"
                    value={editedComment!.content}
                    onChange={onEditedCommentChange}
                  />
                ) : (
                  <ListItemText
                    className={content ? classes.commentText : classes.deletedCommentText}
                    primary={content ?? "[Comment deleted by user]"}
                    secondary={
                      <>
                        {moment.unix(updatedAt ?? createdAt).format("DD/MM/yyyy HH:mm:ss")}
                        {updatedAt && " (edited)"}
                      </>
                    }
                  />
                )}
              </ListItem>
            );
          })}
        </List>
        <div className={classes.newCommentContainer}>
          <CommentBox
            label="Add comment"
            rows={3}
            value={newCommentText}
            onChange={setNewCommentText}
          />
          <Fab
            disabled={!newCommentText || newCommentText.length > MAX_MESSAGE_LENGTH}
            color="primary"
            size="medium"
            className={classes.sendCommentButton}
            onClick={sendComment}
          >
            <SendIcon />
          </Fab>
        </div>
      </Card>
      <Card className={classes.sheetMusicViewer}>
        {fileUrl && (
          <iframe title="sheet-music-file" src={fileUrl} className={classes.sheetMusicFrame} />
        )}
      </Card>
    </>
  );
}

interface CommentBoxProps {
  label: string;
  value: string;
  onChange: React.ChangeEventHandler<HTMLTextAreaElement>;
  rows?: number;
}

function CommentBox({ value, onChange, label, rows }: CommentBoxProps) {
  const exceedingLineBreaks = hasExceedingLineBreaks(value);
  return (
    <TextField
      label={label}
      fullWidth
      multiline
      rows={rows}
      rowsMax={5}
      error={exceedingLineBreaks || value.length > MAX_MESSAGE_LENGTH}
      helperText={
        exceedingLineBreaks
          ? `you can use at most ${MAX_LINE_BREAKS} line breaks`
          : `${value.length}/${MAX_MESSAGE_LENGTH}`
      }
      variant="outlined"
      value={value}
      onChange={onChange}
    />
  );
}

function hasExceedingLineBreaks(text: string) {
  return text.split("").filter(_ => _ === "\n").length > MAX_LINE_BREAKS;
}
