---
title: Sentiment Analysis Service
emoji: 💬
colorFrom: blue
colorTo: purple
sdk: gradio
sdk_version: 4.44.0
python_version: 3.9
app_file: app.py
pinned: false
---

# Sentiment Analysis Service

This is a sentiment analysis service that uses the CardiffNLP Twitter RoBERTa model to classify text as positive, neutral, or negative.

## API Endpoint

Once deployed on Hugging Face Spaces, the service will be available at:
- **Primary endpoint**: `https://YOUR-USERNAME-ai-service.hf.space/api/predict`
- **Fallback endpoint**: `https://YOUR-USERNAME-ai-service.hf.space/run/predict`

## Request Format

```json
{
  "data": ["Your text message here"]
}
```

## Response Format

```json
{
  "data": ["positive"]
}
```

Possible sentiment values: `positive`, `neutral`, `negative`

## Deployment Instructions

1. Create a new Space on Hugging Face (https://huggingface.co/spaces)
2. Choose Gradio as the SDK
3. Upload `app.py` and `requirements.txt`
4. The Space will automatically deploy
5. Copy your Space URL (e.g., `https://gulcan9-ai-service.hf.space`)
6. Get your HuggingFace API token from https://huggingface.co/settings/tokens
7. Update your backend's configuration:
   - `AI_SERVICE_URL`: Your Space URL
   - `HF_TOKEN`: Your HuggingFace API token

## Important Notes

- The Space uses Gradio's queue system for API requests
- Direct `/api/predict` and `/run/predict` endpoints require joining the queue
- The backend automatically handles the queue system with polling
- First request may take longer as the model loads (cold start)

## Model

Uses: `cardiffnlp/twitter-roberta-base-sentiment-latest`
