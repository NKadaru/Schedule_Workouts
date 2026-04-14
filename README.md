# Workout Scheduler

**Last Updated:** 2026-04-14

A full-stack workout scheduling application built with a .NET 10 backend and Angular 20 frontend.

## Recent Changes

- `backend/WorkoutScheduler.Api/Services/GeminiAgentService.cs` — Added `GetWhoopContext()` method that fetches today's WHOOP recovery, strain, HRV, RHR, sleep data and recent 7-day daily history for chatbot context.
- `backend/WorkoutScheduler.Api/Services/GeminiAgentService.cs` — Injected `WhoopService` dependency into the Gemini agent service (constructor parameter + private field).
- `backend/WorkoutScheduler.Api/Services/WhoopService.cs` — Added `SortKey` (raw date) to `DailySummary` and renamed local variable from `date` to `displayDate` for clarity.
- `backend/WorkoutScheduler.Api/Models/WhoopModels.cs` — Edited WHOOP models file (no visible content diff — file save/touch).
- `backend/WorkoutScheduler.Api/Services/WhoopService.cs` — Changed daily history date format from raw ISO date substring to `MMM-dd` display format (e.g., "Apr-14") using `DateTime.TryParse`.
- `backend/WorkoutScheduler.Api/Services/WhoopService.cs` — Refactored `GetAsync<T>` to use per-request `HttpRequestMessage` instead of setting `DefaultRequestHeaders`, added response logging and try/catch around deserialization with truncated JSON in error logs.
- `backend/WorkoutScheduler.Api/Services/WhoopService.cs` — Updated WHOOP API endpoints from `v1` to `v2` for cycle, recovery, and sleep data fetching.
- `backend/WorkoutScheduler.Api/Services/WhoopService.cs` — Edited WHOOP service (no visible content diff — file save/touch).
- `backend/WorkoutScheduler.Api/Controllers/WhoopController.cs` — Added `GET /api/whoop/debug` endpoint that returns raw WHOOP debug data (requires active connection).
- `backend/WorkoutScheduler.Api/Services/WhoopService.cs` — Refactored `GetDashboardAsync` to fetch cycles, recoveries, and sleeps in parallel, then join them by cycle ID/date into `DailyHistory` entries instead of building separate monthly lists and merging after the fact.
- `backend/WorkoutScheduler.Api/Services/WhoopService.cs` — Added file-based token persistence for WHOOP OAuth tokens (load on startup, save after exchange/refresh) so tokens survive app restarts; also simplified `IsConnected` check.
- `frontend/src/app/whoop/whoop.component.html` — Refactored 30-Day History table to use `dashboard.dailyHistory` instead of `monthlyStrain`, with inline `day.recovery`, `day.strain`, and `day.sleepHours` fields replacing helper methods.
- `backend/WorkoutScheduler.Api/Models/WhoopModels.cs` — Added `DailySummary` class and `DailyHistory` list to `WhoopDashboard` for per-day recovery, strain, and sleep history.
- `frontend/src/app/whoop/whoop.component.css` — Simplified WHOOP component styles: removed bar chart and color-coded card classes, switched connect button from solid fill to outlined/transparent style with hover background.
- `frontend/src/app/app.component.css` — Changed active tab style from solid green background to transparent with green border and text for a subtler, outline-style appearance.
- `frontend/src/app/whoop/whoop.component.css` — Added WHOOP dashboard grid layout, color-coded metric cards (green/yellow/red), and bar chart styles for recovery/strain/sleep trends.
- `frontend/src/app/whoop/whoop.component.ts` — Added computed getters (`avgRecovery`, `avgStrain`, `avgSleep`) for monthly dashboard averages; removed minor whitespace.
- `frontend/src/app/whoop/whoop.component.html` — Renamed heading from "WHOOP Recovery" to "WHOOP Dashboard", added "Today" section title, and restructured card grid layout.
- `backend/WorkoutScheduler.Api/Models/WhoopModels.cs` — Added `DailyEntry` class and monthly trend lists (`MonthlyRecovery`, `MonthlyStrain`, `MonthlySleep`) to `WhoopDashboard` for 30-day historical data support.
- `frontend/src/app/app.component.ts` — Added `activeView` property to support toggling between 'schedule' and 'whoop' views.
- `frontend/src/app/app.component.html` — Added `<app-whoop>` component to the main app template, placing it between the health status and error message sections.
- `frontend/src/app/whoop/whoop.component.html` — Added WHOOP Recovery panel template with recovery score, strain, HRV, RHR, sleep hours, and sleep score cards, plus connect/loading states.
- `frontend/src/app/services/whoop.service.ts` — Added Angular WHOOP service with methods to get OAuth authorize URL, connection status, and recovery/strain/sleep dashboard data.
- `backend/WorkoutScheduler.Api/Services/WhoopService.cs` — Implemented WHOOP API integration: OAuth token exchange, token refresh, and `GetDashboardAsync()` method that fetches recovery, strain, and sleep data from the WHOOP developer API.
- `backend/WorkoutScheduler.Api/Models/WhoopModels.cs` — Added Whoop data models: `WhoopCycleScore`, `WhoopSleep`, `WhoopSleepScore`, `WhoopSleepStages`, `WhoopPaginatedResponse<T>`, and a frontend-friendly `WhoopDashboard` summary class.
- `backend/WorkoutScheduler.Api/appsettings.Development.json` — Added Whoop API configuration (ClientId, ClientSecret, RedirectUri) for OAuth integration.
- `frontend/public/privacy.html` — Updated GrindFlow privacy policy page (WHOOP data usage disclosure, styling, and contact info).
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
