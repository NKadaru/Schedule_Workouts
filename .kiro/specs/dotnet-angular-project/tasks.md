# Implementation Plan: Workout Scheduling Full-Stack Project

## Overview

Incrementally scaffold a .NET 10 + Angular 20 monorepo. Each task builds on the previous one, starting with the backend project structure, then the frontend, then wiring them together with proxy configuration and integration, and finally adding documentation.

## Tasks

- [x] 1. Scaffold the .NET 10 backend solution and API project
  - [x] 1.1 Create the backend solution and Web API project
    - Create `backend/` directory
    - Run `dotnet new sln -n WorkoutScheduler -o backend`
    - Run `dotnet new webapi -n WorkoutScheduler.Api -o backend/WorkoutScheduler.Api --framework net10.0`
    - Add the project to the solution: `dotnet sln backend/WorkoutScheduler.sln add backend/WorkoutScheduler.Api/WorkoutScheduler.Api.csproj`
    - Verify the `.csproj` targets `net10.0`
    - _Requirements: 1.1, 1.2, 1.4_

  - [x] 1.2 Configure Program.cs with minimal hosting, CORS, and camelCase JSON
    - Set up `builder.Services.AddControllers()` with `JsonSerializerOptions` using `JsonNamingPolicy.CamelCase`
    - Configure CORS policy to allow the Angular dev server origin (`http://localhost:4200`)
    - Map controllers and set up the `/api` route prefix
    - _Requirements: 1.3, 3.3, 4.3_

  - [x] 1.3 Create the Models folder and data models
    - Create `backend/WorkoutScheduler.Api/Models/` folder
    - Add `HealthResponse.cs` with `Status` and `Timestamp` properties
    - Add `WeatherForecast.cs` with `Date`, `TemperatureC`, `Summary`, and computed `TemperatureF` properties
    - _Requirements: 4.1, 4.2_

  - [x] 1.4 Create the Services folder and WeatherForecastService
    - Create `backend/WorkoutScheduler.Api/Services/` folder
    - Implement `WeatherForecastService` that generates sample forecast data
    - Register the service in `Program.cs` via dependency injection
    - _Requirements: 4.1, 4.2_

  - [x] 1.5 Create the Controllers folder with HealthCheckController and WeatherForecastController
    - Create `backend/WorkoutScheduler.Api/Controllers/` folder
    - Implement `HealthCheckController` with `GET /api/health` returning `HealthResponse` (HTTP 200)
    - Implement `WeatherForecastController` with `GET /api/weatherforecast` injecting `WeatherForecastService`
    - _Requirements: 1.5, 4.1, 4.2_

- [x] 2. Checkpoint - Verify backend builds and runs
  - Ensure the backend project compiles with `dotnet build backend/WorkoutScheduler.sln`
  - Ensure all tests pass, ask the user if questions arise.

- [x] 3. Scaffold the Angular 20 frontend project
  - [x] 3.1 Create the Angular 20 application
    - Run `ng new frontend --style=css --routing=false --ssr=false --skip-git` (or equivalent Angular CLI command for Angular 20)
    - Verify `frontend/package.json` contains Angular 20 dependencies
    - Verify standalone components are the default
    - _Requirements: 2.1, 2.2, 2.4, 2.5_

  - [x] 3.2 Create frontend data models
    - Create `frontend/src/app/models/health-response.ts` with `HealthResponse` interface
    - Create `frontend/src/app/models/weather-forecast.ts` with `WeatherForecast` interface
    - _Requirements: 5.1_

  - [x] 3.3 Create the HealthService
    - Create `frontend/src/app/services/health.service.ts`
    - Implement `checkHealth(): Observable<HealthResponse>` calling `GET /api/health`
    - Include error handling that propagates HTTP errors
    - _Requirements: 5.1_

  - [x] 3.4 Update AppComponent to display API health status
    - Inject `HealthService` into `AppComponent`
    - Call `checkHealth()` on initialization
    - Display the health status and timestamp on success
    - Display a user-friendly error message (e.g., "Unable to connect to the API server") when the API is unreachable
    - _Requirements: 2.3, 5.2, 5.3_

- [x] 4. Configure development proxy
  - [x] 4.1 Create proxy configuration file
    - Create `frontend/proxy.conf.json` with `/api` target pointing to `http://localhost:5000`
    - Set `secure: false` and `changeOrigin: true`
    - _Requirements: 3.1, 3.2_

  - [x] 4.2 Wire proxy config into Angular dev server
    - Update `angular.json` to include `proxyConfig` option pointing to `proxy.conf.json` in the serve target
    - _Requirements: 3.1, 3.2_

- [x] 5. Checkpoint - Verify frontend builds
  - Ensure the frontend project compiles with `ng build` or `npm run build` in the `frontend/` directory
  - Ensure all tests pass, ask the user if questions arise.

- [x] 6. Backend tests
  - [x] 6.1 Set up the backend test project
    - Run `dotnet new xunit -n WorkoutScheduler.Api.Tests -o backend/WorkoutScheduler.Api.Tests --framework net10.0`
    - Add project to solution: `dotnet sln backend/WorkoutScheduler.sln add backend/WorkoutScheduler.Api.Tests/WorkoutScheduler.Api.Tests.csproj`
    - Add project reference to `WorkoutScheduler.Api`
    - Add `Microsoft.AspNetCore.Mvc.Testing` NuGet package
    - Add `FsCheck` and `FsCheck.Xunit` NuGet packages
    - _Requirements: 1.5, 4.2_

  - [x] 6.2 Write unit tests for health and weather endpoints
    - Test `GET /api/health` returns 200 with `{ status: "healthy", timestamp: "..." }`
    - Test `GET /api/weatherforecast` returns a JSON array with expected properties
    - Use `WebApplicationFactory<Program>` for integration-style tests
    - _Requirements: 1.5, 4.2, 4.3_

  - [x] 6.3 Write property test for camelCase JSON serialization (Property 1)
    - **Property 1: CamelCase JSON serialization round trip**
    - Generate random C# model instances, serialize with the API's configured `JsonSerializerOptions`, and assert all JSON keys are camelCase equivalents of C# PascalCase property names
    - Use FsCheck with a minimum of 100 iterations
    - **Validates: Requirements 4.3**

- [x] 7. Frontend tests
  - [x] 7.1 Write unit tests for HealthService
    - Test that `checkHealth()` calls `GET /api/health`
    - Test error handling when the API returns a non-2xx response
    - _Requirements: 5.1_

  - [x] 7.2 Write unit tests for AppComponent
    - Test that the component renders health status on successful API response
    - Test that the component renders an error message when the API is unreachable
    - _Requirements: 5.2, 5.3_

- [x] 8. Checkpoint - Ensure all tests pass
  - Run `dotnet test backend/WorkoutScheduler.sln` and verify all backend tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [x] 9. Create root-level README with build and run instructions
  - Create `README.md` at the repository root
  - Document prerequisites: .NET 10 SDK, Node.js version, Angular CLI version
  - Document how to build and run the backend (`dotnet run` from `backend/WorkoutScheduler.Api/`)
  - Document how to build and run the frontend (`ng serve` from `frontend/`)
  - Document how to run both projects simultaneously for local development
  - _Requirements: 6.1, 6.2, 6.3, 6.4_

- [x] 10. Final checkpoint - Ensure everything builds and all tests pass
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation
- Property tests validate universal correctness properties from the design document
- The backend uses C# / .NET 10 and the frontend uses TypeScript / Angular 20
