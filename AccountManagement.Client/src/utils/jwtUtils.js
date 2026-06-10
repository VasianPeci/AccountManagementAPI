export function getUserFromToken() {
    const token = localStorage.getItem("token");

    if (!token) return null;

    try {
        const payload = JSON.parse(decodeToken(token.split(".")[1]));
        const role = getClaim(payload, [
            "role",
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        ]);

        return {
            id: getClaim(payload, [
                "nameid",
                "sub",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
            ]),
            clientId: getClaim(payload, ["clientId", "client_id"]),
            email: getClaim(payload, [
                "email",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
            ]),
            role: Array.isArray(role) ? role[0] : role,
        };
    } catch {
        return null;
    }
}

function getClaim(payload, keys) {
    return keys.map((key) => payload[key]).find(Boolean);
}

function decodeToken(value) {
    const base64 = value.replace(/-/g, "+").replace(/_/g, "/");
    const padded = base64.padEnd(Math.ceil(base64.length / 4) * 4, "=");

    return atob(padded);
}
