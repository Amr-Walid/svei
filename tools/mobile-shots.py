"""Capture viewport-sized mobile screenshots (never full_page — that OOMs the 192MB renderer).
Scrolls in viewport-height steps and saves each frame separately as individual JPEGs."""
import sys, pathlib
from playwright.sync_api import sync_playwright

BASE = "http://127.0.0.1:5020"
ARGS = ["--single-process","--no-zygote","--disable-dev-shm-usage","--disable-gpu",
        "--js-flags=--max-old-space-size=192"]
OUT = pathlib.Path("/tmp/mob/shots"); OUT.mkdir(parents=True, exist_ok=True)

def shoot(path, tag, w, h, frames, sel=None):
    with sync_playwright() as pw:
        b = pw.chromium.launch(args=ARGS)
        try:
            ctx = b.new_context(viewport={"width": w, "height": h}, device_scale_factor=1,
                                is_mobile=True, has_touch=True)
            pg = ctx.new_page()
            pg.goto(BASE + path, wait_until="domcontentloaded", timeout=45000)
            pg.wait_for_timeout(1600)
            if sel:                                        # targeted element shot
                el = pg.query_selector(sel)
                if el:
                    el.scroll_into_view_if_needed(); pg.wait_for_timeout(500)
                    el.screenshot(path=str(OUT / f"{tag}.jpg"), quality=72, type="jpeg")
                    print("saved", tag)
            else:
                for i in range(frames):
                    pg.evaluate(f"window.scrollTo(0,{i*h})"); pg.wait_for_timeout(650)
                    pg.screenshot(path=str(OUT / f"{tag}-{i}.jpg"), quality=68, type="jpeg")
                    print("saved", f"{tag}-{i}")
            ctx.close()
        finally:
            b.close()

if __name__ == "__main__":
    tag, path = sys.argv[1], sys.argv[2]
    w = int(sys.argv[3]); h = int(sys.argv[4]); n = int(sys.argv[5])
    sel = sys.argv[6] if len(sys.argv) > 6 else None
    shoot(path, tag, w, h, n, sel)
