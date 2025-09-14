# Phonebook (Interview Assessment)

A minimal microservices sample for a phonebook domain.

- **Contacts.API** (PostgreSQL, EF Core, seed + migrations)  
- **Reports.API** (Minimal API + SignalR Hub)  
- **Reports.Worker** (RabbitMQ consumer, MongoDB projection + report computation)  
- **Client (MVC)** (Contacts/Reports UI, SignalR alert)

> Full brief: **[docs/Assessment (Interview Pre-Assignment).pdf](docs/Assessment%20(Interview%20Pre-Assignment).pdf)**

---

## Prerequisites
- Docker & Docker Compose (for PostgreSQL, MongoDB, RabbitMQ containers)
- .NET 9 SDK
- (Optional) Visual Studio 2022

## Quick Start
1. **Start infrastructure:**
   ```bash
   docker compose up -d
   ```
2. **Visual Studio → Multiple startup projects** (order):  
   **Contacts.API → Reports.Worker → Reports.API → Client**
3. **Run (F5)**
> **Tip:** You can also run each project with `dotnet run --launch-profile "<ProjectName>"` from its folder, but Visual Studio (Multiple startup projects) is the recommended way.


**Runtime behavior:**
- **Contacts.API:** On first start, applies EF Core migrations (if any) and **seeds** `Contacts` & `ContactInfos` when empty.  
- **Reports.Worker:** **Seeds** Mongo collections (`contact_infos`, `reports`) when empty.  
- **Client:** UI for listing/creating/deleting contacts and viewing reports.  
- **SignalR:** When a report is Completed/Failed, a top-right **auto-fade** alert appears.

**Notes:**
- Ports/URLs are defined in each project’s `launchSettings.json`. Hub path: `/hubs/reports`.  
- If the Client runs on a different origin, CORS is configured in **Reports.API** `Program.cs`.

---

## Seed behavior
**Contacts.API (PostgreSQL):**
- Applies pending migrations (no-op if none).  
- Inserts a small dataset with fixed GUIDs into `Contacts` and `ContactInfos` **only if tables are empty**.

**Reports.Worker (MongoDB):**
- Inserts sample `contact_infos` and two `reports` (Completed / Failed) **only if collections are empty**.
