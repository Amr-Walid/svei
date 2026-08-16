#!/usr/bin/env python3
"""
Subset the Font Awesome webfonts down to the icons this site can actually show.

Why
---
Font Awesome Free ships 2,575 glyphs across three files (~295 KB) and the site
renders roughly 130 of them. Every visitor was downloading the entire icon
catalogue before the page could finish painting.

Why this is safe even though icons are admin-editable
-----------------------------------------------------
Icon names are free text in the admin (FieldKind.Icon), so a strict "only the
icons currently on the site" subset would silently break the day someone picks a
new one. Two things prevent that:

  1. The keep-list is the union of  (a) every icon hard-coded in a view,
     (b) every icon currently stored in the database, and
     (c) a curated SAFELIST of common industrial/UI icons an editor is likely to
         reach for.
  2. tools/verify-icons.py re-checks the database against the subset, so a
     missing glyph is caught by the test suite rather than by a visitor.

Re-run this after adding icons:  python3 tools/subset-icons.py

The originals are preserved as *.full.woff2 so the subset can always be rebuilt
or reverted without re-downloading Font Awesome.
"""
from __future__ import annotations

import re
import shutil
import sqlite3
import sys
from pathlib import Path

try:
    from fontTools import subset
    from fontTools.ttLib import TTFont
except ImportError:
    sys.exit("fonttools is required:  pip install 'fonttools[woff]'")

ROOT = Path(__file__).resolve().parent.parent
FA_DIR = ROOT / "wwwroot" / "lib" / "fontawesome"
CSS = FA_DIR / "css" / "all.min.css"
FONTS = FA_DIR / "webfonts"
DB = ROOT / "svei.db"

FILES = {
    "solid": "fa-solid-900.woff2",
    "regular": "fa-regular-400.woff2",
    "brands": "fa-brands-400.woff2",
}

# Icons an editor may plausibly choose that are not on the site today. Keeping
# them costs a few KB and removes the main risk of subsetting a font whose
# contents are chosen at runtime.
SAFELIST = """
check check-double xmark plus minus circle-check circle-xmark circle-info
circle-question circle-exclamation triangle-exclamation ban info question
star star-half heart thumbs-up thumbs-down bell bookmark flag tag tags
arrow-right arrow-left arrow-up arrow-down chevron-right chevron-left
chevron-up chevron-down angle-right angle-left caret-down caret-up
up-right-from-square arrow-right-arrow-left rotate rotate-right share-nodes
house building building-columns industry warehouse city store shop
gear gears sliders wrench screwdriver-wrench hammer toolbox
microchip memory server database hard-drive network-wired wifi signal
bolt plug battery-full solar-panel lightbulb fire droplet leaf recycle
seedling globe earth-americas location-dot map map-pin route truck ship plane
train bus box box-open boxes-packing pallet dolly warehouse barcode qrcode
users user user-tie user-group people-group handshake handshake-angle
briefcase graduation-cap chalkboard-user certificate award trophy medal
chart-line chart-bar chart-pie chart-column gauge-high bullseye target
clipboard-list clipboard-check list-check table-list file-lines file-pdf
file-invoice file-contract folder-open envelope envelope-open paper-plane
phone phone-volume mobile-screen headset comments comment-dots message
calendar calendar-days calendar-check clock hourglass-half stopwatch
shield-halved lock unlock key fingerprint eye eye-slash user-shield
magnifying-glass filter sort arrow-up-wide-short percent tags money-bill
credit-card cart-shopping bag-shopping receipt coins scale-balanced
wrench cogs cube cubes layer-group diagram-project sitemap code laptop-code
desktop mobile tablet print camera image images video play circle-play
gem crown ribbon rocket lightbulb wand-magic-sparkles palette brush
temperature-half wind snowflake sun moon cloud water flask vial atom
heart-pulse hand-holding-heart hands-holding-child suitcase-medical
pen pen-ruler pen-to-square trash arrow-up-from-bracket download upload
link chain paperclip copy floppy-disk expand compress maximize
bars ellipsis grip-vertical square check-square list list-ul list-ol
""".split()

BRANDS_SAFELIST = """
facebook facebook-f instagram linkedin linkedin-in x-twitter twitter youtube
whatsapp telegram tiktok snapchat pinterest github gitlab google apple
android microsoft windows amazon behance dribbble medium reddit vimeo
threads discord slack skype weixin wechat line viber
""".split()


