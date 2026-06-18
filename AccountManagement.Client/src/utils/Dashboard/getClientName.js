function getClientName(clients, clientId) {
  const client = clients.find((item) => item.id === clientId);

  if (!client) return "Unknown client";

  return (
    [client.firstName, client.lastName].filter(Boolean).join(" ") ||
    client.username
  );
}

export default getClientName;
