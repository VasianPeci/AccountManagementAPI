const API_URL = "http://localhost:5251/api/Payments";

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

export async function createTopUpSession(data) {
  const response = await fetch(`${API_URL}/topup`, {
    method: "POST",
    headers: getHeaders({
      "Content-Type": "application/json",
    }),
    body: JSON.stringify(data),
  });

  return getResponse(response, "Failed to start Stripe top up");
}

export async function confirmTopUp(sessionId) {
  const response = await fetch(`${API_URL}/confirm`, {
    method: "POST",
    headers: getHeaders({
      "Content-Type": "application/json",
    }),
    body: JSON.stringify({ sessionId }),
  });

  return getResponse(response, "Failed to confirm Stripe payment");
}
