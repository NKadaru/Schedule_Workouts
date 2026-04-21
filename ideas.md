# GrindFlow — Future Ideas

## Easy (hours)

### PWA (Progressive Web App)
- Run `ng add @angular/pwa` to make GrindFlow installable on mobile
- Users can add it to their home screen from the browser
- Works offline with cached data

### Docker Containerization
- Add Dockerfiles for backend and frontend
- Makes deployment to any cloud provider trivial
- docker-compose for local dev with both services

## Medium (1-2 days)

### SQLite Database
- Replace `workouts.json` with SQLite via Entity Framework Core
- Free, no server needed — just a single file (`grindflow.db`)
- Enables workout history tracking, user data persistence
- NuGet: `Microsoft.EntityFrameworkCore.Sqlite`

### Agent Tool-Calling
- Let the chatbot actually modify workouts (not just give advice)
- Groq supports function calling with Llama models
- Define tools: addExercise, removeExercise, swapDay, createPlan
- Bot reads WHOOP recovery → decides → modifies the schedule

## Larger (weekend project)

### Authentication & Multi-User
- JWT-based auth with ASP.NET Identity
- User registration and login UI in Angular
- Per-user workout plans, WHOOP tokens, and chat history
- Role-based access if needed later

### Workout History & Analytics
- Track completed workouts over time in the database
- Show trends: consistency, volume, strain patterns
- Combine WHOOP recovery data with workout completion for insights
- "You've been overtraining legs" or "time for a deload week"

## Integrations

### Strava
- OAuth2 flow (similar to WHOOP)
- Pull running/cycling data — distance, pace, heart rate zones
- Show alongside workout schedule

### MyFitnessPal
- Nutrition tracking data
- Calories, macros, meal logging
- Chatbot can factor nutrition into workout recommendations

### Apple Health / Google Fit
- Fallback for non-WHOOP users
- Steps, heart rate, sleep data via platform APIs

## Deployment Options
- Backend: Azure App Service, AWS ECS, or Railway
- Frontend: Vercel, Azure Static Web Apps, or S3 + CloudFront
- Use environment variables for all secrets
- CI/CD via GitHub Actions (`.github/workflows/` already exists)
