import { Styles } from "@material-ui/core/styles/withStyles";
import { AppTheme } from "AppTheme";

const compositionsPanelStyles: Styles<AppTheme, {}> = {
  panelTitle: {
    textAlign: "center",
    marginBottom: "1em",
  },
  panelsContainer: { display: "flex", width: "100%", height: "100%" },
  compositionsTable: { flex: 1 },
  filtersPanel: {
    width: "21em",
    marginRight: "3em",
    padding: "2em",
    "& > *": { marginBottom: "2rem" },
  },
  filtersTitle: {
    textAlign: "center",
  },
  compositionInfo: {
    padding: "1em",
  },
  filterRow: {
    display: "flex",
  },
};

export default compositionsPanelStyles;
