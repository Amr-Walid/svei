#!/usr/bin/env python3
"""
Generate the Arabic wordmark variants of the SVEI logo.

The English logo (wwwroot/img/brand/logo-*.svg) hard-codes the Latin wordmark
"Silicon Valley / Electronic Industries" as <text>, so Arabic pages were showing
an English logo. Arabic <text> in an SVG cannot be relied upon (it needs a font
with Arabic shaping installed on the viewer's machine), so this script shapes the
Arabic strings with HarfBuzz using the site's own self-hosted IBM Plex Sans
Arabic subset and bakes the result into <path> outlines. The result renders
identically everywhere with zero font dependency.

Outputs:
    wwwroot/img/brand/logo-dark-ar.svg   (for light backgrounds)
    wwwroot/img/brand/logo-light-ar.svg  (for dark backgrounds)

Run:  python3 tools/make-ar-logo.py
"""
from __future__ import annotations

import pathlib
import tempfile

import uharfbuzz as hb
from fontTools.pens.svgPathPen import SVGPathPen
from fontTools.ttLib import TTFont

ROOT = pathlib.Path(__file__).resolve().parent.parent
FONTS = ROOT / "wwwroot" / "fonts" / "files"
OUT = ROOT / "wwwroot" / "img" / "brand"

# Arabic subset of IBM Plex Sans Arabic, per weight (see wwwroot/fonts/plexar.local.css)
FACES = {
    400: FONTS / "Qw3CZRtWPQCuHme67tEYUIx3Kh0PHR9N6Ys43PWrfQ.woff2",
    700: FONTS / "Qw3NZRtWPQCuHme67tEYUIx3Kh0PHR9N6YOG-eCRXMR5Kw.woff2",
}

LINE_1 = "سيليكون فالي"          # bold
LINE_2 = "للصناعات الإلكترونية"  # regular


_TTF_CACHE: dict[int, pathlib.Path] = {}


def as_ttf(weight: int) -> pathlib.Path:
    """HarfBuzz cannot read WOFF2 — silently shapes everything to .notdef.
    Decompress the subset to a plain TTF in a temp dir once and reuse it."""
    if weight not in _TTF_CACHE:
        tmp = pathlib.Path(tempfile.gettempdir()) / f"svei-plexar-{weight}.ttf"
        f = TTFont(str(FACES[weight]))
        f.flavor = None  # drop woff2 wrapper
        f.save(str(tmp))
        f.close()
        _TTF_CACHE[weight] = tmp
    return _TTF_CACHE[weight]


def shape_to_path(text: str, weight: int, font_size: float, x: float, y: float) -> tuple[str, float]:
    """Shape `text` RTL and return (svg path data, advance width in user units)."""
    path = as_ttf(weight)
    tt = TTFont(str(path))
    glyphset = tt.getGlyphSet()
    upem = tt["head"].unitsPerEm
    scale = font_size / upem

    blob = hb.Blob.from_file_path(str(path))
    face = hb.Face(blob)
    hbfont = hb.Font(face)

    buf = hb.Buffer()
    buf.add_str(text)
    buf.direction = "rtl"
    buf.script = "Arab"
    buf.language = "ar"
    hb.shape(hbfont, buf, {"kern": True, "liga": True})

    order = tt.getGlyphOrder()
    pieces: list[str] = []
    pen_x = 0.0

    for info, pos in zip(buf.glyph_infos, buf.glyph_positions):
        name = order[info.codepoint]
        pen = SVGPathPen(glyphset, ntos=lambda v: f"{v:.2f}")
        glyphset[name].draw(pen)
        d = pen.getCommands()
        if d:
            # Glyph space -> user space: scale, flip Y, translate to the pen position.
            gx = x + (pen_x + pos.x_offset) * scale
            gy = y + (-pos.y_offset) * scale
            pieces.append(
                f'<g transform="translate({gx:.2f} {gy:.2f}) scale({scale:.6f} {-scale:.6f})">'
                f'<path d="{d}"/></g>'
            )
        pen_x += pos.x_advance

    tt.close()
    return "\n    ".join(pieces), pen_x * scale


MARK_W = 100.0   # the red monogram square
GUTTER = 20.0    # optical gap between mark and wordmark


def build(word_fill: str, sub_fill: str) -> str:
    # RTL reading order: the mark LEADS, so it sits on the right and the Arabic
    # wordmark runs leftwards from it — the mirror image of the Latin lockup.
    mark_x = 372.0 - MARK_W          # 272
    right_edge = mark_x - GUTTER     # wordmark's right edge

    l1, w1 = shape_to_path(LINE_1, 700, 34.0, 0.0, 46.0)
    l2, w2 = shape_to_path(LINE_2, 400, 19.5, 0.0, 82.0)

    # HarfBuzz RTL output already has the glyphs in visual order with positive
    # advances, so each run occupies [0, w]. Shift it so its right edge lands on
    # `right_edge`, which right-aligns both lines under each other.
    g1 = f'<g transform="translate({right_edge - w1:.2f} 0)" fill="{word_fill}">\n    {l1}\n  </g>'
    g2 = f'<g transform="translate({right_edge - w2:.2f} 0)" fill="{sub_fill}">\n    {l2}\n  </g>'

    return f'''<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 372 100" width="372" height="100" role="img" aria-label="سيليكون فالي للصناعات الإلكترونية">
  <title>سيليكون فالي للصناعات الإلكترونية</title>

  <!-- Monogram mark (leads on the right for RTL) -->
  <g transform="translate({mark_x:.0f} 0)">
    <rect width="100" height="100" fill="#E31B23"/>
    <g fill="#FFFFFF" font-family="Inter, 'Helvetica Neue', Arial, sans-serif" font-weight="800">
      <text x="27" y="46" font-size="42" text-anchor="middle" letter-spacing="-1">S</text>
      <text x="72" y="46" font-size="42" text-anchor="middle" letter-spacing="-1">V</text>
      <text x="27" y="88" font-size="42" text-anchor="middle" letter-spacing="-1">E</text>
      <text x="72" y="88" font-size="42" text-anchor="middle" letter-spacing="-1">i</text>
    </g>
  </g>

  <!-- Arabic wordmark (outlined from IBM Plex Sans Arabic — no font dependency) -->
  {g1}
  {g2}
</svg>
'''


def main() -> None:
    (OUT / "logo-dark-ar.svg").write_text(build("#26292E", "#5A5C5E"), encoding="utf-8")
    (OUT / "logo-light-ar.svg").write_text(build("#FFFFFF", "rgba(255,255,255,0.72)"), encoding="utf-8")
    for f in ("logo-dark-ar.svg", "logo-light-ar.svg"):
        print(f"wrote {OUT / f}  ({(OUT / f).stat().st_size} bytes)")


if __name__ == "__main__":
    main()
