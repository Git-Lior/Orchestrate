import { Styles } from "@material-ui/core/styles/withStyles";
import { AppTheme } from "AppTheme";

const sheetMusicPanelStyles: Styles<AppTheme, {}> = () => ({
  container: { display: "flex", height: "100%" },
  instruments: { padding: "1em", marginRight: "2em" },
  comments: { flex: 2, marginRight: "3em" },
  sheetMusicViewer: { flex: 3 },
  sheetMusicFrame: { width: "100%", height: "100%", border: "none" },
});

export default sheetMusicPanelStyles;
