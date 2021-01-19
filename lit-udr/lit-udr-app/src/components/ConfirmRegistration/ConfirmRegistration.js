import React from 'react';
import Axios from 'axios'

export default class ConfirmRegistration extends React.Component {
    constructor(props) {
        super(props);
        this.state = {}
    }

    componentDidMount(){
        Axios.get("https://localhost:44385/api/register/activate?hash=" + this.props.match.params.hash)
        .then(res => {
          window.alert("Registration activation success! If u are writer, please check email.");
        })
    }

 render() { return (<div></div>)} 
}
