#!/usr/bin/env python3
"""
Generate every SVEI logo variant with all text baked into <path> outlines.

Why outlines and not <text>?
    An SVG referenced through <img src="..."> is an isolated document: it cannot
    see the host page's @font-face rules, and it has no network access of its
    own. So `font-family="Inter"` inside the logo silently fell back to whatever
    the OS happened to have. That fallback is wider than Inter, so the Latin
    wordmark "Silicon Valley" overflowed the 372-wide viewBox and rendered
    visibly clipped in the header. The Arabic wordmark had the same class of
    problem, plus it needs real shaping.

    Baking the glyphs to outlines removes the font dependency entirely: the
    artwork measures the same everywhere, and the viewBox is computed from the
    actual shaped advance so nothing can ever be cut off again.

Outputs (all viewBox="0 0 <fitted> 100"):
    wwwroot/img/brand/logo-dark.svg      Latin, for light backgrounds
    wwwroot/img/brand/logo-light.svg     Latin, for dark backgrounds
    wwwroot/img/brand/logo-dark-ar.svg   Arabic, for light backgrounds
    wwwroot/img/brand/logo-light-ar.svg  Arabic, for dark backgrounds

Run:  python3 tools/make-logo.py
"""
from __future__ import annotations

import math
import pathlib
import tempfile

import uharfbuzz as hb
from fontTools.pens.svgPathPen import SVGPathPen
from fontTools.ttLib import TTFont
from fontTools.varLib.instancer import instantiateVariableFont

ROOT = pathlib.Path(__file__).resolve().parent.parent
FONTS = ROOT / "wwwroot" / "fonts" / "files"
OUT = ROOT / "wwwroot" / "img" / "brand"

# Inter is shipped as a single variable font (wght 100-900) — the same file backs
# every weight in wwwroot/fonts/inter.local.css.
INTER_VAR = FONTS / "UcC73FwrK3iLTeHuS_nVMrMxCp50SjIa1ZL7.woff2"

# IBM Plex Sans Arabic ships as static per-weight subsets.
PLEX_AR = {
    400: FONTS / "Qw3CZRtWPQCuHme67tEYUIx3Kh0PHR9N6Ys43PWrfQ.woff2",
    700: FONTS / "Qw3NZRtWPQCuHme67tEYUIx3Kh0PHR9N6YOG-eCRXMR5Kw.woff2",
}

LATIN_1, LATIN_2 = "Silicon Valley", "Electronic Industries"
ARABIC_1, ARABIC_2 = "سيليكون فالي", "للصناعات الإلكترونية"

MARK_W = 100.0   # the red monogram square is 100x100
GUTTER = 20.0    # optical gap between the mark and the wordmark
TAIL = 6.0       # trailing breathing room so ink never touches the canvas edge
HEIGHT = 100.0

_CACHE: dict[tuple[str, int], pathlib.Path] = {}


def face_path(family: str, weight: int) -> pathlib.Path:
    """Return a plain TTF for (family, weight), building it once.

    HarfBuzz cannot read WOFF2 at all — it silently shapes every glyph to
    .notdef, which renders as tofu boxes with no error whatsoever. So the
    subset always has to be decompressed to a bare TTF first. For Inter we
    additionally pin the variable `wght` axis, otherwise every weight would
    come out at the 400 default.
    """
    key = (family, weight)
    if key not in _CACHE:
        tmp = pathlib.Path(tempfile.gettempdir()) / f"svei-{family}-{weight}.ttf"
        if family == "inter":
            f = TTFont(str(INTER_VAR))
            f = instantiateVariableFont(f, {"wght": weight}, updateFontNames=False)
        else:
            f = TTFont(str(PLEX_AR[weight]))
        f.flavor = None  # drop the woff2 wrapper
        f.save(str(tmp))
        f.close()
        _CACHE[key] = tmp
    return _CACHE[key]


def shape(text: str, family: str, weight: int, size: float, rtl: bool,
          tracking: float = 0.0) -> tuple[str, float]:
    """Shape `text` and return (svg fragment drawn from x=0, advance width).

    `tracking` is letter-spacing in em, matching the CSS/SVG convention.
    """
    path = face_path(family, weight)
    tt = TTFont(str(path))
    glyphset = tt.getGlyphSet()
    scale = size / tt["head"].unitsPerEm
    track = tracking * size

    hbfont = hb.Font(hb.Face(hb.Blob.from_file_path(str(path))))
    buf = hb.Buffer()
    buf.add_str(text)
    buf.direction = "rtl" if rtl else "ltr"
    buf.script = "Arab" if rtl else "Latn"
    buf.language = "ar" if rtl else "en"
    hb.shape(hbfont, buf, {"kern": True, "liga": True})

    order = tt.getGlyphOrder()
    pieces: list[str] = []
    pen_x = 0.0  # in user units, so tracking can be mixed in directly

    for info, pos in zip(buf.glyph_infos, buf.glyph_positions):
        pen = SVGPathPen(glyphset, ntos=lambda v: f"{v:.2f}")
        glyphset[order[info.codepoint]].draw(pen)
        d = pen.getCommands()
        if d:
            gx = pen_x + pos.x_offset * scale
            gy = -pos.y_offset * scale
            pieces.append(
                f'<g transform="translate({gx:.2f} {gy:.2f}) '
                f'scale({scale:.6f} {-scale:.6f})"><path d="{d}"/></g>'
            )
        pen_x += pos.x_advance * scale + track

    tt.close()
    # The trailing tracking unit is spacing after the last glyph, not ink.
    return "\n    ".join(pieces), max(0.0, pen_x - track)


