# DevShelf 🗂️

> Your personal developer command center. Save snippets, bookmark resources, track tasks — all in one place, built by a developer for developers.

![DevShelf Dashboard Preview](https://img.shields.io/badge/status-in%20development-yellow?style=flat-square)
![React](https://img.shields.io/badge/React-18-61DAFB?style=flat-square&logo=react)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)
![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen?style=flat-square)

---

## The Problem

Every developer has the same chaos:

- Code snippets scattered across random `.txt` files and Slack messages to yourself
- Browser bookmarks folder with 300 links you'll "read later" and never find again
- Tasks split between 3 different apps none of which understand developer workflow
- No single place that connects your *resources*, *knowledge*, and *work*

**DevShelf fixes this.** One app, built on your own stack, that you own completely and can extend however you want.

---

## Features

### MVP (v1.0)
- 🔐 **Authentication** — JWT-based register and login with protected routes
- 📋 **Code Snippets** — Save, tag, search, and copy reusable code in any language with syntax highlighting
- 🔖 **Resources** — Bookmark articles, docs, tools, and videos with notes and tags
- ✅ **Tasks** — Developer-focused todo board with project grouping and priority levels
- 📊 **Dashboard** — Activity overview, stats, recent items, and quick-add actions
- 🌙 **Dark / Light mode** — Persisted to localStorage

### Roadmap (v2.0+)
- 🤖 AI-powered auto-tagging via OpenAI API
- 🔗 Browser extension — save current page directly to Resources
- 💻 VS Code extension — save selected code directly to Snippets
- 🐙 GitHub Gists sync — pull your gists into Snippets automatically
- 👥 Team workspaces — share snippets and resources with your team
- 📱 Mobile app — React Native port
- 🤖 Slack / Discord bot integration

---

## Tech Stack

### Frontend
| Technology | Purpose |
|---|---|
| React 18 | UI library |
| Vite | Build tool and dev server |
| React Router v6 | Client-side routing |
| Axios | HTTP client with interceptors |
| react-syntax-highlighter | Code snippet display |
| CSS Modules / Tailwind | Styling |

### Backend
| Technology | Purpose |
|---|---|
| ASP.NET Core 8 Web API | REST API |
| Entity Framework Core | ORM and migrations |
| SQLite | Development database |
| PostgreSQL | Production database |
| BCrypt.Net | Password hashing |
| JWT Bearer | Authentication tokens |

---

## Project Structure

```
devshelf/
├── client/                         # React frontend (Vite)
│   ├── public/
│   ├── src/
│   │   ├── api/
│   │   │   └── axios.js            # Axios instance + JWT interceptor
│   │   ├── components/
│   │   │   ├── common/             # Button, Modal, Spinner, EmptyState
│   │   │   ├── layout/             # Layout, Navbar, Sidebar
│   │   │   ├── snippets/           # SnippetCard, SnippetForm, SnippetDetail
│   │   │   ├── resources/          # ResourceCard, ResourceForm
│   │   │   └── tasks/              # TaskCard, TaskForm, KanbanBoard
│   │   ├── hooks/
│   │   │   ├── useAuth.js          # Auth state + login/logout/register
│   │   │   ├── useFetch.js         # Generic data fetching with loading/error
│   │   │   ├── useSnippets.js      # Snippets CRUD operations
│   │   │   ├── useResources.js     # Resources CRUD operations
│   │   │   └── useTasks.js         # Tasks CRUD operations
│   │   ├── pages/
│   │   │   ├── LoginPage.jsx
│   │   │   ├── RegisterPage.jsx
│   │   │   ├── DashboardPage.jsx
│   │   │   ├── SnippetsPage.jsx
│   │   │   ├── SnippetDetailPage.jsx
│   │   │   ├── ResourcesPage.jsx
│   │   │   └── TasksPage.jsx
│   │   ├── context/
│   │   │   └── AuthContext.jsx     # Global auth state via Context API
│   │   ├── utils/
│   │   │   ├── auth.js             # JWT decode, token storage helpers
│   │   │   └── formatters.js       # Date, truncate, language label helpers
│   │   ├── App.jsx                 # Routes + Layout wiring
│   │   └── main.jsx                # Entry point + BrowserRouter
│   ├── index.html
│   ├── vite.config.js
│   └── package.json
│
├── server/                         # ASP.NET Core backend
│   ├── Controllers/
│   │   ├── AuthController.cs       # POST /auth/register, /auth/login
│   │   ├── SnippetsController.cs   # CRUD /api/snippets
│   │   ├── ResourcesController.cs  # CRUD /api/resources
│   │   ├── TasksController.cs      # CRUD /api/tasks
│   │   └── DashboardController.cs  # GET /api/dashboard/stats
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Snippet.cs
│   │   ├── Resource.cs
│   │   └── DevTask.cs
│   ├── DTOs/
│   │   ├── Auth/
│   │   │   ├── LoginDto.cs
│   │   │   ├── RegisterDto.cs
│   │   │   └── AuthResponseDto.cs
│   │   ├── SnippetDto.cs
│   │   ├── ResourceDto.cs
│   │   └── TaskDto.cs
│   ├── Services/
│   │   ├── AuthService.cs
│   │   └── JwtService.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Middleware/
│   │   └── JwtMiddleware.cs
│   ├── Migrations/
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Program.cs
│
├── .gitignore
├── .env.example
└── README.md
```

---

## Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) v18+
- [.NET SDK](https://dotnet.microsoft.com/download) 8.0+
- [Git](https://git-scm.com/)

### 1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/devshelf.git
cd devshelf
```

### 2. Set up the backend

```bash
cd server

# Restore dependencies
dotnet restore

# Copy environment config
cp appsettings.Development.json.example appsettings.Development.json
# Edit appsettings.Development.json — add your JWT secret key

# Run database migrations
dotnet ef database update

# Start the API server (runs on https://localhost:7001)
dotnet run
```

### 3. Set up the frontend

```bash
cd client

# Install dependencies
npm install

# Copy environment config
cp .env.example .env
# Edit .env — set VITE_API_URL=https://localhost:7001

# Start the dev server (runs on http://localhost:5173)
npm run dev
```

### 4. Open the app

Navigate to `http://localhost:5173` — register an account and you're in.

---

## API Reference

### Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/auth/register` | Create new account | No |
| POST | `/auth/login` | Login, returns JWT | No |

### Snippets

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/snippets` | Get all user snippets | Yes |
| GET | `/api/snippets/:id` | Get single snippet | Yes |
| POST | `/api/snippets` | Create snippet | Yes |
| PUT | `/api/snippets/:id` | Update snippet | Yes |
| DELETE | `/api/snippets/:id` | Delete snippet | Yes |

### Resources

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/resources` | Get all user resources | Yes |
| GET | `/api/resources/:id` | Get single resource | Yes |
| POST | `/api/resources` | Create resource | Yes |
| PUT | `/api/resources/:id` | Update resource | Yes |
| DELETE | `/api/resources/:id` | Delete resource | Yes |

### Tasks

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/tasks` | Get all user tasks | Yes |
| GET | `/api/tasks/:id` | Get single task | Yes |
| POST | `/api/tasks` | Create task | Yes |
| PUT | `/api/tasks/:id` | Update task | Yes |
| PATCH | `/api/tasks/:id/status` | Update task status only | Yes |
| DELETE | `/api/tasks/:id` | Delete task | Yes |

### Dashboard

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/dashboard/stats` | Summary stats + recent items | Yes |

---

## Data Models

### User
```json
{
  "id": "uuid",
  "username": "string",
  "email": "string",
  "passwordHash": "string",
  "createdAt": "datetime"
}
```

### Snippet
```json
{
  "id": "uuid",
  "title": "string",
  "description": "string",
  "code": "string",
  "language": "string",
  "tags": ["string"],
  "userId": "uuid",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

### Resource
```json
{
  "id": "uuid",
  "title": "string",
  "url": "string",
  "notes": "string",
  "type": "article | video | tool | docs | other",
  "tags": ["string"],
  "userId": "uuid",
  "createdAt": "datetime"
}
```

### Task
```json
{
  "id": "uuid",
  "title": "string",
  "description": "string",
  "status": "todo | in-progress | done",
  "priority": "low | medium | high",
  "project": "string",
  "dueDate": "datetime | null",
  "userId": "uuid",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

---

## Environment Variables

### Frontend (`client/.env`)
```env
VITE_API_URL=https://localhost:7001
```

### Backend (`server/appsettings.Development.json`)
```json
{
  "Jwt": {
    "Key": "your-super-secret-key-min-32-chars",
    "Issuer": "devshelf-api",
    "Audience": "devshelf-client",
    "ExpiryDays": 7
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=devshelf.db"
  }
}
```

---

## Build Roadmap

### ✅ Sprint 1 — Auth Foundation
- [ ] .NET project scaffold with EF Core + SQLite
- [ ] User model + AppDbContext
- [ ] POST /auth/register with BCrypt password hashing
- [ ] POST /auth/login returning signed JWT
- [ ] React project scaffold with Vite
- [ ] AuthContext + useAuth hook
- [ ] Login page with form validation
- [ ] Register page with form validation
- [ ] Axios instance with JWT interceptor
- [ ] ProtectedRoute component
- [ ] React Router setup with protected and public routes

### 🔲 Sprint 2 — Snippets
- [ ] Snippet model + migration
- [ ] SnippetsController full CRUD
- [ ] Snippets list page with search and language filter
- [ ] Snippet detail page (`/snippets/:id`)
- [ ] Add/edit snippet modal with form
- [ ] Syntax highlighting display
- [ ] Copy to clipboard button
- [ ] Tag filter UI

### 🔲 Sprint 3 — Resources + Tasks
- [ ] Resource model + migration
- [ ] ResourcesController full CRUD
- [ ] Resources list page with type and tag filter
- [ ] Task model + migration
- [ ] TasksController full CRUD + PATCH status
- [ ] Tasks board with status columns
- [ ] Priority badges
- [ ] Project grouping

### 🔲 Sprint 4 — Dashboard + Polish
- [ ] DashboardController stats endpoint
- [ ] Dashboard page with live stats
- [ ] Loading skeleton states
- [ ] Consistent empty states
- [ ] Dark / light mode toggle
- [ ] Toast notifications for actions
- [ ] Responsive layout for mobile

---

## Contributing

This is a personal productivity project but PRs and suggestions are welcome.

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Commit your changes: `git commit -m 'feat: add your feature'`
4. Push to the branch: `git push origin feature/your-feature-name`
5. Open a Pull Request

### Commit Convention

This project uses [Conventional Commits](https://www.conventionalcommits.org/):

```
feat:     new feature
fix:      bug fix
docs:     documentation changes
style:    formatting, no logic change
refactor: code restructure, no feature change
chore:    dependencies, config, tooling
```

---

## License

MIT — see [LICENSE](LICENSE) for details.

---

## Author

Built by a developer who got tired of losing good snippets to the void.

> *"The best tool is the one you actually use."*

---

<p align="center">
  <sub>Start with Sprint 1. Ship something real.</sub>
</p>
