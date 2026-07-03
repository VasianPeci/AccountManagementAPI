import { useNavigate } from "react-router-dom";

function PaymentCancel() {
  const navigate = useNavigate();

  return (
    <div className="dashboard">
      <div className="payment-panel">
        <h1>Payment Cancelled</h1>
        <p className="info-message">No top up was made to your account.</p>

        <button type="button" onClick={() => navigate("/")}>
          Back to Dashboard
        </button>
      </div>
    </div>
  );
}

export default PaymentCancel;
