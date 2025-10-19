from flask import Flask, request, jsonify
from transformers import pipeline
import os

app = Flask(__name__)

# Use a sentiment model that returns positive/neutral/negative
SENTIMENT_MODEL = "cardiffnlp/twitter-roberta-base-sentiment-latest"
classifier = pipeline("sentiment-analysis", model=SENTIMENT_MODEL)

@app.route('/api/predict', methods=['POST'])
def predict():
    """
    Accepts JSON: {"data": ["text"]}
    Returns JSON: {"data": ["sentiment"]}
    """
    try:
        data = request.get_json()
        
        if not data or 'data' not in data or not isinstance(data['data'], list) or len(data['data']) == 0:
            return jsonify({"error": "Invalid request format. Expected: {\"data\": [\"text\"]}"}), 400
        
        text = data['data'][0]
        
        if not text or not text.strip():
            return jsonify({"data": ["neutral"]})
        
        result = classifier(text)[0]
        label = result.get("label", "neutral").lower()
        
        # Normalize label
        if label.startswith("pos"):
            sentiment = "positive"
        elif label.startswith("neg"):
            sentiment = "negative"
        else:
            sentiment = "neutral"
        
        return jsonify({"data": [sentiment]})
    
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route('/health', methods=['GET'])
def health():
    return jsonify({"status": "healthy", "model": SENTIMENT_MODEL})

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 7860))
    app.run(host='0.0.0.0', port=port)
