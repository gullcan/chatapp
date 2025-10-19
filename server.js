import express from "express";
import cors from "cors";

const app = express();
app.use(cors());
app.use(express.json());

// Sahte veri (API endpoint)
const stats = {
  sales: 400,
  cost: 300,
  profit: 100,
};
const reportData = [
  { name: "Ocak", value: 400 },
  { name: "Şubat", value: 300 },
  { name: "Mart", value: 500 },
  { name: "Nisan", value: 200 },
];

// Endpoint'ler
app.get("/api/dashboard", (req, res) => res.json(stats));
app.get("/api/reports", (req, res) => res.json(reportData));

app.listen(5002, () => {
  console.log("✅ Backend running on http://localhost:5002");
});
