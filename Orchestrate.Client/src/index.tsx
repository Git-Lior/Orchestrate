import "./index.css";

import React from "react";
import ReactDOM from "react-dom";
import moment from "moment";
import { BrowserRouter, Route, Switch } from "react-router-dom";

import reportWebVitals from "./reportWebVitals";
import AppTheme from "./AppTheme";

import App from "./App";
import AdminApp from "./admin";

moment.locale("he");

ReactDOM.render(
  <React.StrictMode>
    <AppTheme
      children={
        <BrowserRouter>
          <Switch>
            <Route exact path="/admin-panel" children={<AdminApp />} />
            <Route children={<App />} />
          </Switch>
        </BrowserRouter>
      }
    />
  </React.StrictMode>,
  document.getElementById("root")
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
