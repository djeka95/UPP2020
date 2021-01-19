import React from 'react';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import TextField from '@material-ui/core/TextField';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import { NavLink } from 'react-router-dom';
import { Checkbox, FormControlLabel, InputLabel, MenuItem, Select } from '@material-ui/core';
import Axios from 'axios';
import User from '../../model/User'
import Alert from '@material-ui/lab/Alert';

export default class SignUp extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            firstName:'',
            firstNameOk:false,
            lastName:'',
            lastNameOk:false,
            email:'',
            emailOk:false,
            password:'',
            passwordOk:false,
            writer:false,
            city:'',
            cityOk:false,
            country:'',
            countryOk:true,
            selectedGenres:[],
            formDisabled:true,
            formFieldsData:[],
            errorText:'',
            error:false,
            genreError:true,
            dropGenres:false,
            writerChecked:false,
            betaReaderChecked:false
        }

        this.onButtonClick = this.onButtonClick.bind(this)
        this.handleChange = this.handleChange.bind(this)
    }
    componentDidMount(){
      Axios.get("https://localhost:44385/api/register?processDefinitionId=&&ProcessInstanceId=")
      .then(res => {
        this.setState({formFieldsData:res.data})
        console.log(res.data)
        var countryField = res.data.filter(x => x['Key'] === 'country')
        var someCountry = countryField[0]['Value']['value']['value'][20]
        this.setState({country:someCountry})
      })
    }

  handleChange = (type,e) => {
    this.setState({error:false})
    this.setState({errorText:""})

    if(type === 'writer'){
      this.setState({writerChecked:!this.state.writerChecked})
      this.setState({writerChecked:!this.state.writerChecked})
      this.setState({writerChecked:!this.state.writerChecked})
    }
    if(type === 'betaReader'){
      this.setState({betaReaderChecked:!this.state.betaReaderChecked})
    }
    if(type === 'firstName')
        if(e.target.value.length !== 0){
        this.setState({firstName:e.target.value})
        this.setState({firstNameOk:true})
        }else{
          let currentField = this.state.formFieldsData.filter(x => x['Key'] === "firstName")
          if(currentField[0]['Value']['value']['Required'] === true){
            this.setState({firstNameOk:false})
            this.setState({error:true})
            this.setState({errorText:"First Name is required"})
          }
        }

    if(type === 'lastName')
        if(e.target.value.length !== 0){
        this.setState({lastName:e.target.value})
        this.setState({lastNameOk:true})
        }else{
          let currentField = this.state.formFieldsData.filter(x => x['Key'] === "lastName")
          if(currentField[0]['Value']['value']['Required'] === true){
            this.setState({lastNameOk:false})
            this.setState({error:true})
            this.setState({errorText:"Last Name is required"})
          }
        }

    if(type === 'email')
        if(e.target.value.length !== 0){
          var currentField = this.state.formFieldsData.filter(x => x['Key'] === "email")
          var regexString = currentField[0]['Value']['value']['value']
          var regex = RegExp(regexString);

          if(regex.test(e.target.value))
          {
            this.setState({email:e.target.value})
            this.setState({emailOk:true})
          }
          else
          {
            this.setState({emailOk:false})
            this.setState({error:true})
            this.setState({errorText:"Email mmust be is format example@exaammple.xx"})
          }
        }else{
          if(this.state.formFieldsData.filter(x => x['Key'] === "email")[0]['Value']['value']['Required'] === true){
            this.setState({emailOk:false})
            this.setState({error:true})
            this.setState({errorText:"Email is required"})
          }
        }

    if(type === 'country')
        if(e.target.value.length !== 0){
        this.setState({country:e.target.value})
        this.setState({countryOk:true})
        }else{
            this.setState({countryOk:false})
        }

    if(type === 'city')
        if(e.target.value.length !== 0){
        this.setState({city:e.target.value})
        this.setState({cityOk:true})
        }else{
          if(this.state.formFieldsData.filter(x => x['Key'] === "city")[0]['Value']['value']['Required'] === true){
            this.setState({cityOk:false})
            this.setState({error:true})
            this.setState({errorText:"City is required"})
          }
        }

    if(type === 'password'){
        if(e.target.value.length > this.state.formFieldsData.filter(x => x['Key'] === "password")[0]['Value']['value']['MinLength']){
        this.setState({password:e.target.value})
        this.setState({passwordOk:true})
        }else{
            this.setState({passwordOk:false})
            this.setState({error:true})
            this.setState({errorText:"Password is required and must be longer than 8 chars"})
        }
      }
    if(type === 'writer')
        this.setState({writer:!this.state.writer})
    if(type === 'genre'){
        var selected = this.state.selectedGenres.concat(e.target.value);
        this.setState({ selectedGenres: selected })
        this.setState({genreError:false})
    }

    if(this.state.writerChecked || this.state.betaReaderChecked){
      this.setState({dropGenres: true})
    }else{
      this.setState({dropGenres: false})
    }

    if(this.state.firstNameOk && this.state.lastNameOk && this.state.emailOk 
        && this.state.passwordOk && this.state.countryOk && this.state.cityOk)
        this.setState({formDisabled:false},(() => {}))
    else
        this.setState({formDisabled:true},(() => {}))
  }

  onButtonClick = () =>{
      var task = this.state.formFieldsData.filter(x => x['Key'] === "TaskId");
      var currentTask = task[0];

      var processDefinition = this.state.formFieldsData.filter(x => x['Key'] === "ProcessDefinitionId");
      var currentProcessDefinition = processDefinition[0];

      var processInstance = this.state.formFieldsData.filter(x => x['Key'] === "ProcessInstanceId");
      var currentProcessInstance = processInstance[0];

      var dto = new User(this.state.firstName,this.state.lastName,this.state.email,this.state.password,
        this.state.selectedGenres,this.state.country,this.state.city,this.state.writerChecked,this.state.betaReaderChecked,currentTask['Value'],currentProcessDefinition['Value'],currentProcessInstance['Value'])
      console.log(JSON.stringify(dto))
      const config = {headers: {'content-type':'application/json'}}
      Axios.post("https://localhost:44385/api/register",JSON.stringify(dto),config)
      .then(res =>{
        window.alert("Registration successful! Please check your email.");
      }).catch(error =>{
        window.alert("Something went wrong. Please try again.");

        Axios.get("https://localhost:44385/api/register?processDefinitionId=" + currentProcessDefinition['Value'] + "&&processInstanceId=" + currentProcessInstance['Value'])
        .then(res => {
          this.setState({formFieldsData:res.data})
          var countryField = res.data.filter(x => x['Key'] === 'country')
          var someCountry = countryField[0]['Value']['value']['value'][20]
          this.setState({country:someCountry})
        })
      })
  }

  returnFieldRender(field){
    if(field['Value'] !== null)
    {
      if((field['Value']['type'] === 'String'))
      {
        if( (Array.isArray(field['Value']['value']['value'])))
        {
          if(field['Key'] === "country")
          {
            return (
              <Grid key={field['Key']} item xs={12}>
              <InputLabel>{field['Key']}</InputLabel>
              <Select value={this.state.country} onChange={(e) => this.handleChange(field['Key'],e)}>
              {field['Value']['value']['value'].map( (item,index) => (<MenuItem key={item} value={item}>{item}</MenuItem> ) )}
              </Select>
              </Grid>
            )
          }

          if(this.state.dropGenres){
            return (
              <Grid key={field['Key']} item xs={12}>
              <InputLabel>{field['Key']}</InputLabel>
              <Select error={this.state.genreError} value={[]} onChange={(e) => this.handleChange(field['Key'],e)}>
              {field['Value']['value']['value'].map( (item,index) => (<MenuItem key={item} value={item}>{item}</MenuItem> ) )}
              </Select>
              </Grid>
            )
          }
        }
        else
        {
          if(field['Key'] === 'password')
          {
            return (
              <Grid key={field['Key']} item xs={12}>
              <TextField
                variant="outlined"
                required
                fullWidth
                error={this.state.error}
                id={field['Key']}
                label={field['Key']}
                name={field['Key']}
                type="password"
                autoFocus
                onChange={(e) => this.handleChange(field['Key'],e)}
              />
            </Grid>)
          }

          return (
            <Grid key={field['Key']} item xs={12}>
            <TextField
              variant="outlined"
              required
              fullWidth
              error={this.state.error}
              id={field['Key']}
              label={field['Key']}
              name={field['Key']}
              autoFocus
              onChange={(e) => this.handleChange(field['Key'],e)}
            />
          </Grid>)
        }
      }

      if(field['Value']['type'] === 'Boolean'){
        return(
        <Grid key={field['Key']} item xs={12}>
          <FormControlLabel
            control={<Checkbox onClick={(e) => this.handleChange(field['Key'],e)} />}
            label={field['Key'].toLowerCase() + "?"}
          />
        </Grid>)
      }
    }
  }

 render() {
  return ((
    <Container component="main" maxWidth="xs">
      <CssBaseline />
      <div >
        <Typography variant="h1">
          Sign up
        </Typography>
        <form  noValidate>
          <Grid container spacing={2}>
            {this.state.formFieldsData.map(field =>(
              this.returnFieldRender(field)
            ))}

          </Grid>
          <Button
            type="Button"
            fullWidth
            variant="contained"
            color="primary"
            onClick={this.onButtonClick}
            disabled={this.state.formDisabled}
          >
            Sign Up
          </Button>
          <Grid container justify="flex-end">
            <Grid item>
              <NavLink to="/signin" variant="body2">
                Already have an account? Sign in
              </NavLink>
            </Grid>
          </Grid>
        </form>

        {this.state.error === true? 
        <Alert severity="error">{this.state.errorText}</Alert>
        :
        <div></div>}
      </div>
    </Container>
  ))} 
}