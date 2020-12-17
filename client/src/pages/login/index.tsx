import "./index.css";
import React from "react";

import Container from "@material-ui/core/Container";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";
import TextField from "@material-ui/core/TextField";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Button from "@material-ui/core/Button";

import logo from "assets/logo.png";

function LoginPage() {
  return (
    <Container className="login-page">
      <img src={logo} className="app-logo" alt="Orchestrate" />
      <Container maxWidth="sm">
        <Paper className="login-container" elevation={5}>
          <Typography variant="h4" align="center">
            התחברות
          </Typography>
          <form>
            <TextField
              required
              fullWidth
              autoFocus
              variant="outlined"
              margin="normal"
              id="email"
              name="email"
              autoComplete="email"
              label="כתובת מייל"
            />
            <TextField
              type="password"
              required
              fullWidth
              variant="outlined"
              margin="normal"
              id="password"
              name="password"
              label="סיסמה"
              autoComplete="current-password"
            />
            <FormControlLabel
              control={<Checkbox value="remember" color="primary" />}
              label="זכור אותי"
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              className="login-submit"
            >
              התחבר
            </Button>
          </form>
        </Paper>
      </Container>
    </Container>
  );
}

export default LoginPage;
