const API_BASE_URL = "http://localhost:5251/api"

export async function login(data) {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        throw new Error("Invalid username or password!");
    }

    return response.json();
}

export async function register(registerData) {
  const response = await fetch(`${API_BASE_URL}/clients/register`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(registerData),
  });

  const message = await response.text();

  if (!response.ok) {
    throw new Error(message || "Registration failed");
  }

  return message;
}