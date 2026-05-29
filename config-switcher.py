import sys
import shutil
import json
import logging
from datetime import datetime
from pathlib import Path
from typing import Dict, List, Callable, Union
from pydantic import BaseModel, field_validator


logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)-5s] %(message)s",
    datefmt="%Y-%m-%d %H:%M:%S",
    stream=sys.stdout,
)
logger = logging.getLogger("config-switcher")


class PathMapping(BaseModel):
    label: str
    target: Path
    local: Path
    public: Path

    @field_validator('target', 'local', 'public', mode='before')
    @classmethod
    def ensure_path(cls, v: Union[str, Path]) -> Path:
        return Path(v)


BASE_DIR = Path(__file__).resolve().parent

PUBLIC_HOST = "bluebridge.homeonthewater.com"
LOCAL_HOST = "localhost"

PATHS: List[PathMapping] = [
    PathMapping(
        label="SERVER / config.json",
        target=BASE_DIR / "SERVER" / "config.json",
        local=BASE_DIR / "configs" / "server-config.json.local",
        public=BASE_DIR / "configs" / "server-config.json.public",
    ),
    PathMapping(
        label="Resources / config.json",
        target=BASE_DIR / "Minionsparadise" / "Assets" / "Resources" / "config.json",
        local=BASE_DIR / "configs" / "server-config.json.local",
        public=BASE_DIR / "configs" / "server-config.json.public",
    ),
    PathMapping(
        label="Resources / client-config.json",
        target=BASE_DIR / "Minionsparadise" / "Assets" / "Resources" / "client-config.json",
        local=BASE_DIR / "configs" / "client-config.json.local",
        public=BASE_DIR / "configs" / "client-config.json.public",
    ),
]


def ts() -> str:
    return datetime.now().strftime("%Y-%m-%d  %H:%M:%S")


def divider(char: str = "─", width: int = 52) -> None:
    logger.info(char * width)


def log_info(msg: str) -> None:
    logger.info(msg)


def log_ok(msg: str) -> None:
    logger.info(msg)


def log_warn(msg: str) -> None:
    logger.warning(msg)


def log_error(msg: str) -> None:
    logger.error(msg)


def log_step(msg: str) -> None:
    logger.info(f">> {msg}")


def validate_json(path: Path) -> bool:
    try:
        with open(path, "r", encoding="utf-8") as f:
            json.load(f)
        return True
    except json.JSONDecodeError:
        return False


def cmd_switch(mode: str) -> None:
    divider("=")
    logger.info(f"CONFIG SWITCHER -- switching to {mode.upper()}")
    logger.info(ts())
    divider("=")

    errors: List[str] = []
    success: List[str] = []

    log_step(f"Copying {mode} config files")
    divider()

    for entry in PATHS:
        source: Path = getattr(entry, mode)
        target: Path = entry.target
        label: str = entry.label

        log_info(label)

        if not source.exists():
            log_error(f"Source not found -> {source}")
            errors.append(label)
            continue

        if not validate_json(source):
            log_error(f"Invalid JSON -> {source}")
            errors.append(label)
            continue

        target.parent.mkdir(parents=True, exist_ok=True)

        try:
            shutil.copy2(source, target)
            log_ok(f"Written -> {target.relative_to(BASE_DIR)}")
            success.append(label)
        except OSError as e:
            log_error(f"Copy failed -> {e}")
            errors.append(label)

    total = len(PATHS)
    passed = len(success)
    failed = len(errors)

    divider()
    log_step("Summary")
    divider()
    log_info(f"Total     : {total}")
    log_ok(f"Succeeded : {passed}")
    if failed:
        log_error(f"Failed    : {failed}")
    divider("=")

    if failed == 0:
        logger.info(f"[DONE] All configs switched to {mode.upper()}.")
    elif passed == 0:
        logger.error("[FAIL] Switch failed -- no files were updated.")
        sys.exit(1)
    else:
        logger.warning(f"[WARN] Partial switch: {passed}/{total} targets updated.")
        sys.exit(2)


def cmd_status() -> None:
    divider("=")
    logger.info("CONFIG STATUS")
    logger.info(ts())
    divider("=")

    for entry in PATHS:
        target: Path = entry.target
        label: str = entry.label
        rel: Path = target.relative_to(BASE_DIR)

        if not target.exists():
            logger.warning(f"[MISSING] {label} -> {rel}")
            continue

        try:
            content: str = target.read_text(encoding="utf-8")
        except OSError as e:
            logger.error(f"[ERROR] {label} -> {e}")
            continue

        if PUBLIC_HOST in content:
            tag = "PUBLIC"
            host = PUBLIC_HOST
        elif LOCAL_HOST in content:
            tag = "LOCAL"
            host = LOCAL_HOST
        else:
            tag = "UNKNOWN"
            host = "unknown"

        logger.info(f"[{tag}] {label} -> {host} ({rel})")

    divider("=")


def cmd_help() -> None:
    logger.info(f"""
Usage:
  python config-switcher.py local    switch all configs to LOCAL  (localhost)
  python config-switcher.py public   switch all configs to PUBLIC ({PUBLIC_HOST})
  python config-switcher.py status   show current environment per config file
  python config-switcher.py help     show this message
""")


COMMANDS: Dict[str, Callable[[], None]] = {
    "local": lambda: cmd_switch("local"),
    "public": lambda: cmd_switch("public"),
    "status": cmd_status,
    "help": cmd_help,
    "--help": cmd_help,
    "-h": cmd_help,
}

if __name__ == "__main__":
    if len(sys.argv) < 2 or sys.argv[1] not in COMMANDS:
        logger.error("Unknown or missing command.")
        cmd_help()
        sys.exit(1)

    try:
        COMMANDS[sys.argv[1]]()
    except ValidationError as e:
        logger.error(f"Configuration validation failed: {e}")
        sys.exit(1)
