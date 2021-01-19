import React from 'react';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import TextField from '@material-ui/core/TextField';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';


export default class forgotPassword extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            email:'',
            emailOk:false,
            formDisabled:true
        }

        this.onButtonClick = this.onButtonClick.bind(this)
        this.handleChange = this.handleChange.bind(this)
    }

  handleChange = (type,e) => {
    if(type === 'email')
        if(e.target.value.length !== 0){
        this.setState({email:e.target.value})
        this.setState({emailOk:true})
        }else{
            this.setState({emailOk:false})
        }

    if(this.state.emailOk)
        this.setState({formDisabled:false},(() => {}))
    else
        this.setState({formDisabled:true},(() => {}))
  }

  onButtonClick(){
      console.log(this.state)
  }

 render() { return ((
    <Container component="main" maxWidth="xs">
      <CssBaseline />
      <div >
        <Typography variant="h4">
          Reset Password
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
          </Grid>
          <Button
            type="Button"
            fullWidth
            variant="contained"
            color="primary"
            onClick={this.onButtonClick}
            disabled={this.state.formDisabled}
          >
            Reset Password
          </Button>
        </form>
      </div>
    </Container>
  ))} 
}
