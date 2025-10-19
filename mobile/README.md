# Mobile App (React Native CLI)

This is a minimal scaffold for the React Native mobile app that connects to the .NET backend and displays sentiment results for messages.

## Prerequisites

- Node.js LTS
- React Native CLI environment (Android Studio or Xcode depending on target)
- Android SDK for building an APK

## Setup

1. Initialize a new RN project (if you haven't already):
   ```bash
   npx react-native init SentimentChat
   ```
2. Copy the files from this `mobile/` folder into your project root, or adapt them:
   - `App.js`
   - `api.js`
3. Set your backend URL in `api.js`:
   ```js
   export const BACKEND_URL = "https://<your-render-backend>.onrender.com"; // or http://10.0.2.2:5002 for Android emulator
   ```
4. Run on Android:
   ```bash
   npx react-native run-android
   ```
5. Build APK (release):
   - Follow RN docs for generating a signed APK.

## Files

- `api.js`: Backend API helper for posting messages.
- `App.js`: Simple chat UI using React Native components.
