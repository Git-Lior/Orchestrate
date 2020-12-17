import "./index.css";
import React from "react";
import ReactDOM from "react-dom";
import { create } from "jss";
import rtl from "jss-rtl";
import CssBaseline from "@material-ui/core/CssBaseline";
import {
  StylesProvider,
  jssPreset,
  ThemeProvider,
} from "@material-ui/core/styles";

import reportWebVitals from "./reportWebVitals";
import theme from "./appTheme";
import App from "./App";

const jss = create({ plugins: [...jssPreset().plugins, rtl()] });

ReactDOM.render(
  <React.StrictMode>
    <StylesProvider jss={jss}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <App />
      </ThemeProvider>
    </StylesProvider>
  </React.StrictMode>,
  document.getElementById("root")
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
