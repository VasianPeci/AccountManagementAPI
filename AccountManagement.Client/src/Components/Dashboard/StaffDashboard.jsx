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
  accountEditValues,
}) {
  const activeAccounts = accounts.filter((account) => account.isActive);
  const totalBalance = activeAccounts.reduce(
    (sum, account) => sum + Number(account.balance),
    0,
  );
  const clientsWithoutAccount = clients.filter((client) => {
    const roles = client.roles?.length ? client.roles : ["Client"];
    const hasAccount = activeAccounts.some(
      (account) => account.clientId === client.id,
    );

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
            onChange={(event) =>
              setAdminAccountForm({
                ...adminAccountForm,
                clientId: event.target.value,
              })
            }
          >
            <option value="">Select client</option>
            {clientsWithoutAccount.map((client) => (
              <option key={client.id} value={client.id}>
                {[client.firstName, client.lastName].filter(Boolean).join(" ")}{" "}
                - {client.username}
              </option>
            ))}
          </select>

          <input
            type="text"
            placeholder="Account code"
            value={adminAccountForm.code}
            onChange={(event) =>
              setAdminAccountForm({
                ...adminAccountForm,
                code: event.target.value,
              })
            }
          />

          <input
            type="text"
            placeholder="Account name"
            value={adminAccountForm.name}
            onChange={(event) =>
              setAdminAccountForm({
                ...adminAccountForm,
                name: event.target.value,
              })
            }
          />

          <select
            value={adminAccountForm.currencyId}
            onChange={(event) =>
              setAdminAccountForm({
                ...adminAccountForm,
                currencyId: event.target.value,
              })
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
            placeholder="Balance"
            value={adminAccountForm.balance}
            onChange={(event) =>
              setAdminAccountForm({
                ...adminAccountForm,
                balance: event.target.value,
              })
            }
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
                <strong>
                  {[client.firstName, client.lastName]
                    .filter(Boolean)
                    .join(" ")}
                </strong>
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
                <strong>
                  {
                    accounts.filter((account) => account.clientId === client.id)
                      .length
                  }
                </strong>
              </div>
              <div>
                <span>Balance</span>
                <strong>{getClientBalance(client.id).toFixed(2)}</strong>
              </div>
              {isAdmin && (
                <button
                  className="danger-button"
                  onClick={() => removeClient(client.id)}
                >
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
              <div
                className="staff-row account-management-row"
                key={account.id}
              >
                <div>
                  <span>Client</span>
                  <strong>{getClientName(account.clientId)}</strong>
                </div>

                {isAdmin ? (
                  <>
                    <input
                      type="text"
                      value={edit.code}
                      onChange={(event) =>
                        changeAccountEdit(
                          account.id,
                          "code",
                          event.target.value,
                        )
                      }
                    />
                    <input
                      type="text"
                      value={edit.name}
                      onChange={(event) =>
                        changeAccountEdit(
                          account.id,
                          "name",
                          event.target.value,
                        )
                      }
                    />
                    <input
                      type="number"
                      min="0"
                      step="0.01"
                      value={edit.balance}
                      onChange={(event) =>
                        changeAccountEdit(
                          account.id,
                          "balance",
                          event.target.value,
                        )
                      }
                    />
                    <select
                      value={edit.currencyId}
                      onChange={(event) =>
                        changeAccountEdit(
                          account.id,
                          "currencyId",
                          event.target.value,
                        )
                      }
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
                        onChange={(event) =>
                          changeAccountEdit(
                            account.id,
                            "isActive",
                            event.target.checked,
                          )
                        }
                      />
                      Active
                    </label>
                    <button type="button" onClick={() => saveAccount(account)}>
                      Update
                    </button>
                    <button
                      className="danger-button"
                      type="button"
                      onClick={() => removeAccount(account.id)}
                    >
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

export default StaffDashboard;
