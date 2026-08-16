#!/usr/bin/env python3
"""
Generate responsive WebP variants for every static image under wwwroot/img.

Why this exists
---------------
The source art is already WebP at sane dimensions, but every page was shipping
the *full-size* file no matter how small it is drawn. A production-line card is
painted at 270 CSS px yet downloaded a 1200 px, 110 KB file — roughly 5x more
pixels than the screen can show. Multiply that by a dozen cards and the first
visit costs ~935 KB of images.

The fix is not to lower quality — it is to stop sending pixels the browser is
going to throw away. For each source we emit a ladder of widths at the SAME
encoder quality, write a manifest, and let ResponsiveImageTagHelper turn that
into a `srcset` so the browser picks the smallest file that still covers its
device pixel ratio.

Idempotent: variants whose mtime is newer than the source are skipped, so this
is cheap to re-run in a build step.

Usage:
    python3 tools/optimize-images.py            # generate
    python3 tools/optimize-images.py --report   # measure only, write nothing
    python3 tools/optimize-images.py --clean    # delete generated variants
"""
from __future__ import annotations

import argparse
import json
import os
import re
import sys
from pathlib import Path

try:
    from PIL import Image
except ImportError:
    sys.exit("Pillow is required:  pip install pillow")

ROOT = Path(__file__).resolve().parent.parent
IMG_DIR = ROOT / "wwwroot" / "img"
MANIFEST = ROOT / "wwwroot" / "img" / "variants.json"

# The ladder. 400 covers phone cards, 800 covers tablet/retina cards and
# desktop half-widths, 1200 covers large feature images, 1600/1920 cover the
# hero. A width is skipped when it would be >= the source (never upscale).
WIDTHS = [400, 800, 1200, 1600]

# Quality is deliberately identical to MediaService's uploader (82) so a
# resized variant is visually indistinguishable from the original at its own
# display size. method=6 is the slowest/best encoder search — this runs at
# build time, not per request, so the extra seconds are free.
QUALITY = 82
METHOD = 6

# Small images are already smaller than the first rung; resizing them only adds
# HTTP requests and cache pressure.
MIN_SOURCE_WIDTH = 500
MIN_SOURCE_BYTES = 12 * 1024

VARIANT_RE = re.compile(r"-(\d+)w\.webp$", re.IGNORECASE)
SKIP_DIRS = {"brand", "brands", "patterns"}  # logos/marks: tiny, and must stay crisp


def is_variant(path: Path) -> bool:
    return bool(VARIANT_RE.search(path.name))


def sources() -> list[Path]:
    out = []
    for p in sorted(IMG_DIR.rglob("*.webp")):
        if is_variant(p):
            continue
        rel_top = p.relative_to(IMG_DIR).parts[0] if len(p.relative_to(IMG_DIR).parts) > 1 else ""
        if rel_top in SKIP_DIRS:
            continue
        out.append(p)
    return out


def web_path(p: Path) -> str:
    return "/" + p.relative_to(ROOT / "wwwroot").as_posix()


def clean() -> int:
    n = 0
    for p in IMG_DIR.rglob("*.webp"):
        if is_variant(p):
            p.unlink()
            n += 1
    if MANIFEST.exists():
        MANIFEST.unlink()
    print(f"removed {n} generated variants")
    return 0


def run(report_only: bool) -> int:
    if not IMG_DIR.is_dir():
        sys.exit(f"missing {IMG_DIR}")

    manifest: dict[str, dict] = {}
    made = skipped = 0
    src_bytes = new_bytes = 0

    for src in sources():
        size = src.stat().st_size
        with Image.open(src) as im:
            w, h = im.size
            mode_rgba = im.mode in ("RGBA", "LA", "P")
            base = im.convert("RGBA" if mode_rgba else "RGB")

            entry = {"w": w, "h": h, "variants": []}

            if w < MIN_SOURCE_WIDTH or size < MIN_SOURCE_BYTES:
                # Still record intrinsic size — the tag helper uses it to emit
                # width/height and kill layout shift even without a srcset.
                manifest[web_path(src)] = entry
                continue

            src_bytes += size
            biggest_kept = 0

            for tw in WIDTHS:
                if tw >= w:
                    continue
                out = src.with_name(f"{src.stem}-{tw}w.webp")
                th = max(1, round(h * tw / w))

                if out.exists() and out.stat().st_mtime >= src.stat().st_mtime:
                    skipped += 1
                else:
                    if not report_only:
                        r = base.resize((tw, th), Image.LANCZOS)
                        r.save(out, "WEBP", quality=QUALITY, method=METHOD)
                        r.close()
                    made += 1

                if out.exists():
                    entry["variants"].append({"w": tw, "h": th, "url": web_path(out),
                                              "bytes": out.stat().st_size})
                    new_bytes += out.stat().st_size
                    biggest_kept = tw

            # The original always terminates the ladder so large viewports and
            # 2x/3x screens still have a full-resolution option.
            entry["variants"].append({"w": w, "h": h, "url": web_path(src), "bytes": size})
            manifest[web_path(src)] = entry

    if not report_only:
        MANIFEST.write_text(json.dumps(manifest, indent=1, sort_keys=True), encoding="utf-8")

    total_variant_bytes = sum(
        v["bytes"] for e in manifest.values() for v in e["variants"]
    )
    print(f"sources         : {len(manifest)}")
    print(f"variants written: {made}   (up to date: {skipped})")
    print(f"source weight   : {src_bytes/1024:,.0f} KB")
    print(f"on-disk total   : {total_variant_bytes/1024:,.0f} KB  (originals kept as the top rung)")
    if not report_only:
        print(f"manifest        : {web_path(MANIFEST)}")
    return 0


if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("--report", action="store_true", help="measure only, write nothing")
    ap.add_argument("--clean", action="store_true", help="delete generated variants")
    a = ap.parse_args()
    sys.exit(clean() if a.clean else run(a.report))
