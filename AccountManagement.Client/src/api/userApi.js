const API_URL = "http://localhost:5251/api/Clients";

function getHeaders(headers = {}) {
  const token = localStorage.getItem("token");

  if (!token) return headers;

  return { ...headers, Authorization: `Bearer ${token}` };
}

async function getResponse(response, message) {
  const text = await response.text();

  if (!response.ok) {
    throw new Error(text || message);
  }

  return text ? JSON.parse(text) : null;
}

function formatUser(user) {
  if (!user) return null;

  return {
    ...user,
    id: user.id ?? user.Id,
    username: user.username ?? user.userName ?? user.Username,
    roles: user.roles ?? user.Roles ?? [],
    firstName: user.firstName ?? user.FirstName,
    lastName: user.lastName ?? user.LastName,
    birthdate: user.birthdate ?? user.Birthdate,
    phone: user.phone ?? user.Phone,
  };
}

export async function getUserById(id) {
  const response = await fetch(`${API_URL}/${id}`, {
    headers: getHeaders(),
  });

  return formatUser(await getResponse(response, "Failed to load user"));
}

export async function getUsers() {
  const response = await fetch(API_URL, {
    headers: getHeaders(),
  });

  const users = await getResponse(response, "Failed to load users");

  return users.map(formatUser);
}

export async function getCurrentUser(tokenUser) {
  const id = tokenUser?.clientId || tokenUser?.id;

  if (!id) {
    throw new Error("User ID not found in token.");
  }

  return getUserById(id);
}

export async function updateUser(id, userData) {
  const response = await fetch(`${API_URL}/${id}`, {
    method: "POST",
    headers: getHeaders({
      "Content-Type": "application/json",
    }),
    body: JSON.stringify(userData),
  });

  return formatUser(await getResponse(response, "Update failed"));
}

export async function deleteUser(id, clearToken = true) {
  const response = await fetch(`${API_URL}/${id}`, {
    method: "DELETE",
    headers: getHeaders(),
  });

  const result = await getResponse(response, "Delete failed");

  if (clearToken) {
    localStorage.removeItem("token");
  }

  return result;
}
