const API_URL = "http://localhost:5251/api";

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

function formatAccount(account) {
  const id = account.id ?? account.accountId;
  const code = account.code ?? account.accountCode;
  const name = account.name ?? account.accountName;
  const balance = account.balance ?? account.currentBalance ?? 0;
  const currencyId = account.currencyId ?? account.currency;

  return {
    ...account,
    id,
    code,
    name,
    balance,
    currencyId,
    clientId: account.clientId,
    isActive: account.isActive ?? true,
  };
}

function formatCurrency(currency) {
  return {
    ...currency,
    id: currency.id ?? currency.Id,
    code: currency.code ?? currency.Code,
    description: currency.description ?? currency.Description,
  };
}

export async function getClientAccounts(clientId) {
  const response = await fetch(`${API_URL}/Reports/clients/${clientId}/accounts`, {
    headers: getHeaders(),
  });

  const accounts = await getResponse(response, "Failed to load bank accounts");

  return accounts.map(formatAccount);
}

export async function getBankAccounts() {
  const response = await fetch(`${API_URL}/BankAccounts`, {
    headers: getHeaders(),
  });

  const accounts = await getResponse(response, "Failed to load bank accounts");

  return accounts.map(formatAccount);
}

export async function getCurrencies() {
  const response = await fetch(`${API_URL}/Currencies`, {
    headers: getHeaders(),
  });

  const currencies = await getResponse(response, "Failed to load currencies");

  return currencies.map(formatCurrency);
}

export async function createBankAccount(data) {
  const response = await fetch(`${API_URL}/BankAccounts`, {
    method: "POST",
    headers: getHeaders({
      "Content-Type": "application/json",
    }),
    body: JSON.stringify(data),
  });

  return getResponse(response, "Failed to create bank account");
}

export async function updateBankAccount(id, data) {
  const response = await fetch(`${API_URL}/BankAccounts/${id}`, {
    method: "PUT",
    headers: getHeaders({
      "Content-Type": "application/json",
    }),
    body: JSON.stringify(data),
  });

  return getResponse(response, "Failed to update bank account");
}

export async function deleteBankAccount(id) {
  const response = await fetch(`${API_URL}/BankAccounts/${id}`, {
    method: "DELETE",
    headers: getHeaders(),
  });

  return getResponse(response, "Failed to delete bank account");
}

export async function createBankTransaction(data) {
  const response = await fetch(`${API_URL}/BankTransactions`, {
    method: "POST",
    headers: getHeaders({
      "Content-Type": "application/json",
    }),
    body: JSON.stringify(data),
  });

  return getResponse(response, "Failed to create transaction");
}
