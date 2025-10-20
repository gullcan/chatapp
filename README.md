# AI-Powered Sentiment Chat Application (Web + Mobile)

A full-stack chat system with real-time AI sentiment analysis.

Project structure:

```
project-root/
├── frontend/           # React web app (deploy on Vercel)
├── mobile/             # React Native CLI app (instructions + sample screens)
├── backend/            # .NET Core API with SQLite (deploy on Render) ← backend moved here
└── ai-service/         # Python + Gradio service (deploy on Hugging Face Spaces)
```

## Services Overview

- Frontend (React): simple chat UI calling the backend to send messages and display sentiment.
- Backend (.NET 8 Web API + SQLite): user registration, message persistence, calls AI service for sentiment.
- AI Service (Python + Gradio): exposes an API that returns `positive`, `neutral`, or `negative` for input text.
- Mobile (React Native CLI): same UI logic as web, communicates with backend.

## Data Flow

1. User types a message on Web/Mobile.
2. Message is sent to Backend API `/api/message/analyze`.
3. Backend stores the message in SQLite and calls AI Service.
4. AI returns sentiment; backend returns it to frontend and stores result.
5. Frontend displays the message with sentiment badge.

## Deploy Targets

- Vercel: `frontend/`
- Render: `backend/` (set the service root/working directory to `backend/`)
- Hugging Face Spaces: `ai-service/`

## Working Demo Links

- Web (Vercel - production): https://chatapp-gulcans-projects-f56e0763.vercel.app
- Web (Vercel - previews):
  - https://chatapp-seven-flax.vercel.app
  - https://chatapp-git-main-gulcans-projects-f56e0763.vercel.app
  - https://chatapp-eacaxxp2d-gulcans-projects-f56e0763.vercel.app
- Backend API (Render): https://chatapp-azte.onrender.com
- AI Service (Hugging Face Spaces): https://gulcan99-ai-service.hf.space

## Environment Variables

- Backend (Render):
  - `HF_TOKEN` = Hugging Face token with "Make calls to Inference Providers"
  - `ConnectionStrings__Default` = `Data Source=/data/chat.db`
  - `FRONTEND_ORIGIN` = CSV of allowed origins, e.g.
    `https://chatapp-gulcans-projects-f56e0763.vercel.app,https://chatapp-seven-flax.vercel.app`
  - Optional: `AI_SERVICE_URL` = `https://gulcan99-ai-service.hf.space` (not required when using Inference API)

- Frontend (Vercel):
  - `REACT_APP_BACKEND_URL` = `https://chatapp-azte.onrender.com`

- Mobile (React Native CLI):
  - Set `BACKEND_URL` in `mobile/api.js` (or via env) to `https://chatapp-azte.onrender.com`

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
