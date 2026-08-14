# Private Class Management System

Full-stack MVP with **.NET 8 Web API**, **MongoDB**, **JWT RBAC**, and **Angular 17** (Material UI).

## Architecture

```
Controller → Service → Repository → MongoDB
```

### Roles

| Role | Access |
|------|--------|
| **Admin** | Full access (users, classes, enrollments, attendance, marks, AI) |
| **Teacher** | Manage classes, enrollments, attendance, marks, AI insights |
| **Student** | View own classes, attendance, and marks only |

### Collections

`Users`, `Classes`, `Enrollments`, `Attendance`, `Marks`

## Prerequisites

- .NET 8 SDK
- Node.js 18+
- MongoDB running locally on `mongodb://localhost:27017`

## Quick Start

### 1. Start MongoDB

```bash
# If using Docker:
docker run -d --name mongo -p 27017:27017 mongo:7
```

### 2. Backend API

```bash
cd backend/ClassManagement.Api
dotnet run --launch-profile http
```

- API: http://localhost:5080  
- Swagger: http://localhost:5080/swagger  

Seeded accounts:

| Email | Password | Role |
|-------|----------|------|
| admin@classmgmt.local | Admin@123 | Admin |
| teacher@classmgmt.local | Teacher@123 | Teacher |
| student@classmgmt.local | Student@123 | Student |

### 3. Frontend

```bash
cd frontend/class-management
npm install
npm start
```

App: http://localhost:4200

## Forms / Modules

1. **Login** — email + password (JWT)
2. **Users** — Admin creates users with roles
3. **Classes** — name, subject, teacher
4. **Enrollments** — assign students to classes
5. **Attendance** — mark present/absent
6. **Marks** — enter scores + smart AI feedback
7. **AI Insights** — Auto Attendance Insight (at-risk prediction)

## AI Feature

**Auto Attendance Insight** (`GET /api/ai/attendance-insights`)

- Analyzes attendance rates and declining trends
- Flags students below 75% (Warning) or 60% (Critical)
- Returns natural-language warnings and action suggestions

Also available: **Smart Marks Feedback** (`POST /api/ai/marks-feedback/{markId}`)

## Project Structure

```
backend/ClassManagement.Api/
  Controllers/   Services/   Repositories/   Models/   DTOs/   Data/   Middleware/

frontend/class-management/
  src/app/core/        # auth, guards, interceptors, API
  src/app/features/    # login, users, classes, enrollments, attendance, marks, insights
  src/app/layout/      # shell / sidenav
```

## Configuration

Backend `appsettings.json`:

- `MongoDb:ConnectionString` / `DatabaseName`
- `Jwt:Key` / `Issuer` / `Audience` / `ExpiryMinutes`
- `Cors:Origins` (default `http://localhost:4200`)

Frontend `src/environments/environment.ts`:

- `apiUrl: 'http://localhost:5080/api'`
