import { Styles } from "@material-ui/core/styles/withStyles";

import { AppTheme } from "AppTheme";

const styles: Styles<AppTheme, {}> = {
  container: {
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    height: "100%",
  },
  appLogo: {
    padding: "30px 0",
    width: "100%",
    maxWidth: "700px",
    opacity: "0.7",
  },
  loginArea: {
    marginTop: "30px",
    padding: "30px",
  },
  loginTitle: {
    textAlign: "center",
  },
  submitButton: {
    marginTop: "15px",
  },
  loginMessage: {
    textAlign: "center",
    marginTop: "15px",
  },
};

export default styles;
