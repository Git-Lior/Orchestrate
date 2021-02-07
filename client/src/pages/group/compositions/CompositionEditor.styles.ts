import { Styles } from "@material-ui/core/styles/withStyles";
import { AppTheme } from "AppTheme";

const compositionsEditorStyles: Styles<AppTheme, {}> = theme => ({
  container: {
    position: "relative",
    backgroundColor: theme.palette.background.default,
    borderRadius: 5,
    boxShadow: "0 0 20px",
    padding: "4em",
  },
  title: {
    marginBottom: "1em",
    textAlign: "center",
  },
  cancelEdit: { position: "absolute", left: 0, top: 0 },
  editorContainer: {},
  compositionInfo: {
    display: "flex",
    flexDirection: "column",
  },
  formRow: { display: "flex", marginBottom: "1em" },
  formRowLabel: { marginRight: "10px", fontSize: "18px" },
  doneButton: { marginTop: "20px" },
  sheetMusicContainer: {},
  actions: {
    display: "flex",
    justifyContent: "space-around",
  },
});

export default compositionsEditorStyles;
