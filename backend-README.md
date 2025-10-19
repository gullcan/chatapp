# Backend (.NET 8 + SQLite)

## Run locally

1. Ensure SQLite file will be created in the project root (default `chat.db`).
2. Set AI service URL (local or Spaces):
   - mac/Linux (zsh):
     ```bash
     # For your Hugging Face Space
     export AI_SERVICE_URL=https://gulcan9-ai-service.hf.space
     # Or if running the AI service locally
     # export AI_SERVICE_URL=http://127.0.0.1:7860
     ```
   - Windows (Powershell):
     ```powershell
     # For your Hugging Face Space
     $env:AI_SERVICE_URL="https://gulcan9-ai-service.hf.space"
     # Or if running the AI service locally
     # $env:AI_SERVICE_URL="http://127.0.0.1:7860"
     ```
3. Optionally override DB connection string:
   ```bash
   export ConnectionStrings__Default="Data Source=chat.db"
   ```
4. Run API:
   ```bash
   dotnet run --urls http://0.0.0.0:5002
   ```

Open Swagger at http://localhost:5002/swagger

## Endpoints

- POST `/api/message/analyze`
  - body: `{ "text": "hello", "username": "web" }`
  - returns: `{ id, username, text, sentiment, createdAt }`

## Deploy to Render

- Create a new Web Service, connect GitHub repo.
- Build command:
  ```bash
  dotnet restore && dotnet publish -c Release -o out
  ```
- Start command:
  ```bash
  dotnet out/ChatBackend.dll --urls http://0.0.0.0:$PORT
  ```
- Environment variables:
  - `AI_SERVICE_URL=https://gulcan9-ai-service.hf.space`
  - `ConnectionStrings__Default=Data Source=chat.db`
