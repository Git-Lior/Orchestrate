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
  noLineHeight: { lineHeight: 0 },
  groupSelect: {
    marginLeft: 20,
    background: "none",
    width: 200,
    padding: "2px 10px",
    cursor: "default",
    "&:before, &:after": { borderBottomColor: `${theme.palette.secondary.main} !important` },
  },
  spacer: { flexGrow: 1 },
  username: {
    marginRight: 10,
  },
  groupTabs: {
    "&[disabled]": { pointerEvents: "none" },
  },
});

export default layoutStyles;