function resetSession() {
  localStorage.removeItem("token");
  window.location.href = "/login";
}

export default resetSession;
