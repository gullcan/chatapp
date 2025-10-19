import React, { useState } from "react";

function Chat() {
  const [message, setMessage] = useState("");
  const [chatLog, setChatLog] = useState([]);

  const sendMessage = async () => {
    if (!message.trim()) return;

    try {
      // .NET backend'e istek atıyoruz
      const baseUrl = process.env.REACT_APP_BACKEND_URL || "http://localhost:5002";
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

  return (
    <div className="chat-container">
      <h1>💬 Sentiment AI Chat</h1>
      <div className="chat-box">
        {chatLog.map((entry, index) => (
          <div key={index} className="chat-entry">
            <p><b>You:</b> {entry.message}</p>
            <p><b>Sentiment:</b> {entry.sentiment}</p>
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
