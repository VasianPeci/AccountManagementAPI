import { getUserFromToken } from "../utils/jwtUtils";
import { deleteUser, getCurrentUser, getUsers } from "../api/userApi";
import {
  createBankAccount,
  createBankTransaction,
  deleteBankAccount,
  getBankAccounts,
  getClientAccounts,
  getCurrencies,
  updateBankAccount,
} from "../api/bankingApi";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

function accountEditValues(account) {
  return {
    code: account.code || "",
    name: account.name || "",
    balance: String(account.balance || 0),
    currencyId: account.currencyId || "",
    clientId: account.clientId || "",
    isActive: account.isActive,
  };
}

function Dashboard() {
  const navigate = useNavigate();
  const [tokenUser] = useState(getUserFromToken());
  const [user, setUser] = useState(null);
  const [currencies, setCurrencies] = useState([]);
  const [clientAccounts, setClientAccounts] = useState([]);
  const [clients, setClients] = useState([]);
  const [bankAccounts, setBankAccounts] = useState([]);
  const [accountEdits, setAccountEdits] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [clientAccountForm, setClientAccountForm] = useState({
    code: "",
    name: "",
    balance: "0",
    currencyId: "",
  });

  const [transactionForm, setTransactionForm] = useState({
    action: "0",
    amount: "",
  });

  const [adminAccountForm, setAdminAccountForm] = useState({
    code: "",
    name: "",
    balance: "0",
    currencyId: "",
    clientId: "",
  });

  const role = tokenUser?.role;
  const isClient = role === "Client";
  const isAdmin = role === "Admin";
  const isStaff = role === "Admin" || role === "Auditor";

  useEffect(() => {
    async function loadDashboard() {
      try {
        setLoading(true);
        setError("");

        const currentUser = await getCurrentUser(tokenUser);
        const currencyList = await getCurrencies();

        setUser(currentUser);
        setCurrencies(currencyList);
        setClientAccountForm((form) => ({
          ...form,
          currencyId: form.currencyId || currencyList[0]?.id || "",
        }));
        setAdminAccountForm((form) => ({
          ...form,
          currencyId: form.currencyId || currencyList[0]?.id || "",
        }));

        if (tokenUser?.role === "Client") {
          setClientAccounts(await getClientAccounts(currentUser.id));
        }

        if (tokenUser?.role === "Admin" || tokenUser?.role === "Auditor") {
          const clientList = await getUsers();
          const accountList = await getBankAccounts();
          const editValues = {};

          accountList.forEach((account) => {
            editValues[account.id] = accountEditValues(account);
          });

          setClients(clientList);
          setBankAccounts(accountList);
          setAccountEdits(editValues);
        }
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadDashboard();
  }, [tokenUser]);

  async function loadClientAccounts() {
    if (!user?.id) return;

    setClientAccounts(await getClientAccounts(user.id));
  }

  async function loadStaffData() {
    const clientList = await getUsers();
    const accountList = await getBankAccounts();
    const editValues = {};

    accountList.forEach((account) => {
      editValues[account.id] = accountEditValues(account);
    });

    setClients(clientList);
    setBankAccounts(accountList);
    setAccountEdits(editValues);
  }

  function getCurrencyCode(currencyId) {
    return currencies.find((currency) => currency.id === currencyId)?.code || "";
  }

  function getClientName(clientId) {
    const client = clients.find((item) => item.id === clientId);

    if (!client) return "Unknown client";

    return [client.firstName, client.lastName].filter(Boolean).join(" ") || client.username;
  }

  function resetSession() {
    localStorage.removeItem("token");
    window.location.href = "/login";
  }

  async function createClientAccount(event) {
    event.preventDefault();

    const balance = Number(clientAccountForm.balance);

    if (!clientAccountForm.code || !clientAccountForm.name || !clientAccountForm.currencyId) {
      alert("Fill all bank account fields.");
      return;
    }

    if (Number.isNaN(balance) || balance < 0) {
      alert("Balance must be zero or greater.");
      return;
    }

    try {
      await createBankAccount({
        code: clientAccountForm.code,
        name: clientAccountForm.name,
        balance,
        isActive: true,
        currencyId: clientAccountForm.currencyId,
        clientId: user.id,
      });

      await loadClientAccounts();
      setClientAccountForm({
        code: "",
        name: "",
        balance: "0",
        currencyId: clientAccountForm.currencyId,
      });
    } catch (err) {
      setError(err.message);
    }
  }

  async function createTransaction(event) {
    event.preventDefault();

    const account = clientAccounts[0];
    const amount = Number(transactionForm.amount);

    if (!account) {
      alert("Bank account was not loaded.");
      return;
    }

    if (Number.isNaN(amount) || amount <= 0) {
      alert("Amount must be greater than zero.");
      return;
    }

    if (transactionForm.action === "1" && amount > Number(account.balance)) {
      alert("Withdrawal amount cannot exceed the current balance.");
      return;
    }

    try {
      await createBankTransaction({
        bankAccountId: account.id,
        action: Number(transactionForm.action),
        amount,
        isActive: true,
      });

      await loadClientAccounts();
      setTransactionForm({ ...transactionForm, amount: "" });
    } catch (err) {
      setError(err.message);
    }
  }

  async function createAdminAccount(event) {
    event.preventDefault();

    const balance = Number(adminAccountForm.balance);

    if (!adminAccountForm.clientId || !adminAccountForm.currencyId || !adminAccountForm.code || !adminAccountForm.name) {
      alert("Fill all account fields.");
      return;
    }

    if (Number.isNaN(balance) || balance < 0) {
      alert("Balance must be zero or greater.");
      return;
    }

    try {
      await createBankAccount({
        code: adminAccountForm.code,
        name: adminAccountForm.name,
        balance,
        isActive: true,
        currencyId: adminAccountForm.currencyId,
        clientId: adminAccountForm.clientId,
      });

      await loadStaffData();
      setAdminAccountForm({
        code: "",
        name: "",
        balance: "0",
        currencyId: adminAccountForm.currencyId,
        clientId: "",
      });
    } catch (err) {
      setError(err.message);
    }
  }

  function changeAccountEdit(accountId, field, value) {
    setAccountEdits({
      ...accountEdits,
      [accountId]: {
        ...accountEdits[accountId],
        [field]: value,
      },
    });
  }

  async function saveAccount(account) {
    const edit = accountEdits[account.id];
    const balance = Number(edit.balance);

    if (!edit.code || !edit.name || !edit.currencyId || !edit.clientId) {
      alert("Fill all account fields.");
      return;
    }

    if (Number.isNaN(balance) || balance < 0) {
      alert("Balance must be zero or greater.");
      return;
    }

    try {
      await updateBankAccount(account.id, {
        code: edit.code,
        name: edit.name,
        balance,
        isActive: edit.isActive,
        currencyId: edit.currencyId,
        clientId: edit.clientId,
      });

      await loadStaffData();
    } catch (err) {
      setError(err.message);
    }
  }

  async function removeAccount(accountId) {
    if (!confirm("Delete this bank account?")) return;

    try {
      await deleteBankAccount(accountId);
      await loadStaffData();
    } catch (err) {
      setError(err.message);
    }
  }

  async function removeClient(clientId) {
    if (!confirm("Delete this client?")) return;

    try {
      await deleteUser(clientId, false);
      await loadStaffData();
    } catch (err) {
      setError(err.message);
    }
  }

  const displayName = [user?.firstName, user?.lastName].filter(Boolean).join(" ");

  return (
    <div className="dashboard">
      <h1>{loading ? "Loading dashboard..." : `Welcome ${displayName}`}</h1>

      {error && <p className="error-message">{error}</p>}

      {isClient && (
        <ClientDashboard
          accounts={clientAccounts}
          currencies={currencies}
          accountForm={clientAccountForm}
          setAccountForm={setClientAccountForm}
          transactionForm={transactionForm}
          setTransactionForm={setTransactionForm}
          getCurrencyCode={getCurrencyCode}
          createAccount={createClientAccount}
          createTransaction={createTransaction}
        />
      )}

      {isStaff && (
        <StaffDashboard
          isAdmin={isAdmin}
          clients={clients}
          accounts={bankAccounts}
          currencies={currencies}
          adminAccountForm={adminAccountForm}
          setAdminAccountForm={setAdminAccountForm}
          accountEdits={accountEdits}
          changeAccountEdit={changeAccountEdit}
          getCurrencyCode={getCurrencyCode}
          getClientName={getClientName}
          createAccount={createAdminAccount}
          saveAccount={saveAccount}
          removeAccount={removeAccount}
          removeClient={removeClient}
        />
      )}

      <div className="dashboard-actions">
        <button className="secondary-button" onClick={() => navigate("/settings")}>
          Settings
        </button>

        <button className="danger-button" onClick={resetSession}>
          Logout
        </button>
      </div>
    </div>
  );
}

function ClientDashboard({
  accounts,
  currencies,
  accountForm,
  setAccountForm,
  transactionForm,
  setTransactionForm,
  getCurrencyCode,
  createAccount,
  createTransaction,
}) {
  const account = accounts[0];

  return (
    <div className="client-banking-panel">
      <h2>Client Dashboard</h2>

      {!account && (
        <form className="dashboard-form" onSubmit={createAccount}>
          <h3>Add Bank Account</h3>

          <input
            type="text"
            placeholder="Account code"
            value={accountForm.code}
            onChange={(event) => setAccountForm({ ...accountForm, code: event.target.value })}
          />

          <input
            type="text"
            placeholder="Account name"
            value={accountForm.name}
            onChange={(event) => setAccountForm({ ...accountForm, name: event.target.value })}
          />

          <select
            value={accountForm.currencyId}
            onChange={(event) => setAccountForm({ ...accountForm, currencyId: event.target.value })}
          >
            <option value="">Select currency</option>
            {currencies.map((currency) => (
              <option key={currency.id} value={currency.id}>
                {currency.code} - {currency.description}
              </option>
            ))}
          </select>

          <input
            type="number"
            min="0"
            step="0.01"
            placeholder="Initial balance"
            value={accountForm.balance}
            onChange={(event) => setAccountForm({ ...accountForm, balance: event.target.value })}
          />

          <button type="submit">Create Account</button>
        </form>
      )}

      {account && (
        <div className="account-panel">
          <div className="account-stats">
            <div>
              <span>Account</span>
              <strong>{account.name}</strong>
            </div>
            <div>
              <span>Code</span>
              <strong>{account.code}</strong>
            </div>
            <div>
              <span>Currency</span>
              <strong>{getCurrencyCode(account.currencyId)}</strong>
            </div>
            <div>
              <span>Balance</span>
              <strong>{Number(account.balance).toFixed(2)}</strong>
            </div>
          </div>

          <form className="dashboard-form" onSubmit={createTransaction}>
            <h3>Move Money</h3>

            <div className="transaction-mode">
              <button
                type="button"
                className={transactionForm.action === "0" ? "active" : ""}
                onClick={() => setTransactionForm({ ...transactionForm, action: "0" })}
              >
                Deposit
              </button>
              <button
                type="button"
                className={transactionForm.action === "1" ? "active" : ""}
                onClick={() => setTransactionForm({ ...transactionForm, action: "1" })}
              >
                Withdraw
              </button>
            </div>

            <input
              type="number"
              min="0.01"
              step="0.01"
              placeholder="Amount"
              value={transactionForm.amount}
              onChange={(event) => setTransactionForm({ ...transactionForm, amount: event.target.value })}
            />

            <button type="submit">
              {transactionForm.action === "0" ? "Deposit" : "Withdraw"}
            </button>
          </form>
        </div>
      )}
    </div>
  );
}

function StaffDashboard({
  isAdmin,
  clients,
  accounts,
  currencies,
  adminAccountForm,
  setAdminAccountForm,
  accountEdits,
  changeAccountEdit,
  getCurrencyCode,
  getClientName,
  createAccount,
  saveAccount,
  removeAccount,
  removeClient,
}) {
  const activeAccounts = accounts.filter((account) => account.isActive);
  const totalBalance = activeAccounts.reduce((sum, account) => sum + Number(account.balance), 0);
  const clientsWithoutAccount = clients.filter((client) => {
    const roles = client.roles?.length ? client.roles : ["Client"];
    const hasAccount = activeAccounts.some((account) => account.clientId === client.id);

    return roles.includes("Client") && !hasAccount;
  });

  function getClientBalance(clientId) {
    return accounts
      .filter((account) => account.clientId === clientId)
      .reduce((sum, account) => sum + Number(account.balance), 0);
  }

  return (
    <div className="staff-dashboard-panel">
      <h2>{isAdmin ? "Admin Dashboard" : "Auditor Dashboard"}</h2>

      <div className="staff-stats">
        <div>
          <span>Clients</span>
          <strong>{clients.length}</strong>
        </div>
        <div>
          <span>Bank Accounts</span>
          <strong>{activeAccounts.length}</strong>
        </div>
        <div>
          <span>Total Balance</span>
          <strong>{totalBalance.toFixed(2)}</strong>
        </div>
      </div>

      {isAdmin && (
        <form className="dashboard-form" onSubmit={createAccount}>
          <h3>Create Bank Account</h3>

          <select
            value={adminAccountForm.clientId}
            onChange={(event) => setAdminAccountForm({ ...adminAccountForm, clientId: event.target.value })}
          >
            <option value="">Select client</option>
            {clientsWithoutAccount.map((client) => (
              <option key={client.id} value={client.id}>
                {[client.firstName, client.lastName].filter(Boolean).join(" ")} - {client.username}
              </option>
            ))}
          </select>

          <input
            type="text"
            placeholder="Account code"
            value={adminAccountForm.code}
            onChange={(event) => setAdminAccountForm({ ...adminAccountForm, code: event.target.value })}
          />

          <input
            type="text"
            placeholder="Account name"
            value={adminAccountForm.name}
            onChange={(event) => setAdminAccountForm({ ...adminAccountForm, name: event.target.value })}
          />

          <select
            value={adminAccountForm.currencyId}
            onChange={(event) => setAdminAccountForm({ ...adminAccountForm, currencyId: event.target.value })}
          >
            <option value="">Select currency</option>
            {currencies.map((currency) => (
              <option key={currency.id} value={currency.id}>
                {currency.code} - {currency.description}
              </option>
            ))}
          </select>

          <input
            type="number"
            min="0"
            step="0.01"
            placeholder="Balance"
            value={adminAccountForm.balance}
            onChange={(event) => setAdminAccountForm({ ...adminAccountForm, balance: event.target.value })}
          />

          <button type="submit">Create Account</button>
        </form>
      )}

      <div className="staff-section">
        <h3>Clients</h3>
        <div className="staff-list">
          {clients.map((client) => (
            <div className="staff-row" key={client.id}>
              <div>
                <span>Name</span>
                <strong>{[client.firstName, client.lastName].filter(Boolean).join(" ")}</strong>
              </div>
              <div>
                <span>Email</span>
                <strong>{client.username}</strong>
              </div>
              <div>
                <span>Role</span>
                <strong>{client.roles?.join(", ") || "Client"}</strong>
              </div>
              <div>
                <span>Accounts</span>
                <strong>{accounts.filter((account) => account.clientId === client.id).length}</strong>
              </div>
              <div>
                <span>Balance</span>
                <strong>{getClientBalance(client.id).toFixed(2)}</strong>
              </div>
              {isAdmin && (
                <button className="danger-button" onClick={() => removeClient(client.id)}>
                  Delete Client
                </button>
              )}
            </div>
          ))}
        </div>
      </div>

      <div className="staff-section">
        <h3>Bank Accounts</h3>
        <div className="staff-list">
          {accounts.map((account) => {
            const edit = accountEdits[account.id] || accountEditValues(account);

            return (
              <div className="staff-row account-management-row" key={account.id}>
                <div>
                  <span>Client</span>
                  <strong>{getClientName(account.clientId)}</strong>
                </div>

                {isAdmin ? (
                  <>
                    <input
                      type="text"
                      value={edit.code}
                      onChange={(event) => changeAccountEdit(account.id, "code", event.target.value)}
                    />
                    <input
                      type="text"
                      value={edit.name}
                      onChange={(event) => changeAccountEdit(account.id, "name", event.target.value)}
                    />
                    <input
                      type="number"
                      min="0"
                      step="0.01"
                      value={edit.balance}
                      onChange={(event) => changeAccountEdit(account.id, "balance", event.target.value)}
                    />
                    <select
                      value={edit.currencyId}
                      onChange={(event) => changeAccountEdit(account.id, "currencyId", event.target.value)}
                    >
                      {currencies.map((currency) => (
                        <option key={currency.id} value={currency.id}>
                          {currency.code}
                        </option>
                      ))}
                    </select>
                    <label className="checkbox-field">
                      <input
                        type="checkbox"
                        checked={edit.isActive}
                        onChange={(event) => changeAccountEdit(account.id, "isActive", event.target.checked)}
                      />
                      Active
                    </label>
                    <button type="button" onClick={() => saveAccount(account)}>
                      Update
                    </button>
                    <button className="danger-button" type="button" onClick={() => removeAccount(account.id)}>
                      Delete
                    </button>
                  </>
                ) : (
                  <>
                    <div>
                      <span>Code</span>
                      <strong>{account.code}</strong>
                    </div>
                    <div>
                      <span>Name</span>
                      <strong>{account.name}</strong>
                    </div>
                    <div>
                      <span>Currency</span>
                      <strong>{getCurrencyCode(account.currencyId)}</strong>
                    </div>
                    <div>
                      <span>Balance</span>
                      <strong>{Number(account.balance).toFixed(2)}</strong>
                    </div>
                  </>
                )}
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
