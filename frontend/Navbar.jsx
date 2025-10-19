import { AppBar, Toolbar, Typography, Button } from "@mui/material";
import { Link } from "react-router-dom";

export default function Navbar() {
  return (
    <AppBar position="static" sx={{ backgroundColor: "#1976d2" }}>
      <Toolbar>
        <Typography variant="h6" sx={{ flexGrow: 1 }}>
          İş Zekâsı Paneli
        </Typography>
        <Button color="inherit" component={Link} to="/">Dashboard</Button>
        <Button color="inherit" component={Link} to="/reports">Reports</Button>
        <Button color="inherit" component={Link} to="/settings">Settings</Button>
      </Toolbar>
    </AppBar>
  );
}
