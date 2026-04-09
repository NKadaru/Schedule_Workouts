# Requirements Document

## Introduction

This document defines the requirements for setting up a full-stack web application project for scheduling workouts. The backend uses .NET 10 (ASP.NET Core Web API) and the frontend uses Angular 20. The project structure should support clean separation of concerns, modern development practices, and a smooth local development experience.

## Glossary

- **Backend**: The ASP.NET Core 10 Web API project that serves HTTP endpoints and handles business logic
- **Frontend**: The Angular 20 single-page application that provides the user interface
- **Solution**: The .NET solution file (.sln) that organizes the backend project(s)
- **API_Server**: The ASP.NET Core Web API application
- **Angular_App**: The Angular 20 client application
- **Developer**: A person who clones and works on this project locally
- **Proxy_Config**: The Angular development server proxy configuration that forwards API requests to the backend

## Requirements

### Requirement 1: .NET 10 Backend Project Setup

**User Story:** As a Developer, I want a .NET 10 Web API project scaffolded with a proper solution structure, so that I can build backend services using the latest .NET platform.

#### Acceptance Criteria

1. THE Solution SHALL contain a .NET 10 ASP.NET Core Web API project
2. THE API_Server SHALL target the `net10.0` target framework
3. THE API_Server SHALL include a `Program.cs` file with minimal API hosting configuration
4. THE Solution SHALL include a `.sln` file at the repository root or in a dedicated `backend` folder
5. THE API_Server SHALL include a sample health-check endpoint that returns HTTP 200 with a JSON status response

### Requirement 2: Angular 20 Frontend Project Setup

**User Story:** As a Developer, I want an Angular 20 project scaffolded with default configuration, so that I can build the user interface using the latest Angular framework.

#### Acceptance Criteria

1. THE Angular_App SHALL be created using the Angular CLI with Angular version 20
2. THE Angular_App SHALL reside in a dedicated `frontend` folder at the repository root
3. THE Angular_App SHALL include a default app component that renders successfully
4. THE Angular_App SHALL use standalone components as the default component style
5. THE Angular_App SHALL include a `package.json` with Angular 20 dependencies

### Requirement 3: Development Proxy Configuration

**User Story:** As a Developer, I want the Angular dev server to proxy API requests to the .NET backend, so that I can develop frontend and backend together without CORS issues.

#### Acceptance Criteria

1. THE Angular_App SHALL include a Proxy_Config that forwards `/api` requests to the API_Server
2. WHEN the Angular dev server receives a request matching the `/api` path prefix, THE Proxy_Config SHALL route the request to the API_Server running on its configured port
3. THE API_Server SHALL configure CORS to allow requests from the Angular dev server origin for non-proxied scenarios

### Requirement 4: Backend API Structure

**User Story:** As a Developer, I want the API project organized with a clean folder structure, so that I can add features in a maintainable way.

#### Acceptance Criteria

1. THE API_Server SHALL organize code into `Controllers`, `Models`, and `Services` folders
2. THE API_Server SHALL include a sample `WeatherForecastController` or equivalent to demonstrate the project structure
3. THE API_Server SHALL return JSON responses with camelCase property naming by default

### Requirement 5: Frontend-Backend Integration Verification

**User Story:** As a Developer, I want a working example of the frontend calling the backend API, so that I can verify the full-stack setup works end to end.

#### Acceptance Criteria

1. THE Angular_App SHALL include a sample service that calls the API_Server health-check endpoint
2. THE Angular_App SHALL display the API response in the default app component
3. WHEN the API_Server is unreachable, THE Angular_App SHALL display a user-friendly error message instead of failing silently

### Requirement 6: Build and Run Instructions

**User Story:** As a Developer, I want clear instructions to build and run both projects locally, so that I can get started quickly after cloning the repository.

#### Acceptance Criteria

1. THE Solution SHALL include a root-level README with instructions to build and run the API_Server
2. THE Solution SHALL include a root-level README with instructions to build and run the Angular_App
3. THE README SHALL document the required prerequisites (.NET 10 SDK, Node.js version, Angular CLI version)
4. THE README SHALL document how to run both projects simultaneously for local development
