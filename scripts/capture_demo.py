"""Captura o app Windows nativo. Requer: pip install pywinauto Pillow.

Execute após compilar o Windows com WindowsAppSDKSelfContained=true.
O script interage apenas com o processo Rastro que ele inicia.
"""
from pathlib import Path
import subprocess
import sys
import time
import ctypes
from pywinauto import Application
from pywinauto import mouse
from PIL import Image

sys.stdout.reconfigure(encoding="utf-8")
root = Path(__file__).resolve().parents[1]
exe = root / "src/Rastro.App/bin/Debug/net10.0-windows10.0.19041.0/win-x64/Rastro.App.exe"
output = root / "docs"
output.mkdir(exist_ok=True)
process = subprocess.Popen([str(exe)], cwd=exe.parent, creationflags=subprocess.CREATE_NO_WINDOW)
frames = []

try:
    app = Application(backend="uia").connect(process=process.pid, timeout=20)
    window = app.window(title_re="Rastro.*")
    window.wait("visible", timeout=25)
    hwnd = window.handle
    ctypes.windll.user32.MoveWindow(hwnd, 0, 0, 1280, 710, True)
    window.set_focus()
    time.sleep(1)

    def snap(name=None):
        image = window.capture_as_image()
        if name:
            image.save(output / name)
        image.thumbnail((1100, 700), Image.Resampling.LANCZOS)
        frames.append(image.convert("RGB"))

    def button(identifier):
        return window.child_window(auto_id=identifier, control_type="Button").wrapper_object()

    snap("inicio.png")
    button("TrackButton").invoke()
    time.sleep(.35)
    assert "Digite o código" in " ".join(c.window_text() for c in window.descendants(control_type="Text"))
    snap()
    button("DemoCodeButton").invoke()
    time.sleep(1.5)
    window.child_window(auto_id="BackButton", control_type="Button").wait("exists", timeout=10)
    snap("rastreamento.png")
    button("RefreshButton").invoke()
    time.sleep(1.1)
    snap()
    button("CopyCodeButton").invoke()
    time.sleep(.3)
    snap()
    mouse.scroll(coords=(1100, 600), wheel_dist=-5)
    time.sleep(.4)
    snap("historico.png")
    mouse.scroll(coords=(1100, 400), wheel_dist=12)
    time.sleep(.3)
    ctypes.windll.user32.MoveWindow(hwnd, 0, 0, 420, 710, True)
    time.sleep(.5)
    snap("celular.png")
    ctypes.windll.user32.MoveWindow(hwnd, 0, 0, 1280, 710, True)
    time.sleep(.3)
    normalized = []
    for frame in frames:
        canvas = Image.new("RGB", (1100, 640), "#F7F8FA")
        frame.thumbnail((1100, 640), Image.Resampling.LANCZOS)
        canvas.paste(frame, ((1100-frame.width)//2, (640-frame.height)//2))
        normalized.append(canvas)
    normalized[0].save(output / "demonstracao.gif", save_all=True, append_images=normalized[1:],
                       duration=[2200] * len(normalized), loop=0, optimize=True)
    print(f"UI validada: busca vazia, consulta, detalhes, atualizar, copiar, histórico e layout móvel. {len(frames)} quadros capturados.")
finally:
    if process.poll() is None:
        process.terminate()
