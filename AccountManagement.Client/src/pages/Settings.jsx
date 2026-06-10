import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { getUserFromToken } from "../utils/jwtUtils";
import { deleteUser, getCurrentUser, updateUser } from "../api/userApi";

function Settings() {
  const navigate = useNavigate();
  const [tokenUser] = useState(getUserFromToken());
  const [currentUser, setCurrentUser] = useState(null);
  const [message, setMessage] = useState("");
  const [formData, setFormData] = useState({
    username: "",
    password: "",
    firstName: "",
    lastName: "",
    phone: "",
  });

  useEffect(() => {
    async function loadUser() {
      try {
        const user = await getCurrentUser(tokenUser);

        setCurrentUser(user);
        setFormData({
          username: user.username || "",
          password: "",
          firstName: user.firstName || "",
          lastName: user.lastName || "",
          phone: user.phone || "",
        });
      } catch (error) {
        setMessage(error.message);
      }
    }

    loadUser();
  }, [tokenUser]);

  function validateForm() {
    if (formData.username && !formData.username.includes("@")) {
      alert("Invalid email format.");
      return false;
    }

    if (formData.password && formData.password.length < 8) {
      alert("Password must be at least 8 characters.");
      return false;
    }

    if (formData.firstName && formData.firstName.length < 2) {
      alert("First name must be at least 2 characters.");
      return false;
    }

    if (formData.lastName && formData.lastName.length < 2) {
      alert("Last name must be at least 2 characters.");
      return false;
    }

    return true;
  }

  async function handleUpdate(e) {
    e.preventDefault();

    if (!currentUser?.id || !validateForm()) return;

    const updateData = {};

    Object.entries(formData).forEach(([key, value]) => {
      if (value.trim()) updateData[key] = value.trim();
    });

    if (!Object.keys(updateData).length) {
      alert("Fill at least one field to update.");
      return;
    }

    try {
      await updateUser(currentUser.id, updateData);
      alert("User updated successfully.");
      navigate("/");
    } catch (error) {
      alert(error.message);
    }
  }

  async function handleDelete() {
    if (!currentUser?.id || !confirm("Are you sure you want to delete your account?")) return;

    try {
      await deleteUser(currentUser.id);
      alert("Account deleted successfully.");
      window.location.href = "/login";
    } catch (error) {
      alert(error.message);
    }
  }

  return (
    <form onSubmit={handleUpdate}>
      <h1>Settings</h1>

      {message && <p className="error-message">{message}</p>}

      <input
        type="email"
        placeholder="New email"
        value={formData.username}
        onChange={(e) => setFormData({ ...formData, username: e.target.value })}
      />

      <input
        type="password"
        placeholder="New password"
        value={formData.password}
        minLength={8}
        onChange={(e) => setFormData({ ...formData, password: e.target.value })}
      />

      <input
        type="text"
        placeholder="First name"
        value={formData.firstName}
        minLength={2}
        maxLength={50}
        onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
      />

      <input
        type="text"
        placeholder="Last name"
        value={formData.lastName}
        minLength={2}
        maxLength={50}
        onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
      />

      <input
        type="tel"
        placeholder="Phone"
        value={formData.phone}
        onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
      />

      <button type="submit">Update Account</button>

      <button type="button" className="danger-button" onClick={handleDelete}>
        Delete Account
      </button>

      <button type="button" className="secondary-button" onClick={() => navigate("/")}>
        Back to Dashboard
      </button>
    </form>
  );
}

export default Settings;
