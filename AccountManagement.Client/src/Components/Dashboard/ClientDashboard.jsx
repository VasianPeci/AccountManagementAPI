function ClientDashboard({
  accounts,
  currencies,
  accountForm,
  setAccountForm,
  transactionForm,
  setTransactionForm,
  topUpForm,
  setTopUpForm,
  getCurrencyCode,
  createAccount,
  createTransaction,
  startStripeTopUp,
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
            onChange={(event) =>
              setAccountForm({ ...accountForm, code: event.target.value })
            }
          />

          <input
            type="text"
            placeholder="Account name"
            value={accountForm.name}
            onChange={(event) =>
              setAccountForm({ ...accountForm, name: event.target.value })
            }
          />

          <select
            value={accountForm.currencyId}
            onChange={(event) =>
              setAccountForm({ ...accountForm, currencyId: event.target.value })
            }
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
            onChange={(event) =>
              setAccountForm({ ...accountForm, balance: event.target.value })
            }
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
                onClick={() =>
                  setTransactionForm({ ...transactionForm, action: "0" })
                }
              >
                Deposit
              </button>
              <button
                type="button"
                className={transactionForm.action === "1" ? "active" : ""}
                onClick={() =>
                  setTransactionForm({ ...transactionForm, action: "1" })
                }
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
              onChange={(event) =>
                setTransactionForm({
                  ...transactionForm,
                  amount: event.target.value,
                })
              }
            />

            <button type="submit">
              {transactionForm.action === "0" ? "Deposit" : "Withdraw"}
            </button>
          </form>

          <form className="dashboard-form stripe-topup-form" onSubmit={startStripeTopUp}>
            <h3>Top Up With Stripe</h3>

            <input
              type="number"
              min="1"
              step="0.01"
              placeholder="Amount"
              value={topUpForm.amount}
              onChange={(event) =>
                setTopUpForm({ ...topUpForm, amount: event.target.value })
              }
            />

            <button type="submit">Pay With Stripe</button>
          </form>
        </div>
      )}
    </div>
  );
}

export default ClientDashboard;
