import CssBaseline from "@material-ui/core/CssBaseline";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";

const theme = createMuiTheme({
  typography: {
    htmlFontSize: 10,
    button: { fontWeigh: "bolder", fontSize: "1.6rem" },
  },
  palette: {
    primary: {
      main: "#e16b3c",
    },
    secondary: {
      main: "#f5f3ed",
    },
    background: {
      default: "#f5f3ed",
    },
  },
  overrides: {
    MuiSelect: {
      select: {
        "&:focus": { background: "none !important" },
      },
    },
  },
});

export type AppTheme = typeof theme;

type Props = React.PropsWithChildren<{}>;

export default function AppThemeComponent({ children }: Props) {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      {children}
    </ThemeProvider>
  );
}
