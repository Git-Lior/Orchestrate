import { createMuiTheme } from "@material-ui/core/styles";

// TODO: responsive font size?
const theme = createMuiTheme({
  direction: "rtl",
  typography: {
    fontFamily: [
      "-apple-system",
      "BlinkMacSystemFont",
      '"Assistant"',
      '"Segoe UI"',
      '"Roboto"',
      '"Oxygen"',
      '"Ubuntu"',
      '"Cantarell"',
      '"Fira Sans"',
      '"Droid Sans"',
      '"Helvetica Neue"',
      "sans-serif",
    ].join(","),
    htmlFontSize: 10,
    h4: { fontFamily: "Alef" },
    button: { fontFamily: "Alef", fontWeigh: "bolder", fontSize: "1.6rem" },
  },
  palette: {
    primary: {
      main: "#e16b3c",
    },
    background: {
      default: "#f5f3ed",
    },
  },
  props: {
    // change default props of components
  },
});

export default theme;
