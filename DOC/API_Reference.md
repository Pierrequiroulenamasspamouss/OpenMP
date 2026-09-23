# API Reference

This document provides a detailed technical reference for the Kampai Game Server API.

## Base URLs
- **Main Server**: `http://localhost:44733`
- **Secondary Server**: `http://localhost:44732`

---

## 1. User & Authentication (`user_bp`)

### Login & Session Initiation
- **Route**: `POST /rest/user/login` or `POST /rest/user/session`
- **Request Body**:
  ```json
  {
    "userId": "1000000001"
  }
  ```
- **Response**:
  ```json
  {
    "userId": "1000000001",
    "sessionId": "uuid-v4-string",
    "synergyId": "syn_1000000001",
    "isNewUser": false,
    "isTester": true,
    "country": "US",
    "tosVersion": "1.0",
    "privacyVersion": "1.0",
    "socialIdentities": []
  }
  ```

### Registration (Online / Offline Transition)
- **Route**: `POST /rest/user/register`
- **Request Body** *(optional)*: `{"userId": "OFFLINE_..."}` to migrate offline-created accounts.
- **Response**: Returns session token and confirms UserID.

### Identity Management & Conflict Resolution
- **Link Identity**: `POST /rest/v2/user/<user_id>/identity`
  - Links external identity (Discord). Returns HTTP `409` conflict if already linked to another player.
- **Unlink Discord**: `POST /rest/v2/user/<user_id>/discord/unlink`
  - Removes Discord association while preserving local player progress.
- **Forward Re-link**: `POST /rest/v2/user/<user_id>/identity/<anon_id>`
  - Overwrites and takes over the conflicting account.
- **Reverse Re-link**: `POST /rest/v2/user/<user_id>/identity/<anon_id>/reverseLink`
  - Keeps current player progress and moves the identity link.

### Discord Web Auth Flow
- **Initiate Login**: `GET /auth/discord/login?uid=<user_id>` (Redirects to Discord OAuth2).
- **OAuth Callback**: `GET /auth/discord/callback` (Processes Discord code & stages pending identity).
- **Status Check**: `GET /auth/discord/status?uid=<user_id>` (Polled by client to detect auth completion).

---

## 2. Game State & Resources (`game_bp`)

### Profile Persistence
- **Get Game State**: `GET /rest/gamestate/<user_id>`
  - Returns player state JSON. Automatically initializes from `empty_player.json` if user is new.
  - Dynamically patches active holiday events (e.g., Christmas Event item `110000`) and limited building unlocks.
- **Save Game State**: `POST /rest/gamestate/<user_id>`
  - Persists game state to SQLite DB. Syncs unlocks and special event items.
- **Reset Game State**: `POST /rest/gamestate/<user_id>/reset`

### Static Content & Definitions
- **Definitions**: `GET /rest/definitions/<filename>` — Serves active `definitions.json`.
- **Configuration**: `GET /rest/config/<path>` and `GET /configs/<path>` — Serves `data/config.json`.
- **DLC Manifest**: `GET /rest/dlc/manifests/<filename>` — Serves `DLC_Manifest.json`.
- **DLC Asset Bundles**: `GET /DLC/<path:filename>` — Serves extracted or standalone DLC assets.
- **Videos**: `GET /video.mp4` and `GET /videos/<filename>`.

### Leaderboard & Avatars
- **Leaderboard**: `GET /api/leaderboard` — Top 10 players ranked by Level and XP.
- **Player Avatar**: `GET /api/<uid>/icon.png` — Redirects to Discord avatar or placeholder.

### Timed Social Events (TSE)
- **Get Team State**: `GET /rest/tse/event/<event_id>/team/user/<user_id>`
- **Create Team**: `POST /rest/tse/event/<event_id>/team/user/<user_id>`
- **Join Team**: `POST /rest/tse/event/<event_id>/team/<team_id>/user/<user_id>/join`
- **Leave Team**: `POST /rest/tse/event/<event_id>/team/<team_id>/user/<user_id>/leave`
- **Invite Member**: `POST /rest/tse/event/<event_id>/team/<team_id>/user/<user_id>/invite?externalIds=<ids>`
- **Complete Order**: `POST /rest/tse/event/<event_id>/team/<team_id>/user/<user_id>/order`
- **Claim Event Reward**: `POST /rest/tse/event/<event_id>/team/<team_id>/user/<user_id>/reward`
- **Batch Query Teams**: `POST /rest/tse/event/<event_id>/teams`

---

## 3. Shop & Sales (`sales_bp`)

### Market Prices
- **Route**: `GET /rest/market_prices`
- **Response**: SKU to price strings (e.g. `{"com.ea.gp.minions.coins1": "$0.99"}`).

### Sales & In-Game Store
- **Route**: `GET /rest/sales/<user_id>/v2`
- **Logic**: Filters `salePackDefinitions` via `ShopSchedule.json` by player level, start/end dates, purchase counts, and nopromousers restrictions.
- **Admin Announcement Override**: `GET|POST /api/admin/announcement`

---

## 4. Global Chat (`chat_bp`)

Unified endpoint for sending and receiving in-game messages. Supports both JSON and Unity `WWWForm` submissions.

- **Polling Messages**: `GET /chat` or `GET /api/globalchat`
  - Query parameters: `limit` (default: 100), `since` (ISO timestamp).
  - Response: `{"messages": [{"user": "Stuart", "text": "Banana!", "timestamp": "..."}]}`
- **Posting Message**: `POST /chat` or `POST /api/globalchat`
  - Body: `{"userId": "1000000001", "message": "Hello!"}` or Form-encoded `user` & `text`.

---

## 5. Web Dashboard & Administrative API (`dashboard_bp`)

### User Self-Service Dashboard (`/dashboard`)
- **Login**: `POST /api/dashboard/login`
- **Set Password**: `POST /api/dashboard/set_password`
- **Export Save**: `GET /api/dashboard/backup_save?uid=<uid>&token=<token>`
- **Upload Save**: `POST /api/dashboard/upload_save`
- **Reset Save**: `POST /api/dashboard/reset_save`
- **Migrate Save**: `POST /api/dashboard/migrate_save`
- **View Inventory**: `GET /api/dashboard/inventory?uid=<uid>&token=<token>`
- **Modify Inventory Item**: `POST /api/dashboard/update_inventory_item`
- **Event Status**: `GET /api/dashboard/player_events_status?uid=<uid>&token=<token>`
- **Toggle Christmas Event**: `POST /api/dashboard/toggle_christmas_event`
- **Toggle Limited Buildings**: `POST /api/dashboard/toggle_limited_buildings`
- **Trigger Christmas Minion Offer**: `POST /api/dashboard/trigger_christmas_minion`

### Admin Management Portal (`/admin`)
- **Admin Login**: `POST /api/admin/login` (Uses `ADMIN_PASSWORD`)
- **List All Players**: `GET /api/admin/players`
- **Inspect Shop Schedule**: `GET /api/admin/shop_schedule`
- **Update Shop Schedule**: `POST /api/admin/shop_schedule`
- **Inspect Social Events**: `GET /api/admin/social_events`
- **Regenerate Events**: `POST /api/admin/social_events/regenerate`

---

## 6. Telemetry & Metrics (`metrics_bp`)

- **Telemetry Endpoint**: `POST /rest/telemetry` & `POST /metrics`
- **Health Metrics**: `GET|POST /rest/healthMetrics/`
- **Social Stubs**: `GET|POST /rest/social/`
- **Swrve Campaign Stubs**: `GET /api/1/user_resources_and_campaigns`, `GET /api/1/user_resources_diff`, `POST /1/batch`

