from flask import Flask
import threading
import logging

from config import Config
from routes.user import user_bp
from routes.game import game_bp
from routes.metrics import metrics_bp
from routes.sales import sales_bp
from routes.dashboard import dashboard_bp
from routes.chat import chat_bp
from utils.db import init_db, migrate_files_to_db

# Disable verbose logs
log = logging.getLogger('werkzeug')
log.setLevel(logging.ERROR)

def create_app(port):
    import os
    template_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), 'html'))
    app = Flask(f"App_{port}", template_folder=template_dir)
    
    # Initialize DB
    init_db()
    # Migrate any legacy .json files
    migrate_files_to_db()
    
    # CRITICAL: Keep JSON order intact
    app.config['JSON_SORT_KEYS'] = False

    # Register Blueprints
    app.register_blueprint(user_bp)
    app.register_blueprint(game_bp)
    app.register_blueprint(metrics_bp)
    app.register_blueprint(sales_bp)
    app.register_blueprint(dashboard_bp)
    app.register_blueprint(chat_bp)

    from flask import request
    @app.before_request
    def log_request_info():
        print(f"[HTTP IN] {request.method} {request.url}", flush=True)

    return app

def run_server(port):
    app = create_app(port)
    print(f">>> Server started on port {port}", flush=True)
    app.run(host=Config.HOST, port=port, debug=Config.DEBUG, threaded=True)

if __name__ == '__main__':
    # Using a single run for the main port with debug=True for auto-reload
    # and a separate thread for the other port. 
    # Flask's reloader only works well in the main thread.
    
    def run_secondary():
        # Secondary port
        app_sec = create_app(Config.PORT_SECONDARY)
        print(f">>> Secondary Server started on port {Config.PORT_SECONDARY}", flush=True)
        app_sec.run(host=Config.HOST, port=Config.PORT_SECONDARY, debug=Config.DEBUG, threaded=True, use_reloader=False)

    t2 = threading.Thread(target=run_secondary, daemon=True)
    t2.start()

    # Main port
    app_main = create_app(Config.PORT_MAIN)
    print(f">>> Main Server started on port {Config.PORT_MAIN}", flush=True)
    app_main.run(host=Config.HOST, port=Config.PORT_MAIN, debug=Config.DEBUG, threaded=True, use_reloader=False)
