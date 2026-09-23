# Database Schema

The Kampai Game Server uses a SQLite database to persist all player progress, social events, team structures, and chat history. The database file is located at `c:\Unity\SERVER\player_data\players.db`.

---

## 1. Table: `players`

This is the primary table containing all persistent player data.

### Identification & Versioning
- **`uid`** (TEXT, PK): The "Master" User ID. When accounts are linked (e.g., via Discord), this field stores the primary UID or consolidated comma-separated linked IDs.
- **`ID`** (TEXT): The internal numeric ID of the player, mirroring the active `uid`.
- **`version`** (INTEGER): The data schema version of the player profile.
- **`nextId`** (INTEGER): Instance ID counter for unique item/minion instances.

### Game State (JSON Blobs)
- **`inventory`**: Full player inventory JSON array (currencies, buildings, items, minions).
- **`unlocks`**: List of unlocked items or features.
- **`villainQueue`**: Current queue of pending villain actions.
- **`purchasedSales`**: Purchase tracking history for sales and store packs.
- **`socialRewards`**: Claims for social-based rewards.
- **`triggers`**: State for game-side tutorial or event triggers.
- **`pendingTransactions`**: Tracked but unconfirmed transactions.
- **`PlatformStoreTransactionIDs`**: Store-side validation receipts.
- **`helpTipsTrackingData`**: In-game help/tutorial status.
- **`mtxPurchaseTracking`**: Microtransaction telemetry tracking.

### Progression & Metrics
- **`PlayerLevel`**: Current recalculated level of the player.
- **`xp`**: Experience points.
- **`completedOrders`**: Total completed orderboard tasks.
- **`completedQuestsTotal`**: Total completed quests.
- **`highestFtueLevel`**: Highest completed level of First-Time User Experience.
- **`Time_played`** / **`totalAccumulatedGameplayDuration`**: Total gameplay duration in seconds.
- **`totalGameplayDurationSinceLastLevelUp`**: Gameplay duration since last level up.
- **`lastLevelUpTime`**: Timestamp of last level up.
- **`firstGameStartTime`** / **`lastGameStartTime`**: Session timestamps.
- **`targetExpansionID`**: Island land expansion index.
- **`currentItemCount`**: Quick count of items.

### Identity, Social & Dashboard
- **`name`**: Display name (Minion name, Discord username, or custom name).
- **`DISCORD`**: JSON blob containing Discord profile (`id`, `username`, `avatar`, and `uids`).
- **`discord_username`**: Normalized Discord handle.
- **`discord_avatar`**: Hash of Discord avatar.
- **`FACEBOOK`** / **`GOOGLE_PLAY`**: Associated identity strings for legacy logins.
- **`password`**: Optional password for the web player dashboard (`/dashboard`).
- **`custom_name`**: User-defined custom display name set from web dashboard.
- **`custom_avatar`**: User-selected custom avatar URL.

### Session & Timestamps
- **`last_updated`**: Automatic `CURRENT_TIMESTAMP` on row update.
- **`lastPlayedTime`**: Epoch timestamp (used by offline/online comparison logic; set to `2000000000` when forcing server overrides).
- **`country`**: Alpha-2 ISO country code.
- **`timezoneOffset`**: Player local timezone offset in seconds.

---

## 2. Table: `tse_teams` (Timed Social Events)

Stores collaborative teams for active TSE events.

- **`team_id`** (INTEGER, PK AUTOINCREMENT): Team identifier.
- **`event_id`** (INTEGER): Event definition ID from `definitions.json`.
- **`order_progress`** (TEXT): JSON array of completed order objects (`orderId`, `completedByUserId`).
- **`created_at`**: Automatic timestamp of team creation.

---

## 3. Table: `tse_members`

Tracks player memberships in TSE teams.

- **`id`** (INTEGER, PK AUTOINCREMENT): Member record ID.
- **`team_id`** (INTEGER, FK -> `tse_teams`): Associated team.
- **`user_id`** (TEXT): Player UID.
- **`reward_claimed`** (INTEGER, 0/1): Indicates if the user claimed the event rewards.
- **Constraint**: `UNIQUE(team_id, user_id)`.

---

## 4. Table: `tse_invitations`

Pending team invitations between players.

- **`id`** (INTEGER, PK AUTOINCREMENT): Invitation ID.
- **`event_id`** (INTEGER): Associated event ID.
- **`team_id`** (INTEGER, FK -> `tse_teams`): Team being invited into.
- **`inviter_uid`** (TEXT): Sending player UID.
- **`invitee_uid`** (TEXT): Receiving player UID.
- **`created_at`**: Creation timestamp.

---

## 5. Table: `global_chat`

Persistent message log for the in-game global chat system.

- **`id`** (INTEGER, PK AUTOINCREMENT): Message sequence ID.
- **`user_id`** (TEXT): Sender UID.
- **`message`** (TEXT): Content of message (up to 400 characters).
- **`timestamp`**: Automatic message timestamp.


---

## Data Management Notes
- **JSON Serialization**: Columns labeled as "Blobs" or "JSON" are stored as strings. The server logic uses `json.loads()` and `json.dumps()` with `ensure_ascii=False`.
- **UID Consolidation**: When accounts are linked, the `uid` column is updated to include all associated IDs (e.g., `"1001, 1002"`). The server uses `resolve_master_uid` in `utils/db.py` to handle these lookups.

