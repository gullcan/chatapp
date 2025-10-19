import gradio as gr
from transformers import pipeline

# Use a sentiment model that returns positive/neutral/negative
# CardiffNLP Twitter RoBERTa is lightweight and returns these labels
SENTIMENT_MODEL = "cardiffnlp/twitter-roberta-base-sentiment-latest"
classifier = pipeline("sentiment-analysis", model=SENTIMENT_MODEL)

def predict_sentiment(text: str) -> str:
    """
    Returns one of: positive | neutral | negative
    This function signature works with Gradio and Spaces Inference API
    which expects payload: {"data": ["text"]} and returns {"data": ["label"]}
    """
    if not text or not text.strip():
        return "neutral"
    try:
        result = classifier(text)[0]
        # result example: { 'label': 'positive', 'score': 0.98 }
        label = result.get("label", "neutral").lower()
        # Normalize label just in case
        if label.startswith("pos"):
            return "positive"
        if label.startswith("neg"):
            return "negative"
        return "neutral"
    except Exception:
        # Fallback to neutral on any unexpected error
        return "neutral"

# Simple Gradio Interface so Spaces exposes /api/predict
demo = gr.Interface(
    fn=predict_sentiment,
    inputs=gr.Textbox(label="Message"),
    outputs=gr.Textbox(label="Sentiment"),
    title="Sentiment Analysis Service",
    description="Returns positive / neutral / negative",
    api_name="predict"  # Explicitly name the API endpoint
)

if __name__ == "__main__":
    # For Hugging Face Spaces deployment
    # Launch without queue for direct API access
    demo.launch(
        share=True,
        server_name="0.0.0.0",
        server_port=7860,
        show_api=True
    )