def icons_from_css() -> dict[str, dict[str, int]]:
    """Map every fa-<name> to its codepoint, per style, from the shipped CSS."""
    text = CSS.read_text(encoding="utf-8", errors="ignore")

    # Font Awesome 6 minified CSS declares each icon as a custom property and
    # groups aliases on one selector:
    #     .fa-user-times,.fa-user-xmark{--fa:"\f235"}
    # Older builds used `content:"\f235"` instead, so accept both.
    cp: dict[str, int] = {}
    for names, hexcode in re.findall(
        r'((?:\.fa-[a-z0-9-]+(?:::?before)?\s*,\s*)*\.fa-[a-z0-9-]+(?:::?before)?)'
        r'\s*\{[^}]*?(?:--fa|content)\s*:\s*"\\([0-9a-fA-F]+)"',
        text,
    ):
        for n in re.findall(r'\.fa-([a-z0-9-]+)', names):
            cp[n] = int(hexcode, 16)
    return cp


def icons_from_views() -> set[str]:
    found: set[str] = set()
    pat = re.compile(r'fa-([a-z0-9-]+)')
    skip = {"solid", "regular", "brands", "light", "thin", "duotone", "fw", "lg",
            "sm", "xs", "spin", "pulse", "beat", "shake", "flip", "rotate",
            "stack", "border", "pull-left", "pull-right", "inverse", "li", "ul",
            "2x", "3x", "4x", "5x", "suggest", "1x", "10x", "xl", "2xl"}
    for folder in ("Views", "Areas", "wwwroot/js", "Data"):
        base = ROOT / folder
        if not base.is_dir():
            continue
        for f in base.rglob("*"):
            if f.suffix.lower() not in (".cshtml", ".js", ".cs"):
                continue
            for m in pat.findall(f.read_text(encoding="utf-8", errors="ignore")):
                if m not in skip:
                    found.add(m)
    return found


def icons_from_db() -> set[str]:
    found: set[str] = set()
    if not DB.exists():
        return found
    con = sqlite3.connect(str(DB))
    try:
        tables = [r[0] for r in con.execute(
            "SELECT name FROM sqlite_master WHERE type='table'")]
        for t in tables:
            cols = [r[1] for r in con.execute(f'PRAGMA table_info("{t}")')]
            for c in cols:
                if "icon" not in c.lower():
                    continue
                for (v,) in con.execute(f'SELECT "{c}" FROM "{t}" WHERE "{c}" IS NOT NULL'):
                    for m in re.findall(r'fa-([a-z0-9-]+)', str(v)):
                        found.add(m)
    finally:
        con.close()
    return found


def main() -> int:
    if not CSS.exists():
        sys.exit(f"missing {CSS}")

    cp = icons_from_css()
    used = icons_from_views() | icons_from_db()
    keep_names = set(SAFELIST) | set(BRANDS_SAFELIST) | used

    # Always keep the private-use blanks FA uses internally.
    keep_cps = {cp[n] for n in keep_names if n in cp}
    unknown = sorted(n for n in used if n not in cp)

    print(f"icons referenced in views/db : {len(used)}")
    print(f"safelist                     : {len(SAFELIST) + len(BRANDS_SAFELIST)}")
    print(f"codepoints kept              : {len(keep_cps)}")
    if unknown:
        print(f"note: {len(unknown)} name(s) not in the FA css (aliases or typos): "
              f"{', '.join(unknown[:8])}{'…' if len(unknown) > 8 else ''}")

    total_before = total_after = 0
    for style, fname in FILES.items():
        path = FONTS / fname
        if not path.exists():
            print(f"skip {fname} (missing)")
            continue

        backup = path.with_suffix(".full.woff2")
        if not backup.exists():
            shutil.copy2(path, backup)

        # Always subset from the pristine copy so re-runs never compound losses.
        font = TTFont(str(backup))
        available = set(font.getBestCmap().keys())
        wanted = sorted(keep_cps & available)
        if not wanted:
            print(f"skip {fname}: no wanted glyph present")
            font.close()
            continue

        before = backup.stat().st_size
        opts = subset.Options()
        opts.flavor = "woff2"
        opts.desubroutinize = True
        opts.layout_features = ["*"]
        opts.notdef_outline = True
        opts.drop_tables += ["FFTM"]
        subsetter = subset.Subsetter(options=opts)
        subsetter.populate(unicodes=wanted)
        subsetter.subset(font)
        font.flavor = "woff2"
        font.save(str(path))
        font.close()

        after = path.stat().st_size
        total_before += before
        total_after += after
        print(f"  {fname:22} {before/1024:7.1f} KB -> {after/1024:6.1f} KB "
              f"({len(wanted)} glyphs, -{100*(before-after)/before:.0f}%)")

    if total_before:
        print(f"  {'TOTAL':22} {total_before/1024:7.1f} KB -> {total_after/1024:6.1f} KB "
              f"(-{100*(total_before-total_after)/total_before:.0f}%)")
    return 0


if __name__ == "__main__":
    sys.exit(main())
