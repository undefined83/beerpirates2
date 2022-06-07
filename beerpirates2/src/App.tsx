import React, { useState, useEffect } from 'react';
import logo from './logo.svg';
import './App.css';
import { isDisabled } from '@testing-library/user-event/dist/utils';

function App() {

  const [message, setMessage] = React.useState('');
  const [productName, setProductName] = React.useState('');
  const [productBrand, setProductBrand] = React.useState('');
  const [productCategory, setProductCategory] = React.useState('');
  const [productPrice, setProductPrice] = React.useState('');

  const isDisabled = true;

  const getDataFromApi = async(e: any)=>{

    e.preventDefault();
    const data = await fetch(`/api/beerrecommendations`);
    const json = await data.json();
    if (json[0]){
      setProductName(json[0].ProductName);
      setProductBrand(json[0].ProductBrand);
      setProductPrice(json[0].ProductPrice);
      setProductCategory(json[0].ProductCategory);
      var bigString = `${json[0].ProductBrand} Brings you a new ${json[0].ProductCategory}, ${json[0].ProductName} at a deal price of \$${json[0].ProductPrice}`;
      setMessage(bigString);
    }
  };

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Welcome to Beer Pirates Inc. Online Shop
        </p>
        <form id="form1" className="App-form" onSubmit={e => getDataFromApi(e)}>
          <div>
            <button type="submit" className="App-button">Get Beer Recommendation</button>
          </div>
        </form>
        <div id = "reposnseDiv">
            <h5>Recommended Beer:</h5>
            <h4>{message}</h4>
        </div>
      </header>
    </div>
  );
}

export default App;