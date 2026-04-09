# Workout Scheduler

**Last Updated:** 2026-04-09

A full-stack workout scheduling application built with a .NET 10 backend and Angular 20 frontend.

## Recent Changes

- `backend/WorkoutScheduler.Api/Program.cs` — Replaced default minimal API scaffolding with controller-based setup, added CORS policy for Angular dev server, configured camelCase JSON serialization, and exposed `partial class Program` for integration tests.
- `backend/WorkoutScheduler.Api/Data/workouts.json` — Restructured from flat workout arrays to `DayPlan` objects with per-day motivational quotes and exercises.
- `backend/WorkoutScheduler.Api/Models/Workout.cs` — Added `DayPlan` model with `Quote` and `Exercises` properties.
- `backend/WorkoutScheduler.Api/Services/WorkoutService.cs` — Updated to deserialize the new `DayPlan` structure.
- `backend/WorkoutScheduler.Api/Controllers/WorkoutsController.cs` — Updated to return `DayPlan` shape.
- `frontend/src/app/` — Added motivational hero banner, per-day quotes from API, workout completion checkboxes with progress bar, and fallback offline data when the backend is unavailable.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/)
- [Angular CLI 20](https://angular.dev/) — install globally with `npm install -g @angular/cli@20`

## Project Structure

```
├── backend/                  # .NET 10 ASP.NET Core Web API
│   ├── WorkoutScheduler.slnx
│   ├── WorkoutScheduler.Api/
│   └── WorkoutScheduler.Api.Tests/
├── frontend/                 # Angular 20 SPA
│   ├── src/
│   ├── angular.json
│   ├── proxy.conf.json
│   └── package.json
└── README.md
```

## Backend

### Build

```bash
dotnet build backend/WorkoutScheduler.slnx
```

### Run

```bash
dotnet run --project backend/WorkoutScheduler.Api
```

The API starts on `http://localhost:5277` by default. Verify it's running:

```bash
curl http://localhost:5277/api/health
```

### Run Tests

```bash
dotnet test backend/WorkoutScheduler.slnx
```

## Frontend

### Install Dependencies

```bash
cd frontend
npm install
```

### Build

```bash
cd frontend
npm run build
```

### Run

```bash
cd frontend
ng serve
```

Or equivalently:

```bash
cd frontend
npm start
```

The app starts on `http://localhost:4200`.

The frontend includes fallback workout data, so it works offline without the backend — you'll see an "Unable to connect" notice but all workouts and quotes will still display.

### Run Tests

```bash
cd frontend
npm test
```

## Running Both Projects Together

For local development, start the backend and frontend in two separate terminals:

**Terminal 1 — Backend:**

```bash
dotnet run --project backend/WorkoutScheduler.Api
```

**Terminal 2 — Frontend:**

```bash
cd frontend
ng serve
```

The Angular dev server is configured with a proxy (`frontend/proxy.conf.json`) that forwards all `/api` requests to the backend at `http://localhost:5277`. This means you can access the full application at `http://localhost:4200` and API calls will be transparently routed to the .NET backend — no CORS issues during development.
