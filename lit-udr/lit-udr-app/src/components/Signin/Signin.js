import React from 'react';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import TextField from '@material-ui/core/TextField';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import { NavLink } from 'react-router-dom';
import LogInDto from '../../model/LogInDto';
import Axios from 'axios';
import { withRouter } from "react-router-dom";


class SignIn extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            email:'',
            emailOk:false,
            password:'',
            passwordOk:false,
            formDisabled:true
        }

        this.onButtonClick = this.onButtonClick.bind(this)
    }

  handleChange = (type,e) => {
    if(type === 'email')
        if(e.target.value.length !== 0){
        this.setState({email:e.target.value})
        this.setState({emailOk:true})
        }else{
            this.setState({emailOk:false})
        }

    if(type === 'password')
        if(e.target.value.length !== 0){
        this.setState({password:e.target.value})
        this.setState({passwordOk:true})
        }else{
            this.setState({passwordOk:false})
        }

    if(this.state.emailOk && this.state.passwordOk )
        this.setState({formDisabled:false},(() => {}))
    else
        this.setState({formDisabled:true},(() => {}))
  }

  onButtonClick(){
    var dto = new LogInDto(this.state.email,this.state.password);

    const config = {headers: {'content-type':'application/json'}}
    Axios.post("https://localhost:44385/api/register/login",JSON.stringify(dto),config)
    .then(res =>{
      localStorage.setItem('data',JSON.stringify(res.data))
      this.props.history.push("review")
      window.alert("LogIn Success");
    }).catch(error =>{
        window.alert("Something went wrong. Please try again.");
    })
  }

 render() { return ((
    <Container component="main" maxWidth="xs">
      <CssBaseline />
      <div >
        <Typography variant="h1">
          Sign In
        </Typography>
        <form  noValidate>
          <Grid container spacing={2}>
            <Grid item xs={12}>
              <TextField
                variant="outlined"
                required
                fullWidth
                id="email"
                label="Email Address"
                name="email"
                autoComplete="email"
                onChange={(e) => this.handleChange('email',e)}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                variant="outlined"
                required
                fullWidth
                name="password"
                label="Password"
                type="password"
                id="password"
                autoComplete="current-password"
                onChange={(e) => this.handleChange('password',e)}
              />
            </Grid>

          </Grid>
          <Button
            type="Button"
            fullWidth
            variant="contained"
            color="primary"
            onClick={this.onButtonClick}
            disabled={this.state.formDisabled}
          >
            Sign In
          </Button>
          <Grid container>
            <Grid item xs>
              <NavLink to="forgotPassword" variant="body2">
                Forgot password?
              </NavLink>
            </Grid>
            <Grid item>
              <NavLink to="/signup" variant="body2">
                {"Don't have an account? Sign Up"}
              </NavLink>
            </Grid>
          </Grid>
        </form>
      </div>
    </Container>
  ))} 
}

export default withRouter(SignIn)