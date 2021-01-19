import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import SignIn from './components/Signin/Signin';
import reportWebVitals from './reportWebVitals';
import SignUp from './components/Singup/Signup';
import WorkUpload from './components/WorkUpload/WorkUpload';
import ConfirmRegistration from './components/ConfirmRegistration/ConfirmRegistration'
import {
  BrowserRouter as Router,
  Route
} from "react-router-dom";
import ForgotPassword from './components/ForgotPassword/ForgotPassword';
import App from './components/App/App';
import Copyright from './components/Copyright/Copyright';
import Review from './components/Review/Review';
import Payment from './components/Payment/Payment';


ReactDOM.render(
  <Router>

  <Route path="/" component={App} />
 
  <Route path="/signup" component={SignUp} />
  <Route path="/signin" component={SignIn} />
  <Route path="/forgotPassword" component={ForgotPassword} />
  <Route path="/workUpload/:hash" component={WorkUpload} />
  <Route path="/confirmRegistration/:hash" component={ConfirmRegistration} />
  <Route path="/confirmSubscription/:hash" component={Payment} />

  
  <Route path="/review" component={Review} />
  <Route path="/" component={Copyright} />
 
  </Router>,
  document.getElementById('root')
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
