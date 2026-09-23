# Global Chat System

This document describes the implementation of the Global Chat system in Minions Paradise (Private Server).

## Overview
The Global Chat system allows players to send and receive real-time messages within the game. It is integrated into the **MODS** tab of the settings menu.

## Architecture
The system follows the **StrangeIoC** architecture used in the project.

### Components
1. **GlobalChatSignals.cs**:
   - `GlobalChatUpdateSignal`: Dispatched when new messages are polled from the server.
   - `GlobalChatErrorSignal`: Dispatched when a networking error occurs.

2. **IGlobalChatService.cs**:
   - Interface defining chat operations: `StartPolling()`, `StopPolling()`, `SendMessage(string text)`, `GetCachedMessages()`.
   - Data models: `ChatMessage` and `ChatResponse`.

3. **GlobalChatService.cs**:
   - Concrete implementation using Unity's `WWW` class for networking.
   - Polls the `/chat` endpoint of the private server every 5 seconds.
   - Sends messages via `WWWForm` POST to the same endpoint.
   - Identifies players as "Minion #" + Player ID (or "PlayerName" from PlayerPrefs if available).

4. **UIContext managed bindings**:
   - Service and Signals are registered in `UIContext.cs` as Singletons with `CrossContext()` capability.

5. **ModsPanelView.cs / ModsPanelMediator.cs**:
   - The UI is contained in the Mods panel.
   - The mediator manages the lifecycle of the service (starts polling when the menu is opened, stops when closed).

## API Endpoints
The system relies on the following endpoints on the Private Server (`GameConstants.Server.SERVER_URL`). Both `/chat` and `/api/globalchat` are aliases mapped to the same handler:
- `GET /chat` or `GET /api/globalchat`: Returns a JSON object with a list of the last 100 messages.
  ```json
  {
    "messages": [
      {"user": "Minion 123", "text": "Hello World!", "timestamp": "2026-03-27 12:00:00"}
    ]
  }
  ```
- `POST /chat` or `POST /api/globalchat`: Sends a new message.
  - Supports `WWWForm` (fields: `user`/`userId`, `text`/`message`) as well as raw JSON payloads (`{"userId": "...", "message": "..."}`).


## UI Scaling Fix
In addition to the chat system, the following files were modified to allow custom button scaling in the UI:
- `KampaiScrollView.cs`: Removed hardcoded `localScale = Vector3.one`.
- `HUDSettingsMenuPanelMediator.cs`: Removed scale reset for settings buttons.
- `ItemListMediator.cs`: Removed scale reset for item slots.
This allows the user to resize prefabs in the Unity Editor without the game resetting them to full size at runtime.
