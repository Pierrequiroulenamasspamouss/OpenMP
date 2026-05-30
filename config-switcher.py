import os
import json
import shutil
import tkinter as tk
from tkinter import ttk, messagebox, scrolledtext
from datetime import datetime

# Define base paths relative to this script
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))

# Source configuration files
CLIENT_LOCAL_SRC = os.path.join(SCRIPT_DIR, "configs", "client-config.json.local")
CLIENT_PUBLIC_SRC = os.path.join(SCRIPT_DIR, "configs", "client-config.json.public")

SERVER_LOCAL_SRC = os.path.join(SCRIPT_DIR, "configs", "server-config.json.local")
SERVER_PUBLIC_SRC = os.path.join(SCRIPT_DIR, "configs", "server-config.json.public")

# Target configuration files (supporting client, server, and AppData locations)
CLIENT_CONFIG_DESTS = [
    os.path.join(SCRIPT_DIR, "Minionsparadise", "Assets", "Resources", "config.json"),
    os.path.expandvars(r"%USERPROFILE%\AppData\LocalLow\Electronic Arts\Minions\local-config.json")
]

# In UNITY6 this includes config_server.json in Assets/Resources as well
SERVER_CONFIG_DESTS = [
    os.path.join(SCRIPT_DIR, "SERVER", "config.json"),
    os.path.join(SCRIPT_DIR, "Minionsparadise", "Assets", "Resources", "config_server.json"),
    os.path.expandvars(r"%USERPROFILE%\AppData\LocalLow\Electronic Arts\Minions\config.json")
]

class ConfigSwitcherApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Minions Paradise — Config Switcher (Unity 6)")
        self.root.geometry("700x560")
        self.root.resizable(False, False)
        
        # Color Palette (Catppuccin Mocha inspired)
        self.bg_color = "#1e1e2e"
        self.card_color = "#252538"
        self.fg_color = "#cdd6f4"
        self.subtext_color = "#a6adc8"
        self.local_color = "#a6e3a1"   # Green
        self.public_color = "#89b4fa"  # Blue
        self.accent_color = "#cba6f7"  # Purple
        self.border_color = "#313244"
        
        self.root.configure(bg=self.bg_color)
        
        # Configure Styles
        self.style = ttk.Style()
        self.style.theme_use('clam')
        self.style.configure("TFrame", background=self.bg_color)
        self.style.configure("Card.TFrame", background=self.card_color, relief="flat")
        self.style.configure("TLabel", background=self.bg_color, foreground=self.fg_color, font=("Segoe UI", 10))
        self.style.configure("Sub.TLabel", background=self.card_color, foreground=self.subtext_color, font=("Segoe UI", 9))
        self.style.configure("Header.TLabel", background=self.bg_color, foreground=self.accent_color, font=("Segoe UI", 16, "bold"))
        self.style.configure("CardHeader.TLabel", background=self.card_color, foreground=self.fg_color, font=("Segoe UI", 12, "bold"))
        
        self.style.configure("Local.TButton", font=("Segoe UI", 10, "bold"), background=self.local_color, foreground="#11111b")
        self.style.map("Local.TButton", background=[("active", "#b4befe")])
        
        self.style.configure("Public.TButton", font=("Segoe UI", 10, "bold"), background=self.public_color, foreground="#11111b")
        self.style.map("Public.TButton", background=[("active", "#b4befe")])
        
        self.build_ui()
        self.refresh_status()

    def build_ui(self):
        # Title Label
        title_label = ttk.Label(self.root, text="🌴 Minions Paradise Configuration Switcher (Unity 6)", style="Header.TLabel")
        title_label.pack(anchor="w", padx=25, pady=(20, 15))
        
        # Main Containers
        cards_frame = ttk.Frame(self.root, style="TFrame")
        cards_frame.pack(fill="x", padx=25, pady=5)
        
        # Client Config Card
        self.client_card = ttk.Frame(cards_frame, style="Card.TFrame", padding=15)
        self.client_card.grid(row=0, column=0, sticky="nsew", padx=(0, 10))
        
        ttk.Label(self.client_card, text="📱 Client Configuration", style="CardHeader.TLabel").pack(anchor="w", pady=(0, 5))
        self.client_status_badge = tk.Label(self.client_card, text="UNKNOWN", font=("Segoe UI", 10, "bold"), fg="#ffffff", bg="#585b70", padx=8, pady=2)
        self.client_status_badge.pack(anchor="w", pady=5)
        
        self.client_server_lbl = ttk.Label(self.client_card, text="Server: ---", style="Sub.TLabel")
        self.client_server_lbl.pack(anchor="w", pady=2)
        self.client_cdn_lbl = ttk.Label(self.client_card, text="CDN: ---", style="Sub.TLabel")
        self.client_cdn_lbl.pack(anchor="w", pady=2)
        
        # Server Config Card
        self.server_card = ttk.Frame(cards_frame, style="Card.TFrame", padding=15)
        self.server_card.grid(row=0, column=1, sticky="nsew", padx=(10, 0))
        
        ttk.Label(self.server_card, text="🖥️ Server Configuration", style="CardHeader.TLabel").pack(anchor="w", pady=(0, 5))
        self.server_status_badge = tk.Label(self.server_card, text="UNKNOWN", font=("Segoe UI", 10, "bold"), fg="#ffffff", bg="#585b70", padx=8, pady=2)
        self.server_status_badge.pack(anchor="w", pady=5)
        
        self.server_api_lbl = ttk.Label(self.server_card, text="DLC Manifest: ---", style="Sub.TLabel")
        self.server_api_lbl.pack(anchor="w", pady=2)
        self.server_video_lbl = ttk.Label(self.server_card, text="Video URL: ---", style="Sub.TLabel")
        self.server_video_lbl.pack(anchor="w", pady=2)
        
        # Grid weight configuration
        cards_frame.columnconfigure(0, weight=1)
        cards_frame.columnconfigure(1, weight=1)
        
        # Quick Switch Actions Frame
        actions_frame = ttk.Frame(self.root, style="TFrame")
        actions_frame.pack(fill="x", padx=25, pady=20)
        
        local_btn = ttk.Button(actions_frame, text="🔌 Switch to LOCAL (Localhost)", style="Local.TButton", command=self.switch_to_local)
        local_btn.grid(row=0, column=0, sticky="ew", padx=(0, 10), ipady=8)
        
        public_btn = ttk.Button(actions_frame, text="🌐 Switch to PUBLIC (Production)", style="Public.TButton", command=self.switch_to_public)
        public_btn.grid(row=0, column=1, sticky="ew", padx=(10, 0), ipady=8)
        
        actions_frame.columnconfigure(0, weight=1)
        actions_frame.columnconfigure(1, weight=1)
        
        # Logs frame
        log_frame = ttk.Frame(self.root, style="TFrame")
        log_frame.pack(fill="both", expand=True, padx=25, pady=(0, 20))
        
        ttk.Label(log_frame, text="📋 Activity Log", style="TLabel").pack(anchor="w", pady=(0, 5))
        self.log_widget = scrolledtext.ScrolledText(log_frame, bg="#11111b", fg="#a6adc8", font=("Consolas", 9), insertbackground="white", borderwidth=0, highlightthickness=1, highlightbackground="#313244")
        self.log_widget.pack(fill="both", expand=True)
        self.log_widget.config(state="disabled")
        
        self.log("Switcher utility started.")

    def log(self, message):
        self.log_widget.config(state="normal")
        timestamp = datetime.now().strftime("%H:%M:%S")
        self.log_widget.insert(tk.END, f"[{timestamp}] {message}\n")
        self.log_widget.see(tk.END)
        self.log_widget.config(state="disabled")

    def detect_config_type(self, filepath, indicator_key, indicator_val_substring):
        if not os.path.exists(filepath):
            return "MISSING", "File not found"
        try:
            with open(filepath, "r", encoding="utf-8") as f:
                data = json.load(f)
            
            # Helper to retrieve nested values
            val = data
            for key in indicator_key.split("."):
                if isinstance(val, dict):
                    val = val.get(key, None)
                else:
                    val = None
                    break
            
            if val is None:
                return "CUSTOM", "Unknown structure"
            
            if indicator_val_substring in str(val):
                return "LOCAL", str(val)
            else:
                return "PUBLIC", str(val)
        except Exception as e:
            return "ERROR", str(e)

    def refresh_status(self):
        # We determine client status based on the first existing destination file
        existing_client_path = next((p for p in CLIENT_CONFIG_DESTS if os.path.exists(p)), None)
        client_state, client_val = self.detect_config_type(
            existing_client_path or CLIENT_CONFIG_DESTS[0], 
            "server", 
            "localhost"
        )
        
        if client_state == "LOCAL":
            self.client_status_badge.config(text="LOCAL DEVELOPMENT", bg=self.local_color, fg="#11111b")
        elif client_state == "PUBLIC":
            self.client_status_badge.config(text="PUBLIC SERVER", bg=self.public_color, fg="#11111b")
        elif client_state == "MISSING":
            self.client_status_badge.config(text="FILE MISSING", bg="#f38ba8", fg="#11111b")
        else:
            self.client_status_badge.config(text="CUSTOM/ERROR", bg="#f9e2af", fg="#11111b")
            
        # Try loading extra details for client
        if existing_client_path:
            try:
                with open(existing_client_path, "r", encoding="utf-8") as f:
                    cdata = json.load(f)
                self.client_server_lbl.config(text=f"Server: {cdata.get('server', 'N/A')}")
                self.client_cdn_lbl.config(text=f"CDN URL: {cdata.get('cdn_url', 'N/A')}")
            except:
                self.client_server_lbl.config(text="Server: Error parsing file")
                self.client_cdn_lbl.config(text="CDN URL: Error parsing file")
        else:
            self.client_server_lbl.config(text="Server: File not found")
            self.client_cdn_lbl.config(text="CDN URL: File not found")

        # We determine server status based on the first existing destination file
        existing_server_path = next((p for p in SERVER_CONFIG_DESTS if os.path.exists(p)), None)
        server_state, server_val = self.detect_config_type(
            existing_server_path or SERVER_CONFIG_DESTS[0], 
            "allConfigs.anyDeviceType.dlcManifests.low", 
            "localhost"
        )
        
        if server_state == "LOCAL":
            self.server_status_badge.config(text="LOCAL ENVIRONMENT", bg=self.local_color, fg="#11111b")
        elif server_state == "PUBLIC":
            self.server_status_badge.config(text="PUBLIC ENVIRONMENT", bg=self.public_color, fg="#11111b")
        elif server_state == "MISSING":
            self.server_status_badge.config(text="FILE MISSING", bg="#f38ba8", fg="#11111b")
        else:
            self.server_status_badge.config(text="CUSTOM/ERROR", bg="#f9e2af", fg="#11111b")

        # Try loading extra details for server
        if existing_server_path:
            try:
                with open(existing_server_path, "r", encoding="utf-8") as f:
                    sdata = json.load(f)
                device_cfg = sdata.get("allConfigs", {}).get("anyDeviceType", {})
                low_dlc = device_cfg.get("dlcManifests", {}).get("low", "N/A")
                video_uri = device_cfg.get("videoUri", "N/A")
                self.server_api_lbl.config(text=f"DLC Manifest: ...{low_dlc[-40:] if len(low_dlc) > 40 else low_dlc}")
                self.server_video_lbl.config(text=f"Video URL: ...{video_uri[-40:] if len(video_uri) > 40 else video_uri}")
            except:
                self.server_api_lbl.config(text="DLC Manifest: Error parsing file")
                self.server_video_lbl.config(text="Video URL: Error parsing file")
        else:
            self.server_api_lbl.config(text="DLC Manifest: File not found")
            self.server_video_lbl.config(text="Video URL: File not found")

    def copy_file(self, src, dest, label):
        if not os.path.exists(src):
            self.log(f"⚠️ Error: Source file not found: {os.path.basename(src)}")
            return False
        
        try:
            # Ensure target directory exists
            os.makedirs(os.path.dirname(dest), exist_ok=True)
            shutil.copy2(src, dest)
            self.log(f"   -> Copied successfully to {dest}")
            return True
        except Exception as e:
            self.log(f"❌ Error copying to {dest}: {e}")
            return False

    def switch_to_local(self):
        self.log("Initiating switch to LOCAL environment...")
        client_successes = []
        for dest in CLIENT_CONFIG_DESTS:
            res = self.copy_file(CLIENT_LOCAL_SRC, dest, "Client Config")
            client_successes.append(res)
            
        server_successes = []
        for dest in SERVER_CONFIG_DESTS:
            res = self.copy_file(SERVER_LOCAL_SRC, dest, "Server Config")
            server_successes.append(res)
        
        self.refresh_status()
        if any(client_successes) or any(server_successes):
            messagebox.showinfo("Success", "Switch to LOCAL environment completed! Check log for copied destinations.")
        else:
            messagebox.showerror("Error", "Switch failed entirely. Check log for details.")

    def switch_to_public(self):
        self.log("Initiating switch to PUBLIC environment...")
        client_successes = []
        for dest in CLIENT_CONFIG_DESTS:
            res = self.copy_file(CLIENT_PUBLIC_SRC, dest, "Client Config")
            client_successes.append(res)
            
        server_successes = []
        for dest in SERVER_CONFIG_DESTS:
            res = self.copy_file(SERVER_PUBLIC_SRC, dest, "Server Config")
            server_successes.append(res)
        
        self.refresh_status()
        if any(client_successes) or any(server_successes):
            messagebox.showinfo("Success", "Switch to PUBLIC environment completed! Check log for copied destinations.")
        else:
            messagebox.showerror("Error", "Switch failed entirely. Check log for details.")

if __name__ == "__main__":
    root = tk.Tk()
    app = ConfigSwitcherApp(root)
    root.mainloop()
