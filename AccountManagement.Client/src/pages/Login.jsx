import { useState } from "react";
import { login } from "../api/authApi";
import { useNavigate } from "react-router-dom";

function Login() {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        username: "",
        password: "",
    });

    function validateForm() {
        if (!formData.username.trim()) {
            alert("Email is required.");
            return false;
        }

        if (!formData.password.trim()) {
            alert("Password is required.");
            return false;
        }

        return true;
    }

    async function handleSubmit(e) {
        e.preventDefault();

        if (!validateForm()) return;

        try {
            const result = await login(formData);

            localStorage.setItem("token", result.jwtToken);

            alert("Login Successful!");
            window.location.href = "/";
        } catch (error) {
            alert(error.message);
        }
    }

    return (
        <form onSubmit={handleSubmit}>
            <h1>Login</h1>

            <input type="email" placeholder="Email" value={formData.username} onChange={(e) => setFormData({...formData, username: e.target.value})}/>

            <input type="password" placeholder="Password" value={formData.password} onChange={(e) => setFormData({...formData, password: e.target.value})}/>

            <button type="submit">Login</button>

            <p className="auth-switch-text">Don't have an account?</p>
            <button className="secondary-button" type="button" onClick={() => navigate("/register")}>
                Register
            </button>
        </form>
    );
}

export default Login;
