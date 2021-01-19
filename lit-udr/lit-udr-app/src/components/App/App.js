import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import CssBaseline from '@material-ui/core/CssBaseline';
import Toolbar from '@material-ui/core/Toolbar';
import HomeIcon from '@material-ui/icons/HomeSharp';
import {  Button } from '@material-ui/core';
import { useHistory } from 'react-router-dom';


export default function App() {
    let history = useHistory();
    const redirectToHome = (text) =>{
            history.push('/');
    }
    const redirectToSignIn = (text) =>{
        history.push('/signin');
    }
    const signOut = (text) =>{
          history.push('');
          localStorage.clear();
    }

  return (
    <React.Fragment>
      <CssBaseline />
      <AppBar position="relative">
        <Toolbar>
            <HomeIcon />
            <Button  color="inherit"  onClick={redirectToHome}>
             Literarno Udruzenje
            </Button>

            <Button  color="inherit"  onClick={redirectToSignIn}>
             Sing In
            </Button>

            <Button  color="inherit"  onClick={signOut}>
             Sing Out
            </Button>

        </Toolbar>
      </AppBar>
    </React.Fragment>
  );
}