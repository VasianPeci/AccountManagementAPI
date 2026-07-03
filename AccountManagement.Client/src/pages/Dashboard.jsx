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
import StaffDashboard from "../Components/Dashboard/StaffDashboard";
import ClientDashboard from "../Components/Dashboard/ClientDashboard";
import resetSession from "../utils/resetSession";
import getClientName from "../utils/Dashboard/getClientName";
import getCurrencyCode from "../utils/Dashboard/getCurrencyCode";
import { createTopUpSession } from "../api/paymentApi";

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
  const [topUpForm, setTopUpForm] = useState({
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

  async function createClientAccount(event) {
    event.preventDefault();

    const balance = Number(clientAccountForm.balance);

    if (
      !clientAccountForm.code ||
      !clientAccountForm.name ||
      !clientAccountForm.currencyId
    ) {
      alert("Fill all bank account fields.");
      return;
    }

    if (Number.isNaN(balance) || balance < 0) {
      alert("Balance must be zero or greater.");
      return;
    }

    try {
      setError("");
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
      setError("");
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

  async function startStripeTopUp(event) {
    event.preventDefault();

    const account = clientAccounts[0];
    const amount = Number(topUpForm.amount);

    if (!account) {
      alert("Bank account was not loaded.");
      return;
    }

    if (Number.isNaN(amount) || amount <= 0) {
      alert("Amount must be greater than zero.");
      return;
    }

    try {
      setError("");
      const session = await createTopUpSession({
        bankAccountId: account.id,
        currencyId: account.currencyId,
        amount,
      });

      const checkoutUrl = session.url || session.Url;

      if (!checkoutUrl) {
        throw new Error("Stripe checkout URL was not returned.");
      }

      window.location.href = checkoutUrl;
    } catch (err) {
      setError(err.message);
    }
  }

  async function createAdminAccount(event) {
    event.preventDefault();

    const balance = Number(adminAccountForm.balance);

    if (
      !adminAccountForm.clientId ||
      !adminAccountForm.currencyId ||
      !adminAccountForm.code ||
      !adminAccountForm.name
    ) {
      alert("Fill all account fields.");
      return;
    }

    if (Number.isNaN(balance) || balance < 0) {
      alert("Balance must be zero or greater.");
      return;
    }

    try {
      setError("");
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
      setError("");
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
      setError("");
      await deleteBankAccount(accountId);
      await loadStaffData();
    } catch (err) {
      setError(err.message);
    }
  }

  async function removeClient(clientId) {
    if (!confirm("Delete this client?")) return;

    try {
      setError("");
      await deleteUser(clientId, false);
      await loadStaffData();
    } catch (err) {
      setError(err.message);
    }
  }

  const displayName = [user?.firstName, user?.lastName]
    .filter(Boolean)
    .join(" ");

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
          topUpForm={topUpForm}
          setTopUpForm={setTopUpForm}
          getCurrencyCode={(currencyId) => getCurrencyCode(currencies, currencyId)}
          createAccount={createClientAccount}
          createTransaction={createTransaction}
          startStripeTopUp={startStripeTopUp}
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
          getCurrencyCode={(currencyId) => getCurrencyCode(currencies, currencyId)}
          getClientName={(clientId) => getClientName(clients, clientId)}
          createAccount={createAdminAccount}
          saveAccount={saveAccount}
          removeAccount={removeAccount}
          removeClient={removeClient}
          accountEditValues={accountEditValues}
        />
      )}

      <div className="dashboard-actions">
        <button
          className="secondary-button"
          onClick={() => navigate("/settings")}
        >
          Settings
        </button>

        <button className="danger-button" onClick={resetSession}>
          Logout
        </button>
      </div>
    </div>
  );
}

export default Dashboard;
