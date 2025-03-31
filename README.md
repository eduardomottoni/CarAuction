# Project Overview

I started planning the architecture by defining the data structure, endpoints, and frontend. 
This led me to include a database and frontend, but I later realized—on the last day of the project—that these were not actual requirements.

---

## Demo

![Project Demo](./website.gif)

---

## Tech Stack
- **Frontend:** React with Bootstrap
- **Backend:** .NET Core 9.0
- **Database:** SQL Server

## Architectural Decisions

### .NET Core 9.0
There is no reason to start a new project with an older version of .NET Core

### Design Patterns
- **Repository Pattern:** To separate the data access logic from the business logic
- **Dependency Injection:** To inject the repository into the controller

### Attributes
- **bid as decimal:** For currency values precision
- **year as string**: To allow input of years like "2022-2023" or "22/23" ; tradeoff: not ideal for searching queries
- **load capacity as string**: To allow input of values like "1000 kg" or "1 ton"

### API x Frontend Architecture
I used a separate API and frontend approach to connect .NET with React.
- **Pros:** Modularity, flexibility, easy maintenance
- **Cons:** Requires a proxy (CORS) to connect with the backend

### Frontend - React with Bootstrap
I chose React with Bootstrap because a simple UI was required for the job.
- **Pros:** Easy to use, modern UI
- **Cons:** More dependencies to manage

### Database - SQL Server
SQL Server was chosen due to its seamless integration with Entity Framework Core and its ease of use with .NET.

## Libraries Used
- **Entity Framework Core**: For database connection, model creation, migrations, and ORM
- **Swagger**: For API testing and documentation
- **Moq**: For unit testing the API

## Possible Improvements
- Add an integration test project
- Add a UI test project using Jest
- Add functional tests using Cypress
- Implement a pipeline to deploy the API, database, and frontend
- Implement logging with Serilog

## Issues Encountered
- Failed to create a pipeline with database migration

---

## Requirements

- **Node.js**
- **NPM**
- **.NET Core 9.0**
- **ef-tools**
```bash
dotnet tool install --global dotnet-ef
```
- **SQL Server** (MSSQLLocalDB)
```bash
sqlcmd -S (localdb)\MSSQLLocalDB
```

## Prerequisites

- Adjust the SQL Server connection string in `appsettings.json` to your local setup.

## Migration

1. Run the following command to set up the database:

```bash
dotnet ef database update
```

## Steps to Run

### Backend

1. Navigate to the backend directory.
2. Run:

```bash
dotnet run
```

### Frontend

1. Navigate to the frontend directory.
2. Run:

```bash
npm run build
npm run dev
```

---
