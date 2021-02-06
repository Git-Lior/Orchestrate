import { Styles } from "@material-ui/core/styles/withStyles";
import { AppTheme } from "AppTheme";

const compositionsStyles: Styles<AppTheme, {}> = () => ({
  panelTitle: {
    textAlign: "center",
    margin: "20px 0",
  },
  panelsContainer: { display: "flex", width: "100%", padding: "0 3em" },
  resultsPanel: { flex: 1 },
  filtersPanel: {
    width: "21em",
    marginRight: "3em",
    padding: "2em",
  },
  compositionInfo: {
    padding: "1em",
  },
  filterRow: {
    display: "flex",
  },
});

export default compositionsStyles;
