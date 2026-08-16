#!/usr/bin/env python3
"""
Safety net for the subsetted Font Awesome webfonts.

Why this exists
---------------
tools/subset-icons.py strips Font Awesome from 2,575 glyphs (~295 KB) down to
the ones this site can actually show. That is only safe because icon names are
free text in the admin (FieldKind.Icon): the day an editor picks an icon that
was subsetted away, it would render as a blank box.

This script closes that loop. It reads the glyphs that are really present in the
shipped .woff2 files (not what the subsetter *intended* to keep) and compares
them against every icon referenced by a view or stored in the database. A
missing glyph is reported here, in the test suite, instead of by a visitor.

Exit codes
----------
  0  every referenced icon is present (or the fonts are not subsetted yet)
  1  at least one referenced icon is missing from the subset

Fixing a failure is one command:  python3 tools/subset-icons.py
(add the icon to SAFELIST first if it should be permanently available).
"""
from __future__ import annotations

import importlib.util
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent

try:
    from fontTools.ttLib import TTFont
except ImportError:
    sys.exit("fonttools is required:  pip install 'fonttools[woff]'")


def _load_subsetter():
    """Import subset-icons.py despite the hyphen in its filename.

    Reusing the subsetter's own extraction logic guarantees the verifier and
    the subsetter can never disagree about which icons are referenced.
    """
    path = Path(__file__).resolve().parent / "subset-icons.py"
    spec = importlib.util.spec_from_file_location("subset_icons", path)
    if spec is None or spec.loader is None:
        sys.exit(f"cannot load {path}")
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


_sub = _load_subsetter()
FONTS = _sub.FONTS
FILES = _sub.FILES
icons_from_css = _sub.icons_from_css
icons_from_db = _sub.icons_from_db
icons_from_views = _sub.icons_from_views


def glyph_codepoints(path: Path) -> set[int]:
    """Codepoints actually encoded in a font file's cmap."""
    with TTFont(str(path), lazy=True) as font:
        return set(font.getBestCmap().keys())


def main() -> int:
    subsetted = [
        name for name in FILES.values()
        if (FONTS / name.replace(".woff2", ".full.woff2")).exists()
    ]
    if not subsetted:
        print("fonts are not subsetted (no *.full.woff2 backups) - nothing to verify")
        return 0

    cp_map = icons_from_css()
    if not cp_map:
        print("FAIL: could not parse any icon codepoints out of the Font Awesome CSS")
        return 1

    # Every codepoint present across all three shipped files.
    available: set[int] = set()
    for name in FILES.values():
        path = FONTS / name
        if path.exists():
            available |= glyph_codepoints(path)

    referenced = icons_from_views() | icons_from_db()

    missing: list[str] = []
    unknown: list[str] = []
    for icon in sorted(referenced):
        cp = cp_map.get(icon)
        if cp is None:
            # Not a real Font Awesome name (a css helper class, a partial match
            # from a regex, or a typo). Not our problem: it never rendered.
            unknown.append(icon)
            continue
        if cp not in available:
            missing.append(f"{icon} (U+{cp:04X})")

    checked = len(referenced) - len(unknown)
    print(f"icons referenced (views + db) : {len(referenced)}")
    print(f"resolvable to a real glyph    : {checked}")
    print(f"glyphs available in subset    : {len(available)}")

    if unknown:
        preview = ", ".join(unknown[:12])
        more = f" (+{len(unknown) - 12} more)" if len(unknown) > 12 else ""
        print(f"ignored, not real FA names    : {len(unknown)}  [{preview}{more}]")

    if missing:
        print()
        print(f"FAIL: {len(missing)} referenced icon(s) missing from the subset:")
        for m in missing:
            print(f"  - {m}")
        print()
        print("Fix: add the name to SAFELIST in tools/subset-icons.py if it should")
        print("     stay available, then re-run  python3 tools/subset-icons.py")
        return 1

    print()
    print(f"PASS: all {checked} referenced icons are present in the subsetted fonts")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
