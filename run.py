"""
EducationCenterSystem - Dev Automation Script
1. Terminate running processes and release ports (5145, 7102).
2. Clean compilation artifacts (bin, obj, publish, dotnet clean).
3. Build the entire solution (dotnet build).
4. Launch Backend (API) and wait for readiness on port 5145.
5. Launch Frontend (WPF).
"""

import argparse
import os
import shutil
import socket
import subprocess
import sys
import time
import urllib.request
from pathlib import Path

ROOT_DIR = Path(__file__).resolve().parent
SLN_FILE = ROOT_DIR / "EducationCenterSystem.sln"
API_PROJ = ROOT_DIR / "src" / "EducationCenterSystem.Api" / "EducationCenterSystem.Api.csproj"
FRONTEND_PROJ = ROOT_DIR / "src" / "EducationCenterSystem.Presentation" / "EducationCenterSystem.Presentation.WinForms.csproj"
FRONTEND_WINFORMS_PROJ = FRONTEND_PROJ

API_BIN = ROOT_DIR / "src" / "EducationCenterSystem.Api" / "bin" / "Debug" / "net9.0" / "EducationCenterSystem.Api.dll"
FRONTEND_BIN = ROOT_DIR / "src" / "EducationCenterSystem.Presentation" / "bin" / "Debug" / "net9.0-windows" / "EducationCenterSystem.Presentation.WinForms.dll"
FRONTEND_WINFORMS_BIN = FRONTEND_BIN

API_PORT = 5145
API_HEALTH_URL = f"http://127.0.0.1:{API_PORT}/swagger/index.html"


def log(msg: str):
    print(f"\n[DEV] {msg}", flush=True)


def handle_remove_readonly(func, path, exc_info):
    """Clear readonly flag and retry deletion."""
    import stat
    os.chmod(path, stat.S_IWRITE)
    func(path)


