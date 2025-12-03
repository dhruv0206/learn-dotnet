# Gemini Chat App (.NET + Next.js)

This is a modern chat application built with a .NET 10 (Preview/Latest) backend and a Next.js TypeScript frontend, integrated with Google's Gemini API.

## Prerequisites

- .NET SDK (Latest)
- Node.js & npm
- A Google Gemini API Key (Get one from [Google AI Studio](https://aistudio.google.com/))

## Project Structure

- `Backend/`: ASP.NET Core Web API
- `frontend/`: Next.js 14+ App Router application

## Setup & Running

### 1. Backend

Navigate to the `Backend` directory:

```bash
cd Backend
```

Set your Gemini API Key. You can do this by setting an environment variable or editing `Program.cs` directly (not recommended for production).

**PowerShell:**
```powershell
$env:GEMINI_API_KEY="your_api_key_here"
dotnet run
```

The backend will start on `http://localhost:5218`.

### 2. Frontend

Navigate to the `frontend` directory:

```bash
cd frontend
npm install
npm run dev
```

The frontend will start on `http://localhost:3000`.

## Features

- **Modern UI**: Dark mode, glassmorphism, and smooth animations.
- **Gemini Integration**: Chats with the `gemini-1.5-flash` model.
- **Real-time**: Fast response times with a lightweight backend.

## Troubleshooting

- **CORS Errors**: Ensure the backend is running on port 5218. If it changes, update the fetch URL in `frontend/src/app/page.tsx`.
- **API Errors**: Check the backend console for detailed error messages from the Gemini API.
