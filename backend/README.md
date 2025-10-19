# Backend (.NET 8 + SQLite)

This folder mirrors the API currently in the repository root, packaged for deployment from `backend/` (e.g., to Render).

## Run locally

- Set AI service URL:
  - mac/Linux:
    ```bash
    export AI_SERVICE_URL=https://gulcan9-ai-service.hf.space
    ```
  - Windows (PowerShell):
    ```powershell
    $env:AI_SERVICE_URL="https://gulcan9-ai-service.hf.space"
    ```
- Optionally override DB connection:
  ```bash
  export ConnectionStrings__Default="Data Source=chat.db"
  ```
- Run:
  ```bash
  dotnet run --urls http://0.0.0.0:5002
  ```

## Deploy to Render

- Build command:
  ```bash
  dotnet restore && dotnet publish -c Release -o out
  ```
- Start command:
  ```bash
  dotnet out/ChatBackend.dll --urls http://0.0.0.0:$PORT
  ```
- Env vars:
  - `AI_SERVICE_URL=https://gulcan9-ai-service.hf.space`
  - `ConnectionStrings__Default=Data Source=chat.db`
