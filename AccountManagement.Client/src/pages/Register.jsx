import { useState } from "react";
import { register } from "../api/authApi";
import { useNavigate } from "react-router-dom";

function Register() {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        username: "",
        password: "",
        roles: ["Client"],
        firstName: "",
        lastName: "",
        birthdate: "",
        phone: "",
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

        if (formData.password.length < 6) {
            alert("Password must be at least 6 characters.");
            return false;
        }

        if (!formData.firstName.trim()) {
            alert("First name is required.");
            return false;
        }

        if (!formData.lastName.trim()) {
            alert("Last name is required.");
            return false;
        }

        if (!formData.birthdate) {
            alert("Birthdate is required.");
            return false;
        }

        if (!formData.phone.trim()) {
            alert("Phone is required.");
            return false;
        }

        return true;
    }

    async function handleSubmit(e) {
        e.preventDefault();

        if (!validateForm()) return;
        
        try {
            await register(formData);
            alert("Registration successful!");
            navigate("/login");
        } catch (error) {
            alert(error.message);
        }
    }

    return (
        <form onSubmit={handleSubmit}>
            <h1>Register</h1>

            <input type="email" placeholder="Email" value={formData.username} onChange={(e) => setFormData({...formData, username: e.target.value})}/>

            <input type="password" placeholder="Password" value={formData.password} onChange={(e) => setFormData({...formData, password: e.target.value})}/>

            <input type="text" placeholder="First name" value={formData.firstName} onChange={(e) => setFormData({...formData, firstName: e.target.value})}/>

            <input type="text" placeholder="Last name" value={formData.lastName} onChange={(e) => setFormData({...formData, lastName: e.target.value})}/>

            <input type="date" value={formData.birthdate} onChange={(e) => setFormData({...formData, birthdate: e.target.value})}/>

            <input type="text" placeholder="Phone" value={formData.phone} onChange={(e) => setFormData({...formData, phone: e.target.value})}/>

            <button type="submit">Register</button>

            <p className="auth-switch-text">Already have an account?</p>
            <button className="secondary-button" type="button" onClick={() => navigate("/login")}>
                Login
            </button>
        </form>
    );
}

export default Register;