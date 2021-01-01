import { fade } from "@material-ui/core/styles/colorManipulator";
import { Styles } from "@material-ui/core/styles/withStyles";

import { AppTheme } from "AppTheme";

const layoutStyles: Styles<AppTheme, {}> = theme => ({
  toolbar: { height: "65px" },
  appLogo: {
    height: "65px",
    backgroundColor: theme.palette.secondary.main,
  },
  noLineHeight: { lineHeight: 0 },
  groupSelect: {
    backgroundColor: fade(theme.palette.secondary.main, 0.6),
    width: 200,
    padding: "2px 10px",
    height: "100%",
    cursor: "default",
  },
  groupTabs: {
    "&[disabled]": { pointerEvents: "none" },
  },
  buttonsSpacer: { flexGrow: 1 },
});

export default layoutStyles;
