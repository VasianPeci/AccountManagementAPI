function validateForm(formData) {
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

export default validateForm;