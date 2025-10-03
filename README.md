# Doera - *Your era to do*
### Overview
Doera is a web-based personal task manager that helps users organize and manage their tasks efficiently. Each user has their own private workspace to track, add, and update tasks. The app is designed to be simple, intuitive, and accessible for anyone looking to stay organized, whether for personal use, teams, or as a learning project.

## Table of Contents
- [Overview](#overview)
- [Live Demo](#live-demo)
- [Getting Started](#getting-started)
  - [Docker Compose](#option-1-docker-compose)
  - [Manual Setup](#option-2-manual-setup-net-8--sql-server)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
  - [Layered Structure](#layered-structure)
  - [CQRS & Patterns](#cqrs-hybrid)
  - [Project Structure](#project-structure)
- [Features](#features)
- [Domain & Data](#domain--data)
  - [Tables](#tables)
  - [ER Diagram](#er-diagram)
  - [Data & Persistence](#data--persistence)
- [Learning Highlights](#learning-highlights)
- [License](#license)

## Live Demo
### See Doera running at [doera.denic.dev](https://doera.denic.dev).
> **Note:** This demo runs on Azure’s free/student tier.  
> The app may take a while to start or show occasional errors.  
> If you run into issues, try refreshing after a few seconds.

## Getting Started

You can run Doera using Docker Compose (recommended) or manually with .NET 8 and SQL Server.

**First, clone the repository:**
```sh
git clone https://github.com/Lajron/Doera.git
cd Doera
```
### Option 1: Docker Compose
**Prerequisite:** [Docker](https://www.docker.com/) installed

1. Start the services:
   ```sh
   docker compose up
   ```
   - Web app: [http://localhost:8080](http://localhost:8080)
   - SQL Server: localhost:1433

2. Stop the services:
   ```sh
   docker compose down
   ```

*The database is automatically migrated and ready to use.*

**Additional Commands**
- Rebuild containers:
  ```sh
  docker compose build
  ```
- Run in detached mode:
  ```sh
  docker compose up -d
  ```
### Option 2: Manual Setup (.NET 8 & SQL Server)
**Prerequisites:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

1. Configure your connection string in `appsettings.json` if necessary.
2. Run the application:
   ```sh
   dotnet run --project Doera.Web
   ```
3. Open your browser and check the console output for the exact local URL (e.g., http://localhost:5020).

*The database will be automatically migrated when you start the app.*

---

## Tech Stack

### Backend
##### **.NET 8 (C# 12), ASP.NET Core MVC**
- Entity Framework Core (SqlServer, Tools, Design)
- ASP.NET Core Identity
- FluentValidation
- IMemoryCache
- SendGrid

### Frontend
##### **Razor MVC Views, HTML5, CSS3, Javascript**
- jQuery
- Bootstrap
- NToastNotify (Toastr notifications)

### Database
##### **Microsoft SQL Server**

### Observability / Logging
- Elmah.io (error monitoring)
- Built-in logging (Microsoft.Extensions.Logging)

### Tooling
- EF Core CLI support (Design + Tools packages)
- Visual Studio 2022 / .NET CLI
- SQL Server Management Studio

---

## Architecture



### Layered Structure
- **Web (`Doera.Web`)**
  - MVC controllers, view models, mappings, toast notifications (NToastNotify), authentication endpoints, startup wiring (`Program.cs`), custom view location expander.
- **Application (`Doera.Application`)**
  - DTOs (requests/responses), business/application services (`TagService`, `TodoItemService`, `TodoListService`, `AdminService`), validation (FluentValidation), result/error abstractions, service interfaces.
- **Infrastructure (`Doera.Infrastructure`)**
  - EF Core persistence (`ApplicationDbContext`, Migrations), repositories (`TagRepository`, `TodoItemRepository`, etc.), `BaseRepository<T>`, Unit of Work, Identity adapters (`ICurrentUser`, `IdentityService`), query dispatcher + query handlers (read side), slug utility.
- **Core (`Doera.Core`)**
  - Domain entities, enums, base entity type, repository + specification + unit of work interfaces, identity store abstractions.

### Responsibilities per Layer
- **Web:** Orchestrates HTTP flow, model binding, validation feedback; invokes application services or query dispatcher; handles authentication UI and notifications.  
- **Application:** Coordinates use cases (create/update/list); enforces application-level rules; maps to/from DTOs; invokes repositories via `IUnitOfWork`; encapsulates validation pipeline.  
- **Infrastructure:** Implements technical details (EF Core, SQL Server, repositories, query handlers producing DTOs, identity context access, cross-cutting utilities).  
- **Core:** Pure domain + interface contracts (no external tech dependencies); defines what infrastructure must implement.


### CQRS Hybrid
- **Query side:** Lightweight mediator via `IQueryDispatcher` and `IQueryHandler<TQuery,TResult>`; handlers are auto-registered through reflection.  
- **Command side:** Commands handled through application services (imperative service layer); not using full CQRS.  
- **Result model:** Query handlers and services return `Result<T>` (success + value or error), enabling uniform controller handling.  
- **Hybrid approach:** Service-based commands + mediated queries rather than strict domain-driven / full CQRS segregation.

### Patterns in Use

**Architecture & Layering**
- **Layered (*Clean-ish*) architecture** with dependency inversion (interfaces between Core/Application/Infrastructure/Web).  
- **Partial CQRS:** Queries via mediator-like dispatcher (`IQueryDispatcher` / `IQueryHandler<,>`), commands via application services.  
- **Lightweight mediator** (reflection-based handler discovery & registration).  
- **Application service layer** (use‑case orchestration: `TodoItemService`, etc.).  
- **Extension-based service registration** (`AddApplicationLayer`, `AddInfrastructureLayer`, etc.).  

**Data Access & Persistence**
- **Repository + Unit of Work** (`BaseRepository<T>`, `IUnitOfWork`).  
- **Specification** (predicate + includes passed into repository `FindAsync`).  
- **Async-first data access** (`await` throughout repositories + EF Core APIs).  
- **Projection** (LINQ `Select` directly to DTOs, avoiding over-fetching).  
- **Soft lifecycle fields** (`DeletedAt`, `ArchivedAt`) enabling soft-delete style handling.  

**Validation & Mapping**
- **FluentValidation pipeline** (centralized input/business rule validation).  
- **DTO + ViewModel mapping** (no entity leakage to controllers/views).  
- **Result wrapper pattern** (`Result<T>` for uniform success/error flow).  

**Utilities & Identity**
- **Identity abstraction** (`ICurrentUser`, `IIdentityService`) instead of raw `HttpContext`.  
- **Utility abstraction** (`ISlugGenerator`) for consistent slug creation.  

### Design Principles

**SOLID**
- **SRP (Single Responsibility Principle):** Controllers handle HTTP + flow, application services orchestrate use cases, repositories encapsulate persistence, query handlers perform focused read projections.
- **OCP (Open/Closed Principle):** Adding new queries/handlers, repositories, or specifications extends behavior without modifying existing core types (reflection-based handler registration reinforces this).
- **LSP (Liskov Substitution Principle):** Interfaces (`IRepository<T>`, `ITodoItemRepository`, `IQueryHandler<,>`) can be substituted by their implementations without breaking callers (no behavioral assumptions leaked).
- **ISP (Interface Segregation Principle):** Narrow, purpose-focused interfaces (e.g., `ICurrentUser`, `IIdentityService`, specific repository interfaces).
- **DIP (Dependency Inversion Principle):** High-level policies (services, handlers) depend on abstractions declared in Core / Application; Infrastructure provides implementations via DI.

**Clean Architecture Intent**
- **Core:** Domain entities + base abstractions (no external dependencies).  
- **Application:** Use case orchestration, DTO contracts, validation, result modeling.  
- **Infrastructure:** Implements persistence, identity adapters, repositories, query handlers.  
- **Web:** Composition root / delivery mechanism (wires everything; no business logic).  
- **Dependency direction:** Outer layers (Web, Infrastructure) depend inward (on Application/Core). Aligns with Clean Architecture (no inward dependency on Infrastructure).

**Additional Principles / Practices**
- **Separation of Concerns:** Read model (query handlers) vs write operations (services).  
- **Explicit Boundaries:** Identity & current user abstracted behind interfaces.  
- **Fail Fast:** Validation (FluentValidation) + `Result<T>` short-circuits error paths.  
- **Convention over Configuration:** Reflection-based query handler registration.  
- **Async Everywhere:** Avoids blocking I/O at data layer.  
- **Projection over Entity Exposure:** LINQ `Select` → DTOs prevents leaking persistence models.  
- **Minimal Knowledge (Law of Demeter leaning):** Controllers talk only to services/dispatchers.  
- **Consistency / Uniform Error Flow:** `Result<T>` unifies success/error propagation.

**Potential Improvements (Optional)**
- Move query handlers into Application to simplify Infrastructure (if desired).  
- Introduce command dispatcher for symmetry (if write complexity grows).  

### Data / Control Flow (Typical Request)
```
            ┌─────────────────────────┐
            │          Client         │
            └────────────┬────────────┘
                         │ HTTP Request
                         │
                ┌────────▼─────────┐
                │  MVC Controller  │
                └─────── ┬─────────┘
                         │ decides based on intent
           ┌─────────────┴──────────────┐
           │                            │
    (Command / Write)             (Query / Read)
           │                            │
   ┌───────────────┐          ┌──────────────────┐
   │  App Service  │          │ Query Dispatcher │
   │  (use-case)   │          └─────────┬────────┘
   └───────┬───────┘                    │
           │                            │ resolves handler
           │                            │
   ┌───────▼─────────┐         ┌────────▼────────┐
   │  Repositories   │         │  Query Handler  │
   │  (UnitOfWork)   │         │  (projection)   │
   └────────┬────────┘         └────────┬────────┘
            │                           │
            └─────────────┬─────────────┘
                          │
                  ┌───────▼────────┐
                  │    DbContext   │
                  │    (EF Core)   │
                  └───────┬────────┘
                          │ SQL
                          │
                  ┌───────▼────────┐
                  │    SQL Server  │
                  └───────┬────────┘
                          │
               ┌──────────▼───────────┐
               │  Mapping / Transform │
               │  Entity → DTO → VM   │
               └──────────┬───────────┘
                          │  
                 ┌────────▼────────┐
                 │   Razor View    │
                 └────────┬────────┘
                          │
                 ┌────────▼────────┐
                 │  HTTP Response  │
                 └─────────────────┘
```
---
### Project Structure
```
Doera Solution
│ 
├──Doera.Web Layer
│   │
│   ├── Program.cs / Startup configuration
│   ├── appsettings.json / appsettings.Development.json
│   ├── Extensions
│   │   └── Service / DB / view location extensions
│   ├── Features
│   │   ├── Account
│   │   │   ├── Controllers
│   │   │   └── ViewModels (login, register, other account VMs)
│   │   ├── TodoList
│   │   │   ├── Controllers / Razor pages (Index, Create, Edit)
│   │   │   ├── ViewModels (Create/Edit/List pages)
│   │   │   ├── Mapping helpers
│   │   │   ├── Components / Partials
│   │   │   └── Query / filter helpers
│   │   ├── TodoItem
│   │   │   ├── Controllers / Razor pages (Create/Edit)
│   │   │   ├── ViewModels (Create/Edit/other item VMs)
│   │   │   ├── Mapping helpers
│   │   │   └── Partials (_TodoItemFormFields.cshtml)
│   │   └── (additional features)
│   ├── Models
│   │   └── Legacy model binding classes (optional)
│   ├── Views
│   │   ├── _ViewImports.cshtml / _ViewStart.cshtml
│   │   ├── Shared
│   │   │   ├── Layouts (_Layout.cshtml)
│   │   │   ├── Partials (Navbar, Sidebar, Breadcrumbs, others)
│   │   │   └── Validation scripts / other shared views
│   │   ├── Home (Index.cshtml)
│   │   ├── Account (Login/Register/AccessDenied)
│   │   ├── TodoList (Index + partials)
│   │   ├── TodoItem (feature-specific pages)
│   │   └── Error (Error.cshtml + status pages)
│   └── wwwroot
│       ├── lib (jQuery, Bootstrap, validation libs)
│       ├── css (site.css)
│       ├── js (site.js, sidebar.js, breadcrumbs.js, others)
│       └── img (assets)
│
├──Doera.Application Layer
│   │
│   ├── Abstractions
│   │   └── Results
│   │       └── Common result / error handling types
│   ├── Caching
│   │   └── Cache keys and invalidators
│   ├── DTOs
│   │   ├── Common
│   │   │   └── Shared request/response types
│   │   ├── <Feature>
│   │   │   ├── DTOs (data transfer objects)
│   │   │   ├── Requests (input models)
│   │   │   └── Responses (output models)
│   │   └── ...
│   ├── Extensions
│   │   └── Service registration and validation pipeline extensions
│   ├── Interfaces
│   │   ├── Core
│   │   │   └── Query / dispatcher / handler abstractions
│   │   ├── Services
│   │   │   └── Application service interfaces per feature
│   │   └── Identity
│   │       └── Current user and identity service abstractions
│   ├── Services
│   │   └── Application service implementations (use-case orchestration)
│   ├── Specifications
│   │   └── Query / filter / business rule specifications
│   └── Validation
│       ├── <Feature>
│       │   └── Request validators per feature
│       └── ...
│
├──Doera.Infrastructure Layer
│   │
│   ├── Caching
│   │   └── MemoryCacheService (in-memory caching implementation)
│   ├── Data
│   │   ├── ApplicationDbContext (EF Core DbContext)
│   │   └── Configuration (entity configurations / fluent API)
│   ├── Extensions
│   │   └── Infrastructure / caching / query helpers
│   ├── Identity
│   │   ├── CurrentUser (ICurrentUser implementation)
│   │   └── IdentityService (IIdentityService implementation)
│   ├── Migrations
│   │   └── EF Core migrations + model snapshots
│   ├── Persistance
│   │   └── UnitOfWork (transaction management)
│   ├── Queries
│   │   ├── QueryDispatcher (mediator-like dispatcher)
│   │   ├── TagHandlers (query handlers for tags)
│   │   ├── TodoItemHandlers (query handlers for items)
│   │   └── TodoListHandlers (query handlers for lists)
│   ├── Repositories
│   │   └── Base + feature-specific repositories (Tag, TodoItem, TodoList)
│   └── Utilities
│       └── SlugGenerator (helper for consistent slugs)
│
└──Doera.Core Layer
    │
    ├── Constant (application-wide constants)
    ├── Entities
    │   ├── Base (Entity base classes + auditable/soft-delete interfaces)
    │   └── Feature entities (Tag, TodoItem, TodoItemTag, TodoList, User)
    ├── Enums (SortDirection, TodoPriority, TodoSort, TodoStatus)
    ├── Interfaces
    │   ├── ISpecification, IUnitOfWork
    │   └── Repositories (generic + feature-specific repository interfaces)
    └── Specifications (base / abstract specification classes)
```
## Configuration (Quick Reference)
| Key | Purpose | Notes |
|-----|---------|-------|
| ConnectionStrings:SqlConnection | SQL Server connection | Use env var / secrets |
| ElmahIo:Enabled | Toggle Elmah.io | Typically true only in prod |
| ElmahIo:ApiKey / ElmahIo:LogId | Error monitoring | Never commit |
| SendGrid:ApiKey | Outbound email | Optional |
| ASPNETCORE_ENVIRONMENT | Runtime environment | Development / Production |
---

## Features

- **User Authentication & Authorization**
  - Registration, Login, Logout (ASP.NET Core Identity with GUID user keys)
  - Authorization via `[Authorize]` on protected controllers
  - AccessDenied route + cookie auth customization
  - Current user abstraction (`ICurrentUser`) + `IdentityService`

- **Todo List Management**
  - Create / Edit / View lists (`TodoListController`, `TodoListService`)
  - Ordered lists (stored `Order` field & highest-order calculation)
  - Per-user isolation (userId scoping in queries)
  - Breadcrumb + sidebar context

- **Todo Item Management**
  - Create / Edit items (TodoItem feature pages)
  - Status, Priority, Start/Due dates, Archive semantics (fields like `ArchivedAt`)
  - Batch / filtered retrieval (filter model on list view)
  - Repository operations + targeted update methods (`ExecuteUpdateStatusAsync`, delete by list)
  - Inline statistics panel: completion %, overdue, due soon, priority & status distribution, tag usage

- **Tagging System**
  - Tags per user (`Tag` entity + unique (UserId, NormalizedName) index)
  - Tag association to items (`TodoItemTag`)
  - DTO projection for tags in item summaries
  - Slug / normalization helper (`SlugGenerator`, `ISlugGenerator`)

- **Query / Read Model (Hybrid CQRS)**
  - Lightweight query dispatcher (`IQueryDispatcher`, `QueryDispatcher`)
  - Reflection-based query handler registration
  - Direct EF Core projections in handlers (no repository overhead)
  - Result wrapper pattern for uniform success/error return

- **Validation**
  - FluentValidation pipeline (`AddApplicationValidation`) for create/update flows
  - Server-side model validation integration -> `Result<T>` → `ModelState` using `DataAnnotation`
  - Client-side jQuery + unobtrusive validation assets

- **Notifications / Toasts**
  - NToastNotify (Toastr) integration (position, progress bar, dedupe)
  - Usage in controllers/services for user feedback (success/failure flows)

- **Caching Strategy**
  - In-memory caching layer (`IMemoryCache`)
  - Abstraction: `ICacheService` / `AddAppCaching`
  - Targeted invalidation after writes (e.g. `_cache.InvalidateTodoLists(_currentUser)` in `TodoListService`)
  - Read path kept lean (queries hit DB directly to avoid stale projections)

- **Error Handling**
  - Global exception handling (production `UseExceptionHandler("/Error")`)
  - Status code re-execution (`/Error/{code}`)
  - Structured logging at controller/service boundaries (debug/info/warn)
  - Validation and domain errors surfaced via `Result` error list
  - NotFound / AccessDenied guarded responses

- **Logging & Audit Signals**
  - `ILogger<T>` instrumentation across controllers/services
  - Entity timestamps (`CreatedAt`, `UpdatedAt`, soft-state fields like `DeletedAt`, `ArchivedAt`)
  - Activity visibility through log messages (create/update/list operations)
  - Optional Elmah.io integration (present; can be enabled for centralized error monitoring)

- **Infrastructure & Persistence**
  - EF Core (SQL Server) with code-first migrations
  - Auto migration execution on startup (`ApplyMigrationsAsync`)
  - Repository + Unit of Work abstractions; spec-based querying (`FindAsync(ISpecification)`)
  - Async-first data access (no sync blocking calls)

- **UI / UX Enhancements**
  - Modular feature folder layout (`Features/*`)
  - Breadcrumb component + dynamic naming (JS enhancement)
  - Sidebar + layout customization (custom view location expander)
  - Partial views & components for reuse (`_TodoItemFormFields`, `_Breadcrumbs`)

- **Cross-Cutting Utilities**
  - Slug generation abstraction
  - Centralized extension-based service registration
  - Reflection-based discovery (query handlers)

- **Email**
  - Pluggable SendGrid implementation (no-op if API key missing)
  - (Future) conditional email confirmation / external providers (Google planned)

- **Soft Lifecycle & State**
  - Soft-delete / archive fields for future activation (e.g. `DeletedAt`, `ArchivedAt`)
  - Order maintenance for lists & potential item ordering logic

---

## Domain & Data

#### Table: User (Identity + audit)
| Column              | Type              | Required | Notes                               |
|---------------------|-------------------|----------|-------------------------------------|
| Id                  | Guid              | Yes      | Primary key                         |
| UserName / Email    | nvarchar(256)     | Optional | Identity-managed indexes            |
| CreatedAt           | datetimeoffset    | Yes      | Audit                               |
| UpdatedAt           | datetimeoffset    | No       | Audit (nullable)                    |
| ConcurrencyStamp    | string            | No       | Optimistic concurrency (Identity)   |
| (Identity fields…)  | —                | —        | Standard Identity columns           |

#### Table: TodoList
| Column     | Type           | Required | Notes                                   |
|------------|----------------|----------|-----------------------------------------|
| Id         | Guid           | Yes      | PK                                      |
| UserId     | Guid           | Yes      | FK → User (cascade)                     |
| Name       | nvarchar(256)  | Yes      |                                         |
| Order      | int            | Yes      | Sequential ordering per user            |
| CreatedAt  | datetimeoffset | Yes      | Audit                                   |
| UpdatedAt  | datetimeoffset | No       | Audit                                   |

#### Table: TodoItem
| Column      | Type           | Required | Notes                                                         |
|-------------|----------------|----------|---------------------------------------------------------------|
| Id          | Guid           | Yes      | PK                                                            |
| UserId      | Guid           | Yes      | FK → User (cascade)                                           |
| TodoListId  | Guid           | Yes      | FK → TodoList (NoAction to preserve list even if item exists) |
| Title       | nvarchar(200)  | Yes      |                                                               |
| Description | nvarchar(max)  | No       |                                                               |
| Status      | string         | Yes      | Enum persisted as string                                      |
| Priority    | string         | Yes      | Enum persisted as string                                      |
| Order       | int            | Yes      | (If used for item ordering)                                   |
| StartDate   | datetimeoffset | No       | Scheduling                                                     |
| DueDate     | datetimeoffset | No       | Scheduling                                                     |
| ArchivedAt  | datetimeoffset | No       | Soft archival                                                  |
| DeletedAt   | datetimeoffset | No       | Soft delete placeholder                                       |
| CreatedAt   | datetimeoffset | Yes      | Audit                                                         |
| UpdatedAt   | datetimeoffset | No       | Audit                                                         |

#### Table: Tag
| Column         | Type           | Required | Notes                                      |
|----------------|----------------|----------|--------------------------------------------|
| Id             | Guid           | Yes      | PK                                         |
| UserId         | Guid           | Yes      | FK → User (cascade)                        |
| DisplayName    | nvarchar(128)  | Yes      | Shown to user                              |
| NormalizedName | nvarchar(128)  | Yes      | Upper/slug variant; uniqueness enforcement |
| CreatedAt      | datetimeoffset | Yes      | Audit                                      |
| UpdatedAt      | datetimeoffset | No       | Audit                                      |

#### Table: TodoItemTag (Join)
| Column     | Type  | Required | Notes                           |
|------------|-------|----------|---------------------------------|
| Id         | Guid  | Yes      | PK (surrogate)                  |
| TodoItemId | Guid  | Yes      | FK → TodoItem (cascade)         |
| TagId      | Guid  | Yes      | FK → Tag (NoAction)             |
| (Unique)   |       |          | Composite index (TodoItemId,TagId) |

### ER Diagram
<img src="https://i.imgur.com/imYonk0.png" alt="ER Diagram" style="max-width: 700px;">

### Data & Persistence
- **DbContext**
  - `ApplicationDbContext` inherits `IdentityDbContext<User, IdentityRole<Guid>, Guid>`
  - Exposes `TodoLists`, `TodoItems`, `Tags`, `TodoItemTags` + all Identity tables

- **Configuration**
  - Auto-loads entity configurations via `builder.ApplyConfigurationsFromAssembly(...)`
  - Enforces constraints, relationships, and indexes

- **Migrations**
  - EF Core code-first
  - Pending migrations automatically executed at startup (`ApplyMigrationsAsync()`)

- **Transactions**
  - Standard writes use `IUnitOfWork.CompleteAsync()` → `SaveChangesAsync()` (implicit transaction support)
  - Multi-step flows use `IUnitOfWork.BeginTransactionAsync(...)` for explicit transactions, commit & rollback

- **Concurrency**
  - Identity entities use `ConcurrencyStamp` for optimistic concurrency
---
## Learning Highlights

### What I Practiced
- Structuring a multi-project .NET solution (Web / Application / Infrastructure / Core) with clear dependency flow.
- Implementing a hybrid CQRS approach: repositories + services for writes, lightweight query handlers for reads.
- Applying FluentValidation for request DTO validation and surfacing errors consistently through a `Result<T>` wrapper.
- Enforcing domain rules with composite indexes (unique tags per user, unique tag links).
- Using EF Core effectively: projections, execute update/delete batch methods, migrations auto-run at startup.
- Abstracting identity concerns (current user + auth workflows) instead of coupling controllers to the framework.
- Designing DTOs and view models that avoid entity leakage and keep Razor views clean.
- Introducing caching selectively (invalidate list cache after writes) without premature optimization.
- Building extension-based service registration for a clean `Program.cs`.
- Applying structured logging (debug/info/warn) to trace request and domain flow.

### Challenges Overcome
- Balancing “pure” Clean Architecture against pragmatic needs (allowing Infrastructure → Application for query DTOs).
- Deciding when to bypass repositories for read performance (query handlers projecting directly).
- Managing user scoping everywhere (never leaking another user’s data) without adding accidental gaps.
- Handling validation layering (FluentValidation + ModelState + domain invariants) without duplicating rules.
- Maintaining ordering logic for lists while keeping the write path simple.
- Avoiding over-engineering (did not prematurely add a full mediator for commands or a Contracts layer).
- Keeping soft lifecycle fields (ArchivedAt / DeletedAt) without yet enabling global filters-future-proofing without clutter.
- Debugging EF Core migration + auto-apply timing during startup, ensuring failures fail fast.

### Key Takeaways
- Clear layering with room for a Contracts module later.
- Hybrid CQRS delivers value now; path to full CQRS remains open.
- DTO projections prevent entity leakage and ease future API exposure.
- Consistent validation + Result pattern standardizes error handling.
- Identity abstraction decouples business logic from framework plumbing.
- Extension-based DI organization improves maintainability.
- Constraints and indexes enforce domain rules early.
- Logging + structure prepare for centralized observability.
- Intentional scope: no premature domain events or rowversion until justified.


## License 
### MIT / Apache-2.0 / Proprietar
