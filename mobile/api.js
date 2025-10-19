// Simple API helper for React Native
// Set your backend URL here. For Android emulator use http://10.0.2.2:<port>
export const BACKEND_URL = process.env.BACKEND_URL || "https://chatapp-fjma.onrender.com";

export async function sendMessage(text, username = "mobile") {
  const res = await fetch(`${BACKEND_URL}/api/message/analyze`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ text, username }),
  });
  if (!res.ok) {
    throw new Error(`Backend error ${res.status}`);
  }
  return res.json(); // { id, username, text, sentiment, createdAt }
}
