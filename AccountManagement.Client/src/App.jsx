import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Dashboard from "./pages/Dashboard";
import Settings from "./pages/Settings";
import PaymentSuccess from "./pages/PaymentSuccess";
import PaymentCancel from "./pages/PaymentCancel";
import "./App.css";
import "./index.css";


function App() {
  const token = localStorage.getItem("token");

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={ token ? (<Dashboard />) : (<Navigate to="/login" />) } />
        <Route path="/login" element={ token ? (<Navigate to="/" />) : (<Login />) }/>
        <Route  path="/register" element={ token ? (<Navigate to="/" />) : (<Register />)} />
        <Route path="/settings" element={ token ? (<Settings />) : (<Navigate to="/login" />) } />
        <Route path="/payment-success" element={ token ? (<PaymentSuccess />) : (<Navigate to="/login" />) } />
        <Route path="/payment-cancel" element={ token ? (<PaymentCancel />) : (<Navigate to="/login" />) } />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
