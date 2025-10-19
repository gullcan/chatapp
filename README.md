# AI-Powered Sentiment Chat Application (Web + Mobile)

A full-stack chat system with real-time AI sentiment analysis.

Project structure:

```
project-root/
├── frontend/           # React web app (deploy on Vercel)
├── mobile/             # React Native CLI app (instructions + sample screens)
├── backend/            # .NET Core API with SQLite (deploy on Render)
└── ai-service/         # Python + Gradio service (deploy on Hugging Face Spaces)
```

## Services Overview

- Frontend (React): simple chat UI calling the backend to send messages and display sentiment.
- Backend (.NET 8 Web API + SQLite): user registration, message persistence, calls AI service for sentiment.
- AI Service (Python + Gradio): exposes an API that returns `positive`, `neutral`, or `negative` for input text.
- Mobile (React Native CLI): same UI logic as web, communicates with backend.

## Data Flow

1. User types a message on Web/Mobile.
2. Message is sent to Backend API `/api/messages`.
3. Backend stores the message in SQLite and calls AI Service.
4. AI returns sentiment; backend returns it to frontend and stores result.
5. Frontend displays the message with sentiment badge.

## Deploy Targets

- Vercel: `frontend/`
- Render: `backend/`
- Hugging Face Spaces: `ai-service/`

## Working Demo Links

- Web (Vercel): https://chatapp-gold-omega.vercel.app
- Backend API (Render): https://chatapp-fjma.onrender.com
- AI Service (Hugging Face Spaces): https://gulcan9-ai-service.hf.space

## Environment Variables

- Backend:
  - `AI_SERVICE_URL` = your Hugging Face Space API URL like `https://<username>-ai-service.hf.space` (no trailing slash)
  - `ConnectionStrings__Default` = `Data Source=chat.db`

- Frontend:
  - `VITE_BACKEND_URL` = Render backend URL like `https://your-backend.onrender.com`

- Mobile:
  - Use the same backend URL in `mobile/api.js`.

## Day-by-Day Plan

- Day 1: Implement AI service + Backend (SQLite persistence) and local tests.
- Day 2: Implement Web Frontend and deploy to Vercel. Integrate with backend.
- Day 3: Implement Mobile app screens and instructions, wire to backend, finalize docs.

## Notes on AI-generated vs Manual Code

- AI-assisted: initial scaffolding for all services, HTTP wiring, basic UI templates.
- Manually written example: backend EF Core `SaveChangesAsync` usage and SQL connection configuration, plus the `MessagesController` call chain to AI service and mapping to DTO responses.

---

See each folder's README for service-specific run and deploy instructions.
# chatapp
