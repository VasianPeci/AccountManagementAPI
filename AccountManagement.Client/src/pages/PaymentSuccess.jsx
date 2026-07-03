import { useEffect, useRef, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { confirmTopUp } from "../api/paymentApi";

function PaymentSuccess() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [message, setMessage] = useState("Confirming payment...");
  const [error, setError] = useState("");
  const confirmStarted = useRef(false);

  useEffect(() => {
    async function confirmPayment() {
      const sessionId = searchParams.get("session_id");

      if (confirmStarted.current) return;

      if (!sessionId) {
        setError("Stripe session was not found.");
        return;
      }

      confirmStarted.current = true;

      try {
        await confirmTopUp(sessionId);
        setMessage("Payment confirmed. Your balance was updated.");
      } catch (err) {
        confirmStarted.current = false;
        setError(err.message);
      }
    }

    confirmPayment();
  }, [searchParams]);

  return (
    <div className="dashboard">
      <div className="payment-panel">
        <h1>Stripe Top Up</h1>

        {error ? (
          <p className="error-message">{error}</p>
        ) : (
          <p className="info-message">{message}</p>
        )}

        <button type="button" onClick={() => navigate("/")}>
          Back to Dashboard
        </button>
      </div>
    </div>
  );
}

export default PaymentSuccess;
