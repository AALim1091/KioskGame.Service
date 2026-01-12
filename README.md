#KioskGame Backend (Spin-to-Win)

A **.NET 8 Minimal API backend** for a kiosk-style **Spin-to-Win game**.  
This service manages player sessions, play limits, prize selection, and session expiration logic for a physical or web-based kiosk experience.

demonstrates clean backend architecture, service-oriented design, and API-first development using modern ASP.NET Core.


#Features

- Player login and session creation
- Configurable play limits per session
- Spin-to-win game logic
- Prize determination
- Session expiration handling
- RESTful API endpoints
- SQLite database with EF Core 8
- Clean separation of concerns
- Production-ready project structure

##Tech Stack

- C#
- .NET 8
- ASP.NET Core Minimal API
- Entity Framework Core 8
- SQLite
- Dependency Injection
- Visual Studio 2022


##Project Structure
KioskGame.Service
KioskGame.Service.Tests
*
KioskGame.Service
│
├── Common
│   
├── Data
│   
├── Dtos
│  
├── Endpoints
│ 
├── Extensions
│ 
├── Models
│ 
├── Routes
│ 
├── Services
│
├── Program.cs
└── appsettings.json


---

##API Endpoints

 Login / Start Session
POST /api/game/login/{playerId}

### Get Player Status
GET /api/game/status/{playerId}

### Play Spin-to-Win
POST /api/game/play/{playerId}

---

##Example API Response

{
    "success": true,
    "data": {
        "playerId": "50",
        "playsRemaining": 3,
        "isSessionExpired": false,
        "sessionExpires": null,
        "expirationReason": null
    },
    "error": null,
    "statusCode": 200
}

---

##Database

- Uses SQLite for lightweight persistence
- EF Core migrations included
- Stores player sessions, play counts, and expiration timestamps

---

##Running Locally

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022

##Steps
1. Clone the repository
2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Run EF Core migrations if required
5. Select kioskGame.Service as projet and run as https(play button at top) to run the API
6. Test endpoints using Postman or UI

---

##Security Notes
- Designed for kiosk-style usage
- No real authentication or user accounts
- No secrets committed to source control
- Intended for controlled environments
---

##Project Purpose
This project exists to demonstrate:
- Backend architecture and design
- Session-based game logic
- Service-layer separation
- Clean C# and .NET 8 practices
- API development for real-world use cases
---

##Planned Enhancements
- Admin configuration for prize probabilities
- Session caching (Redis)
- Frontend kiosk UI integration
- Cloud deployment
- Logging and monitoring
---

##License
A-aron Lim
This project is intended for educational and portfolio purposes.


##Project requirements:
1
Spin-to-Win or Scratch-to-Win Kiosk Mini-Game
Select a mini-game to develop: 1.) Spin-to-Win 2.) Scratch-to-Win
Expected Time Commitment
Estimated effort: 6–10 hours total.
Please do not feel pressure to go beyond this range. We are more interested in clear thinking, correctness, and code quality than completeness or polish.
A simple, well-structured solution is preferred over an overly complex one.
Purpose
This assignment evaluates backend and frontend fundamentals, business logic reasoning, code organization, integration between systems, and basic testing and documentation.
Overview
You will build a small kiosk-style promotional game with a backend API (C#/.NET) and a frontend kiosk UI (any modern framework). Players log in with a Player ID, receive limited plays, and win prizes based on weighted odds.
Core Rules
• Each player receives 3 plays per calendar day. • After the first play, a 5-minute session window begins. • All remaining plays must be used within that window. • If the session expires, remaining plays are lost.
Prize Types
At minimum support:
• No Prize • $5 Free Play • $10 Free Play • Food Voucher • Gift Item
Prize Weighting
Prizes must be awarded using weighted probabilities, not evenly.
Example: No Prize (50), $5 Free Play (25), $10 Free Play (15), Food Voucher (7), Gift Item (3).
One-Time Gift Item Rule
A player may only ever win one Gift Item in their lifetime.
2
If a player who already won a Gift Item lands on it again, the prize must be substituted with $10 Free Play. This rule does not reset daily and must be enforced server-side and persisted.
Backend Requirements
Use C# with .NET 8 or later.
Required endpoints:
POST /api/player/login GET /api/player/{id}/status POST /api/game/play Backend logic should live in testable service classes.
Persistence
Developers’ choice. For example: use NoSQL or similar; SQLite, SQL Server LocalDB, file-based persistence, or similar. Optional enhancement: use Redis or similar. Persist play history, daily plays used, session timing, and whether the player has ever won a Gift Item.
Testing
Include unit tests for:
• Session expiration logic • One-time Gift Item substitution • Prize weighting selection validity
Frontend Requirements
Any Modern Framework. IE., React, Blazor, Angular, or Vue. You may also use Swift or Kotlin. Design for Kiosk screens.
Screens required:
• Attract Screen • Login Screen • Game Screen (Spin or Scratch) • Result Screen • Session Expired Screen
Deliverables
1. Source code (backend + frontend)
2. README.md
3. Brief design notes (1 page max)
