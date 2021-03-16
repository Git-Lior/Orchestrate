import { fade } from "@material-ui/core/styles";
import { Styles } from "@material-ui/core/styles/withStyles";

import { AppTheme } from "AppTheme";

const toolbarHeight = 50;

const layoutStyles: Styles<AppTheme, {}> = theme => ({
  toolbar: { minHeight: toolbarHeight, height: toolbarHeight },
  appLogo: {
    height: 0.9 * toolbarHeight,
    borderRadius: 5,
    backgroundColor: theme.palette.secondary.main,
    marginRight: 30,
  },
  groupSelect: {
    marginLeft: 20,
    backgroundColor: fade(theme.palette.secondary.main, 0.8),
    height: 40,
    width: 200,
    padding: "2px 10px",
    cursor: "default",
    "&:before, &:after": { borderBottomColor: `${theme.palette.secondary.main} !important` },
  },
  spacer: { flexGrow: 1 },
  user: {
    padding: "1rem",
    display: "flex",
    cursor: "pointer",
  },
  groupTabs: {
    "&[disabled]": { pointerEvents: "none" },
  },
  pageContent: {
    display: "flex",
    flexDirection: "column",
    padding: "2em",
    height: `calc(100% - ${toolbarHeight}px)`,
    position: "relative",
  },
});

export default layoutStyles;