def monogram(x: float) -> str:
    """The red square with the outlined S V E i letters."""
    cells = [("S", 27, 46), ("V", 72, 46), ("E", 27, 88), ("i", 72, 88)]
    glyphs = []
    for ch, cx, cy in cells:
        frag, w = shape(ch, "inter", 800, 42.0, rtl=False, tracking=-1 / 42)
        # text-anchor="middle" has no meaning for outlines — centre it manually.
        glyphs.append(
            f'<g transform="translate({cx - w / 2:.2f} {cy:.2f})">{frag}</g>'
        )
    body = "\n      ".join(glyphs)
    return (
        f'<g transform="translate({x:.0f} 0)">\n'
        f'    <rect width="100" height="100" fill="#E31B23"/>\n'
        f'    <g fill="#FFFFFF">\n      {body}\n    </g>\n'
        f'  </g>'
    )


def build(lang: str, word_fill: str, sub_fill: str) -> str:
    rtl = lang == "ar"
    line1, line2 = (ARABIC_1, ARABIC_2) if rtl else (LATIN_1, LATIN_2)
    family = "plexar" if rtl else "inter"
    # Arabic glyphs have a larger optical size at the same point size, so the
    # Arabic wordmark is set slightly smaller to match the Latin lockup's weight.
    size1, size2 = (34.0, 19.5) if rtl else (38.0, 22.0)
    track1, track2 = (0.0, 0.0) if rtl else (-1.2 / 38, 0.2 / 22)
    label = f"{line1} {line2}" if rtl else f"SVEI — {line1} {line2}"

    f1, w1 = shape(line1, family, 700 if rtl else 800, size1, rtl, track1)
    f2, w2 = shape(line2, family, 400, size2, rtl, track2)

    # Fit the canvas to the artwork instead of assuming 372. This is the actual
    # fix for the clipped English wordmark: the width is now derived from the
    # shaped advance, so it cannot be too narrow.
    word_w = max(w1, w2)
    # Integer canvas so the width/height attributes in the markup can state the
    # intrinsic ratio exactly (a mismatch causes a pre-load layout shift).
    total_w = float(math.ceil(MARK_W + GUTTER + word_w + TAIL))

    if rtl:
        # RTL reading order: the mark leads, so it sits on the right and the
        # wordmark runs leftwards from it — the mirror of the Latin lockup.
        mark_x = total_w - MARK_W - TAIL
        right = mark_x - GUTTER
        # HarfBuzz RTL output is already in visual order occupying [0, w], so
        # shifting by (right - w) right-aligns both lines under each other.
        x1, x2 = right - w1, right - w2
    else:
        mark_x = 0.0
        x1 = x2 = MARK_W + GUTTER

    # Baselines: 46 for the wordmark, 82 for the descriptor line.
    g1 = f'<g transform="translate({x1:.2f} 46)" fill="{word_fill}">\n    {f1}\n  </g>'
    g2 = f'<g transform="translate({x2:.2f} 82)" fill="{sub_fill}">\n    {f2}\n  </g>'

    return f'''<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {total_w:g} {HEIGHT:g}" width="{total_w:g}" height="{HEIGHT:g}" role="img" aria-label="{label}">
  <title>{label}</title>

  <!-- Monogram mark -->
  {monogram(mark_x)}

  <!-- Wordmark, outlined so it needs no font and can never be clipped -->
  {g1}
  {g2}
</svg>
'''


def main() -> None:
    variants = [
        ("logo-dark.svg", "en", "#26292E", "#5A5C5E"),
        ("logo-light.svg", "en", "#FFFFFF", "rgba(255,255,255,0.72)"),
        ("logo-dark-ar.svg", "ar", "#26292E", "#5A5C5E"),
        ("logo-light-ar.svg", "ar", "#FFFFFF", "rgba(255,255,255,0.72)"),
    ]
    for name, lang, word, sub in variants:
        svg = build(lang, word, sub)
        (OUT / name).write_text(svg, encoding="utf-8")
        vb = svg.split('viewBox="', 1)[1].split('"', 1)[0]
        print(f"wrote {name:22s} viewBox={vb:16s} {(OUT / name).stat().st_size:>7,} bytes")


if __name__ == "__main__":
    main()
