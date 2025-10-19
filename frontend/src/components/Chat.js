import React, { useEffect, useState } from "react";

function Chat() {
  const [message, setMessage] = useState("");
  const [chatLog, setChatLog] = useState([]);

  const baseUrl =
    process.env.REACT_APP_API_URL ||
    process.env.REACT_APP_BACKEND_URL ||
    "http://localhost:5002";

  useEffect(() => {
    // Load recent messages
    const loadHistory = async () => {
      try {
        const res = await fetch(`${baseUrl}/api/message?limit=50`);
        if (!res.ok) return;
        const items = await res.json();
        // items: [{ text, sentiment, ... }]
        const mapped = items
          .slice()
          .reverse() // oldest first in UI
          .map((it) => ({ message: it.text, sentiment: it.sentiment }));
        setChatLog(mapped);
      } catch (e) {
        // ignore
      }
    };
    loadHistory();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const sendMessage = async () => {
    if (!message.trim()) return;

    try {
      // .NET backend'e istek atıyoruz
      const response = await fetch(`${baseUrl}/api/message/analyze`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ text: message, username: "web" }),
      });

      if (!response.ok) {
        throw new Error(`Backend error ${response.status}`);
      }

      const data = await response.json();
      // Beklenen şekil: { id, username, text, sentiment, createdAt }
      const sentiment = data?.sentiment || "neutral";

      setChatLog([...chatLog, { message, sentiment }]);
      setMessage("");
    } catch (err) {
      console.error("Error sending message:", err);
      setChatLog([...chatLog, { message, sentiment: "Error contacting AI service" }]);
      setMessage("");
    }
  };

  const badgeClass = (s) => {
    const val = (s || "").toLowerCase();
    if (val.startsWith("pos")) return "badge badge-positive";
    if (val.startsWith("neg")) return "badge badge-negative";
    return "badge badge-neutral";
  };

  return (
    <div className="chat-container">
      <h1>💬 Sentiment AI Chat</h1>
      <div className="chat-box">
        {chatLog.map((entry, index) => (
          <div key={index} className="chat-entry">
            <p><b>You:</b> {entry.message}</p>
            <p>
              <b>Sentiment:</b>
              <span className={badgeClass(entry.sentiment)}>{entry.sentiment}</span>
            </p>
          </div>
        ))}
      </div>
      <div className="chat-input">
        <input
          type="text"
          placeholder="Type your message..."
          value={message}
          onChange={(e) => setMessage(e.target.value)}
        />
        <button onClick={sendMessage}>Send</button>
      </div>
    </div>
  );
}

export default Chat;
