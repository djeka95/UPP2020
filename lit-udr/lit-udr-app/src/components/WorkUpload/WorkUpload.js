import React from 'react';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import Axios from 'axios';
import NewUserData from '../../model/NewUserData';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';
import Alert from '@material-ui/lab/Alert';

export default class WorkUpload extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            files:[],
            filesOk:false,
            simulate:false,
            formDisabled:true,
            formFieldsData:[],
            errorText:'',
            error:false
        }

        this.onButtonClick = this.onButtonClick.bind(this)
    }
    componentDidMount(){
      Axios.get("https://localhost:44385/api/files?hash=" + this.props.match.params.hash)
      .then(res => {
        this.setState({formFieldsData:res.data})
        console.log(res.data)
      })
    }

  handleChange = (type,e) => {
    var input = document.getElementById("fileUpload");
    if(type === "camundaFile")
        if(input.files.length >= this.state.formFieldsData.filter(x => x['Key'] === "camundaFile")[0]['Value']['value']['Min']){
        this.setState({files:e.target.value})
        this.setState({filesOk:true})
        this.setState({formDisabled:false})
        this.setState({error:false})
        this.setState({errorText:""})
        }else{
            this.setState({filesOk:false})
            this.setState({error:true})
            this.setState({errorText:"You must upload " + this.state.formFieldsData.filter(x => x['Key'] === "camundaFile")[0]['Value']['value']['Min'] + " or more files"})
        }

        if(type === 'simulate')
            this.setState({simulate:!this.state.simulate})
  }

  onButtonClick(){
      var task = this.state.formFieldsData.filter(x => x['Key'] === "TaskId");
      var currentTask = task[0];

      var processDefinition = this.state.formFieldsData.filter(x => x['Key'] === "ProcessDefinitionId");
      var currentProcessDefinition = processDefinition[0];

      var processInstance = this.state.formFieldsData.filter(x => x['Key'] === "ProcessInstanceId");
      var currentProcessInstance = processInstance[0];

      var dto = new NewUserData(this.props.match.params.hash,currentTask['Value'],currentProcessDefinition['Value'],currentProcessInstance['Value'],null,null,this.state.simulate)
      console.log(JSON.stringify(dto))

      
      const config = {headers: {'content-type':'application/json'}}
      Axios.post("https://localhost:44385/api/files/complete",JSON.stringify(dto),config)
      .then(res =>{
        window.alert("File upload complete!");
      }).catch(error =>{
          console.log(error)
        window.alert("Something went wrong. Please activate your account and Please try again.");
      })
  }

  returnFieldRender(field){
      if(field['Key'] === "camundaFile")
      {
          if((field['Value']['value']) !==  null)
          {
            return (
                <Grid key={field['Key']} item xs={12}>
                    <Button
                    variant="contained"
                    component="label"
                    >
                    <input
                      id="fileUpload"
                        type="file"
                        multiple
                        onChange={(e) => this.handleChange(field['Key'],e)}
                    />
                    </Button>
              </Grid>)
          }
      }
  }

 render() {
  return ((
    <Container component="main" maxWidth="xs">
      <CssBaseline />
      <div >
        <Typography variant="h5">
          Upload samples of your work
        </Typography>
        <form  noValidate>
            {this.state.formFieldsData.map(field =>(
              this.returnFieldRender(field)
            ))}

          <Button
            type="Button"
            fullWidth
            variant="contained"
            color="primary"
            onClick={this.onButtonClick}
            disabled={this.state.formDisabled}
          >
            UPLOAD
          </Button>

          <Grid item xs={12}>
              <FormControlLabel
                control={<Checkbox value="writer" color="primary" />}
                label="Simulate 'Need more data?' "
                onChange={(e) => this.handleChange('simulate',e)}
              />
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