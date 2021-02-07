import { Styles } from "@material-ui/core/styles/withStyles";
import { AppTheme } from "AppTheme";

const compositionsEditorStyles: Styles<AppTheme, {}> = theme => ({
  container: {
    position: "relative",
    height: "calc(100% - 8em)",
    width: "calc(100% - 8em)",
    margin: "4em",
    backgroundColor: theme.palette.background.default,
    borderRadius: 5,
    boxShadow: "0 0 20px",
    padding: "4em",
    display: "flex",
    flexDirection: "column",
  },
  title: {
    marginBottom: "1em",
    textAlign: "center",
  },
  cancelEdit: { position: "absolute", left: 0, top: 0 },
  editorContainer: {
    flex: 1,
    display: "flex",
  },
  editorDivider: {
    margin: "0 4em",
    width: "2px",
  },
  compositionInfo: {
    display: "flex",
    flexDirection: "column",
  },
  formRow: { display: "flex", marginBottom: "2em" },
  formRowLabel: { marginRight: "10px" },
  doneButton: { alignSelf: "flex-start" },
  sheetMusicContainer: {},
});

export default compositionsEditorStyles;
