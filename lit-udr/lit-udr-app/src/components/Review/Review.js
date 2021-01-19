import React from 'react';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import Axios from 'axios';
import ReviewDto from '../../model/ReviewDto';
import { TextField } from '@material-ui/core';


export default class Review extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            data:[],
            comment:''

        }

        this.onButtonClick = this.onButtonClick.bind(this)
    }

    componentDidMount(){
        const data = JSON.parse(localStorage.getItem('data'))

        Axios.get("https://localhost:44385/api/review?Token=" + data)
        .then(res =>{
            this.setState({data:res.data})
            window.alert("You have " + this.state.data.length + " Writer to review")
        }).catch(error =>{
            window.alert("Something went wrong. Please try again.");
        })
    }

  onButtonClick(result){
    const data = JSON.parse(localStorage.getItem('data'))
    var dto = new ReviewDto(data,this.state.data[0]['Id'],result,this.state.data[0]['processDefinitionId'],this.state.data[0]['processInstanceId'],this.state.comment)

    const config = {headers: {'content-type':'application/json'}}
    Axios.post("https://localhost:44385/api/review",JSON.stringify(dto),config)
    .then(res =>{
        this.setState({data:[]})
    }).catch(error =>{
        window.alert("Something went wrong. Please try again.");
    })
  }

  handleChange(e){
      this.setState({comment:e.target.value})
  }

 render() {
    const data = this.state.data 
    if(data.length !== 0){
    return (
            <Container component="main" maxWidth="xs">
            <CssBaseline />
            <div >
                <Typography variant="h5">
                Please review this writer
                </Typography>
                <form  noValidate>
                <Grid container spacing={2}>
                    <Grid item xs={12}>
                    <Button fullWidth target="_blank" variant="contained" href="https://localhost:44385/LiterarnoUdruzenje.pdf">Link To Work</Button>
                    </Grid>
                    <Grid item xs={12}>
                    <Button fullWidth target="_blank" variant="contained" href="https://localhost:44385/LiterarnoUdruzenje.pdf">Link To Work</Button>
                    </Grid>
                    </Grid>

                    <Grid container spacing={2}>
                    <Grid item xs={12}>
                    <Button
                        type="Button"
                        fullWidth
                        variant="contained"
                        color="primary"
                        onClick={(e) => this.onButtonClick('approve',e)}
                    >
                        Approve
                    </Button>
                    </Grid>

                    <Grid item xs={12}>
                    <Button
                        type="Button"
                        fullWidth
                        variant="contained"
                        onClick={(e) => this.onButtonClick('needMoreData',e)}
                    >
                        Need More Work
                    </Button>
                    </Grid>

                    <Grid item xs={12}>
                    <Button
                        type="Button"
                        fullWidth
                        variant="contained"
                        color="secondary"
                        onClick={(e) => this.onButtonClick('decline',e)}
                    >
                        Decline
                    </Button>
                    </Grid>

                    <Grid item xs={12}>
                    <TextField
                        placeholder="Want to leave your comment about work?"
                        fullWidth
                        multiline
                        onChange={(e) => this.handleChange(e)}
                        variant="filled"
        />
                    </Grid>
                </Grid>

                </form>
            </div>
            </Container>)
    }else{
        return(<div></div>)
    }
 }
}
     