def stop_old_processes():
    """Kill lingering processes and release ports."""
    log("Step 1/5: Stopping lingering processes and releasing ports...")
    
    processes_to_kill = [
        "EducationCenterSystem.Api.exe",
        "EducationCenterSystem.Presentation.exe",
        "EducationCenterSystem.Presentation.WinForms.exe",
        "MSBuild.exe",
        "VBCSCompiler.exe",
    ]
    for proc in processes_to_kill:
        subprocess.run(["taskkill", "/F", "/IM", proc], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)

    # Free ports 5145 and 7102
    for port in [API_PORT, 7102]:
        try:
            cmd = f'netstat -aon | findstr ":{port}" | findstr "LISTENING"'
            output = subprocess.check_output(cmd, shell=True, text=True, stderr=subprocess.DEVNULL)
            for line in output.strip().splitlines():
                parts = line.split()
                if len(parts) >= 5:
                    pid = parts[-1]
                    subprocess.run(["taskkill", "/F", "/PID", pid], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        except subprocess.CalledProcessError:
            pass


def clean_artifacts():
    """Wipe bin, obj, publish folders and run dotnet clean."""
    log("Step 2/5: Cleaning old compilation artifacts (bin, obj, publish)...")

    # dotnet clean
    subprocess.run(
        ["dotnet", "clean", str(SLN_FILE), "-v", "quiet", "--nologo"],
        cwd=str(ROOT_DIR),
        check=False
    )

    # Wipe all bin and obj folders recursively inside src and tests
    deleted_count = 0
    for target_dir in ["src", "tests", "publish"]:
        base_path = ROOT_DIR / target_dir
        if not base_path.exists():
            continue

        if target_dir == "publish":
            try:
                shutil.rmtree(base_path, onerror=handle_remove_readonly)
                deleted_count += 1
            except Exception:
                pass
            continue

        for folder in base_path.rglob("*"):
            if folder.is_dir() and folder.name.lower() in ("bin", "obj"):
                try:
                    shutil.rmtree(folder, onerror=handle_remove_readonly)
                    deleted_count += 1
                except Exception:
                    pass

    print(f"       -> Removed {deleted_count} build artifact directories.", flush=True)


def build_projects(fast: bool = False):
    """Compile projects (fast mode compiles only Backend & Frontend, skipping test projects)."""
    if fast:
        log("Step 3/5: Fast compiling Backend & Frontend (skipping tests)...")
        # Build API (which transitively builds Domain, Application, Infrastructure)
        res_api = subprocess.run(
            ["dotnet", "build", str(API_PROJ), "--configuration", "Debug", "--nologo"],
            cwd=str(ROOT_DIR)
        )
        if res_api.returncode != 0:
            print("\n[ERROR] Backend compilation failed! Fix build errors and retry.", file=sys.stderr)
            sys.exit(res_api.returncode)

        # Build Frontend
        res_fe = subprocess.run(
            ["dotnet", "build", str(FRONTEND_PROJ), "--configuration", "Debug", "--nologo"],
            cwd=str(ROOT_DIR)
        )
        if res_fe.returncode != 0:
            print("\n[ERROR] Frontend compilation failed! Fix build errors and retry.", file=sys.stderr)
            sys.exit(res_fe.returncode)

        print("       -> Fast build completed successfully (Backend + Frontend).", flush=True)
    else:
        log("Step 3/5: Compiling entire solution (dotnet build)...")
        res = subprocess.run(
            ["dotnet", "build", str(SLN_FILE), "--configuration", "Debug", "--nologo"],
            cwd=str(ROOT_DIR)
        )
        if res.returncode != 0:
            print("\n[ERROR] Compilation failed! Fix build errors and retry.", file=sys.stderr)
            sys.exit(res.returncode)
        print("       -> Build completed successfully.", flush=True)


def is_port_open(host: str, port: int, timeout_sec: float = 0.5) -> bool:
    """Check if TCP port is accepting connections."""
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
        sock.settimeout(timeout_sec)
        return sock.connect_ex((host, port)) == 0


def wait_for_api_ready(timeout_seconds: int = 30) -> bool:
    """Wait until API starts listening and responds."""
    start_time = time.time()
    print("       -> Waiting for Backend to be ready", end="", flush=True)
    while time.time() - start_time < timeout_seconds:
        if is_port_open("127.0.0.1", API_PORT):
            # Attempt HTTP request to ensure app is responsive
            try:
                req = urllib.request.Request(API_HEALTH_URL, headers={"User-Agent": "DevRunner"})
                with urllib.request.urlopen(req, timeout=1.0) as resp:
                    if resp.status in (200, 301, 302):
                        print(" [READY]")
                        return True
            except Exception:
                # Port is open, warmup may be in progress
                print(" [PORT OPEN, WARMING UP]")
                return True
        print(".", end="", flush=True)
        time.sleep(1)
    print(" [TIMEOUT]")
    return False


def run_backend():
    """Launch Backend API in background."""
    log("Step 4/5: Starting Backend (API)...")
    creationflags = subprocess.CREATE_NEW_CONSOLE
    proc = subprocess.Popen(
        [
            "dotnet", "run",
            "--project", str(API_PROJ),
            "--no-build",
            "--no-restore"
        ],
        cwd=str(ROOT_DIR),
        creationflags=creationflags
    )
    
    ready = wait_for_api_ready(timeout_seconds=35)
    if not ready:
        print("[WARNING] Backend took longer than expected to respond, continuing to Frontend...")
    else:
        print(f"       -> Backend online: {API_HEALTH_URL}", flush=True)
    return proc


def run_frontend():
    """Launch Frontend Presentation (WinForms)."""
    log("Step 5/5: Starting Frontend (WinForms UI)...")
    creationflags = subprocess.CREATE_NEW_CONSOLE
    proc = subprocess.Popen(
        [
            "dotnet", "run",
            "--project", str(FRONTEND_PROJ),
            "--no-build",
            "--no-restore"
        ],
        cwd=str(ROOT_DIR),
        creationflags=creationflags
    )
    print("       -> WinForms window launched.", flush=True)
    return proc


def main():
    parser = argparse.ArgumentParser(description="EducationCenterSystem Runner")
    parser.add_argument("--fast", "-f", action="store_true", help="Fast run: build and run Backend & Frontend only (skips tests)")
    parser.add_argument("--clean", "-c", action="store_true", help="Force clean compilation artifacts (bin, obj, publish)")
    parser.add_argument("--build", "-b", action="store_true", help="Force rebuild projects")
    parser.add_argument("--winforms", "-w", action="store_true", help="Launch WinForms frontend (default)")
    parser.add_argument("mode", nargs="?", choices=["fast", "full"], help="Optional positional mode: 'fast' or 'full'")
    args = parser.parse_args()

    is_fast = args.fast or (args.mode == "fast")

    print("=" * 60)
    mode_text = "Fast Mode (Backend + Frontend, No Tests)" if is_fast else "Full Mode"
    print(f"   EducationCenterSystem - Dev Run Pipeline [{mode_text}]")
    print("=" * 60)
    
    stop_old_processes()

    if args.clean:
        clean_artifacts()
    else:
        log("Step 2/5: Cleaning compilation artifacts (bin, obj)... [SKIPPED]")

    binaries_missing = not (API_BIN.exists() and FRONTEND_BIN.exists())
    if args.build or binaries_missing or is_fast:
        build_projects(fast=is_fast)
    else:
        log("Step 3/5: Compiling solution (dotnet build)... [SKIPPED]")

    run_backend()
    run_frontend()
    
    log("Pipeline finished: Backend & Frontend are running.")


if __name__ == "__main__":
    main()

