import React from 'react';
import Axios from 'axios';


export default class Payment extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
        };
    }

    componentDidMount(){
            console.log("a")
            Axios.get("https://localhost:44385/api/review/activate?hash=" + this.props.match.params.hash)
            .then(res => {
                window.alert("Payment simulation success!")
            }).catch(error =>{
                window.alert("Something went wrong!")
            })
        }

    render() {
    return (
            <div></div>
    )
    }
}
     
