# Admin & Management Guide

This document describes the administrative and management tools available for the Kampai Game Server and player databases.

---

## 1. Web-Based Dashboards (`routes/dashboard.py`)

The server includes built-in web dashboards for administrative management and player self-service.

### A. Admin Dashboard (`/admin`)
- **URL**: `http://localhost:44733/admin`
- **Authentication**: Secured with `ADMIN_PASSWORD` (configured in `SERVER/config.py` or `.env`).
- **Features**:
  - **Player Overview**: Real-time listing of all registered players, levels, XP, total playtime, Discord linkages, and last update timestamps.
  - **Shop Schedule Manager**: Live inspection and dynamic modification of `ShopSchedule.json` (active sale packs, level requirements, purchase limits, and timestamps).
  - **Social Events Controller**: View current active Timed Social Events (TSE) and force regeneration of event cycles.

### B. Player Dashboard (`/dashboard`)
- **URL**: `http://localhost:44733/dashboard`
- **Features**:
  - Player inventory editing (modify quantities or add/delete items).
  - Save backup and upload (allows importing/exporting game state JSON).
  - Cross-account save migration (transfer progress between UIDs).
  - Christmas minion & event toggles per account.
  - Profile customization (name and avatar).

---

## 2. Server Configuration Switcher (`config-switcher.py`)

Located in the workspace root (`c:\Unity\config-switcher.py`), this GUI utility manages environment targets and configuration swapping:

- **Target Versions**: Select **Unity 5**, **Unity 6**, or **Both**.
- **Environments**: Toggle between **Local Development** (localhost:44733) and **Public Server** (external host/reverse proxy).
- **Paths Managed**:
  - `SERVER/config.json`
  - `UNITY5/Minionsparadise/Assets/Resources/config.json` & `config_server.json`
  - `UNITY6/Minionsparadise/Assets/Resources/config.json` & `config_server.json`
  - AppData client and server runtime config files.

---

## 3. Database Maintenance & Backups

The server persists all player profiles, TSE teams, and chat logs into SQLite:

- **Database Path**: `c:\Unity\SERVER\player_data\players.db`
- **Rolling Automated Backups**:
  - Managed by `SERVER/utils/db.py` background thread (`DBRollingBackupScheduler`).
  - Saved to `SERVER/player_data/backups/players_backup_YYYYMMDD_HHMMSS.db`.
  - Automatic rolling pruning retains the most recent 4 backups (configurable via `MAX_ROLLING_BACKUPS`).
- **Legacy Migration**: On startup, flat JSON files in `SERVER/player_data/` are automatically migrated into the SQLite database.
- **Save Comparison Tool**: Use `c:\Unity\tools\compare_saves.py` to inspect and diff player state files or DB exports.

