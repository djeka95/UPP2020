import React from 'react';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles((theme) => ({
    footer: {
      backgroundColor: theme.palette.background.paper,
      padding: theme.spacing(6),
    },
  }));

export default function Copyright() {
    const classes = useStyles();

    return (
        <footer className={classes.footer}>
        <Typography variant="body2" color="textSecondary" align="center">
            {'Copyright © '}
            
            Literalno Udruzenje
            
            {new Date().getFullYear()}
            {'.'}
        </Typography>
      </footer>
    );
  }