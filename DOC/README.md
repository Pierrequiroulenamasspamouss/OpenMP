# Kampai Game & Server Documentation

Welcome to the official documentation for the **Minions Paradise** (Kampai) client projects and the shared **Kampai Game Server**. This directory contains technical references, guides, and architectural overviews for both Unity client versions and the backend server.

---

## Workspace & Version Structure

The repository is organized into distinct environments targeting different Unity versions alongside a shared backend:

| Component | Path | Unity / Tech Version | Purpose |
| :--- | :--- | :--- | :--- |
| **Unity 5 Client** | `UNITY5/` | Unity 5.3.5p4 | Legacy client codebase and native asset loading. |
| **Unity 6 Client** | `UNITY6/` | Unity 6000.3.17f1 | Modernized client codebase targeting Unity 6. |
| **Game Server** | `SERVER/` | Python 3 / Flask / SQLite | Shared local/public backend supporting both Unity 5 and Unity 6 clients. |
| **Config Switcher** | `config-switcher.py` | Python 3 (Tkinter) | GUI utility to toggle client & server configs between Local and Public server environments. |

---

## Server Architecture & Backend Services

The server is located in `c:\Unity\SERVER\` and provides essential backend services for both client versions:

- **Dual-Port Setup**:
  - **Main Port (`44733`)**: Handles primary REST traffic (game state, definitions, configs, market prices, TSE, dashboard/admin, and global chat).
  - **Secondary Port (`44732`)**: Dedicated secondary listener to handle auxiliary requests and prevent blocking main game traffic.
- **Persistence (`SERVER/utils/db.py`)**:
  - Primary storage: SQLite database at `SERVER/player_data/players.db`.
  - Automated rolling database backups with prune limits and startup validation.
  - Automatic migration from legacy flat JSON profiles in `player_data/` on startup.
- **Key Modules / Blueprints**:
  - **User Blueprint (`routes/user.py`)**: User authentication, session management, Discord OAuth2 integration, account conflict resolution, and offline-to-online transitions.
  - **Game Blueprint (`routes/game.py`)**: Game state persistence (`/rest/gamestate/<user_id>`), dynamic limited building & holiday event injection, definitions and configs serving, Swrve telemetry mocks, and Timed Social Events (TSE).
  - **Sales Blueprint (`routes/sales.py`)**: In-game shop schedule parsing (`ShopSchedule.json`), market pricing (`/rest/market_prices`), promotional bundle filtering, and announcement popups.
  - **Chat Blueprint (`routes/chat.py`)**: Real-time Global Chat endpoint (`/chat`, `/api/globalchat`) supporting both Unity form-encoded (`WWWForm`) and JSON formats.
  - **Dashboard & Admin Blueprint (`routes/dashboard.py`)**: Web-based player portal and administrative dashboard (`/dashboard`, `/admin`) for real-time player data editing, inventory management, event toggles, and shop schedule configuration.
  - **Metrics Blueprint (`routes/metrics.py`)**: Client telemetry and health check stubs.

---

## Documentation Index

### Core Guides & References
- **[API Reference](API_Reference.md)**: Detailed specification of all REST endpoints across all blueprints (`game`, `user`, `sales`, `chat`, `dashboard`, `metrics`).
- **[Admin & Management Guide](Admin_Guide.md)**: Server operation instructions, Web Admin Dashboard usage, CLI management, and SQLite database backup procedures.
- **[Database Schema](Database_Schema.md)**: Schema breakdown for SQLite tables (`players`, `tse_teams`, `tse_members`, `tse_invitations`, `global_chat`) and JSON blob structures.
- **[Data Formats](Data_Formats.md)**: Specifications for `definitions.json`, `config.json`, player state payloads, and marketplace definitions.

### Client Systems & Versioning
- **[Offline Mode Implementation Walkthrough](offline_mode_walkthrough.md)**: How offline caching, play offline buttons, preference toggles, and save synchronization work between client and server.
- **[Global Chat System](global_chat_system.md)**: Architecture of the in-game global chat system (StrangeIoC bindings, polling service, UI scaling).
- **[Global Chat API Reference](api_globalchat.md)**: REST API contract for global chat polling and message submission.
- **[Global Chat Unity Implementation Guide](guide_globalchat_implementation.md)**: Practical guide for integrating global chat via Unity WebRequest / coroutines.
- **[Restricted API Replacements & Wrappers](IO_Replacements.md)**: System.IO and SafeHandle wrapper details for multi-platform / WebPlayer compatibility.
- **[Asset Mapping & Texture Investigation](Asset_Mapping_Fixes.md)**: Asset manifest registration, DLC model paths, and shader fixes for night mode.
- **[Analysis of Changes & Troubleshooting](analysis_results.md)**: Historical analysis of StrangeIoC mediation and definition parsing fixes.

