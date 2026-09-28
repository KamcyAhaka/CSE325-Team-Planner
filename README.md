# Team Project Planner (CSE325 - .NET Software Development)

A modern, responsive collaboration workspace built with Blazor, designed for student and developer teams to organize, plan, and execute project deliverables. Easily manage project lifecycles, delegate tasks, track sprint progress visually, and coordinate team members.

## Key Features

* **Visual Kanban Board:** Seamlessly manage task status with an interactive drag-and-drop workflow layout (To Do, In Progress, Done).
* **Project Center:** Spin up projects with custom descriptions, start/end dates, and real-time member assignments.
* **Granular Team Collaboration:** Add new team members, manage roles (e.g., Owner, Member), and display active contributors with dynamic avatar icons.
* **Responsive Workspace Dashboard:** A unified home view highlighting overall project health, timeline metrics, active participants, and individual task lists.
* **Secure Authentication:** Integrated sign-in/sign-up and route protection powered by ASP.NET Core Identity.

---

## Tech Stack

* **Frontend Framework:** [Blazor WebAssembly / Server](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) (.NET 8)
* **Backend Framework:** [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)
* **Database:** [MongoDB Atlas](https://www.mongodb.com/atlas) via the [MongoDB .NET Driver](https://www.mongodb.com/docs/drivers/csharp/)
* **Authentication:** [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
* **Styling:** [Bootstrap](https://getbootstrap.com/) / Vanilla CSS
* **Language:** [C#](https://learn.microsoft.com/en-us/dotnet/csharp/)

---

## Environment Configuration

The app registers a singleton MongoDB client and database through dependency injection. Configure the database name in `appsettings.json`, then provide the connection string through .NET user secrets or the `MONGODB_URI` environment variable:

```json
{
  "MongoDB": {
    "DatabaseName": "TeamProjectPlanner"
  }
}
```

For local development, use user secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb+srv://<user>:<password>@cluster.mongodb.net/?retryWrites=true&w=majority"
```

Repositories and services can request `IMongoDatabase` from dependency injection.

> [!IMPORTANT]
> **Never commit credentials to source control.**
> Store your Atlas connection string using the [.NET Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local development, and environment variables or Azure Key Vault for production deployments.

---

## Getting Started

### 1. Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later
* A compatible IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) with the C# Dev Kit extension

### 2. Clone & Restore Dependencies

```bash
# Restore NuGet packages
dotnet restore
```

### 3. Run Local Development Server

```bash
# Start the Blazor development server
dotnet run
```

Open [https://localhost:5001](https://localhost:5001) (or [http://localhost:5000](http://localhost:5000)) in your browser to view the application.

### 4. Build for Production

```bash
# Build the application in Release mode
dotnet build --configuration Release

# Publish the application
dotnet publish --configuration Release --output ./publish
```

---

## Team Members

* **Divine Ahaka**
* **Luca Carlo**
* **Brigham Young Iga**
