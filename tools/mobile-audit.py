"""Audit the public site at phone widths for horizontal overflow and tap-target problems.

Memory notes for this sandbox: the renderer only gets ~192 MB, so we
* relaunch the browser for EVERY page (reusing one browser across contexts
  dies with TargetClosedError),
* never use full_page screenshots,
* gate on domcontentloaded + a fixed settle because networkidle never fires
  (the brand marquee autoplays forever).
"""
import json, sys
from playwright.sync_api import sync_playwright

BASE = "http://127.0.0.1:5020"
ARGS = ["--single-process", "--no-zygote", "--disable-dev-shm-usage",
        "--disable-gpu", "--js-flags=--max-old-space-size=192"]

PAGES = ["/ar/", "/en/", "/ar/about", "/ar/products", "/ar/services",
         "/ar/production-lines", "/ar/brands", "/ar/gallery", "/ar/news",
         "/ar/events", "/ar/careers", "/ar/contact", "/ar/quote"]

# Find every element wider than the viewport, plus undersized tap targets.
PROBE = """() => {
  const vw = document.documentElement.clientWidth;
  const docW = document.documentElement.scrollWidth;
  const bad = [];
  for (const el of document.querySelectorAll('body *')) {
    const cs = getComputedStyle(el);
    if (cs.display === 'none' || cs.visibility === 'hidden') continue;
    const r = el.getBoundingClientRect();
    if (r.width === 0 && r.height === 0) continue;
    // overflowing past the right/left edge of the viewport
    const over = Math.max(r.right - vw, -r.left);
    if (over > 1.5) {
      bad.push({sel: el.tagName.toLowerCase() + (el.className && typeof el.className === 'string'
                 ? '.' + el.className.trim().split(/\\s+/).slice(0,3).join('.') : ''),
                w: Math.round(r.width), over: Math.round(over)});
    }
  }
  const small = [];
  for (const el of document.querySelectorAll('a, button, input, select, [role=button]')) {
    const cs = getComputedStyle(el);
    if (cs.display === 'none' || cs.visibility === 'hidden') continue;
    const r = el.getBoundingClientRect();
    if (r.width === 0 || r.height === 0) continue;
    if (r.height < 32 || r.width < 24) {
      small.push({sel: el.tagName.toLowerCase() + (el.className && typeof el.className === 'string'
                   ? '.' + el.className.trim().split(/\\s+/).slice(0,2).join('.') : ''),
                  w: Math.round(r.width), h: Math.round(r.height),
                  txt: (el.textContent || '').trim().slice(0, 24)});
    }
  }
  // dedupe by selector, keep the worst offender
  const dedupe = (arr, k) => {
    const m = new Map();
    for (const o of arr) { const p = m.get(o.sel); if (!p || o[k] > p[k]) m.set(o.sel, o); }
    return [...m.values()].sort((a, b) => b[k] - a[k]).slice(0, 12);
  };
  return {vw, docW, scroll: docW - vw, over: dedupe(bad, 'over'), small: dedupe(small, 'w')};
}"""

def audit(width, height, paths):
    out = {}
    for p in paths:
        with sync_playwright() as pw:
            b = pw.chromium.launch(args=ARGS)
            try:
                ctx = b.new_context(viewport={"width": width, "height": height},
                                    device_scale_factor=1, is_mobile=True, has_touch=True)
                pg = ctx.new_page()
                pg.goto(BASE + p, wait_until="domcontentloaded", timeout=45000)
                pg.wait_for_timeout(1400)
                out[p] = pg.evaluate(PROBE)
                ctx.close()
            except Exception as e:
                out[p] = {"error": str(e)[:160]}
            finally:
                b.close()
    return out

if __name__ == "__main__":
    w = int(sys.argv[1]) if len(sys.argv) > 1 else 390
    h = int(sys.argv[2]) if len(sys.argv) > 2 else 844
    sel = sys.argv[3:] or PAGES
    res = audit(w, h, sel)
    print(json.dumps({"viewport": f"{w}x{h}", "pages": res}, ensure_ascii=False, indent=1))
