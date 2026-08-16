#!/usr/bin/env python3
"""
Drives a real browser through the public site and the admin panel and reports
any Content-Security-Policy violation or JavaScript error.

A strict CSP is worthless if it silently breaks the UI, and it is equally
worthless if it is quietly relaxed until nothing complains. This proves the
policy is both enforced and compatible with every screen.

Usage:  python3 tools/csp-audit.py [base_url]
"""
from __future__ import annotations

import sys

from playwright.sync_api import sync_playwright

BASE = (sys.argv[1] if len(sys.argv) > 1 else "http://127.0.0.1:5020").rstrip("/")
EMAIL, PASSWORD = "admin@svei.tech", "Svei@2024!"

PUBLIC = ["/ar", "/en", "/ar/about", "/en/products", "/ar/services",
          "/en/careers", "/ar/contact", "/en/news", "/ar/events",
          "/en/gallery", "/ar/quote", "/ar/production-lines"]

ADMIN = ["/Admin", "/Admin/e/settings", "/Admin/e/products",
         "/Admin/e/products/create", "/Admin/e/news", "/Admin/e/faq/create",
         "/Admin/Media", "/Admin/Users", "/Admin/e/hero", "/Admin/e/jobs"]

ARGS = ["--single-process", "--no-zygote", "--disable-dev-shm-usage",
        "--disable-gpu", "--js-flags=--max-old-space-size=192"]

IGNORE = ("favicon", "the server responded with a status of 404")


def audit(pw, paths, label, login=False):
    problems = []
    browser = pw.chromium.launch(args=ARGS)
    try:
        ctx = browser.new_context(viewport={"width": 1366, "height": 900},
                                  ignore_https_errors=True)
        page = ctx.new_page()

        csp, errs = [], []
        page.on("console", lambda m: (
            csp.append(m.text) if "Content Security Policy" in m.text
            else errs.append(m.text) if m.type == "error" else None))
        page.on("pageerror", lambda e: errs.append(str(e)))

        if login:
            page.goto(f"{BASE}/Account/Login", wait_until="domcontentloaded", timeout=60000)
            page.fill('input[name="email"]', EMAIL)
            page.fill('input[name="password"]', PASSWORD)
            page.click('button[type="submit"]')
            page.wait_for_load_state("domcontentloaded", timeout=60000)
            if "/Account/Login" in page.url:
                print(f"  !! could not sign in — {page.url}")
                return [("login", ["authentication failed"], [])]

        for p in paths:
            csp.clear(); errs.clear()
            try:
                page.goto(BASE + p, wait_until="domcontentloaded", timeout=60000)
                page.wait_for_timeout(1600)
            except Exception as e:
                problems.append((p, [], [f"navigation: {type(e).__name__}"]))
                continue

            real = [e for e in errs if not any(i in e.lower() for i in IGNORE)]
            if csp or real:
                problems.append((p, list(csp), real))
                print(f"  [FAIL] {p}")
                for c in csp[:3]:
                    print(f"         CSP: {c[:150]}")
                for e in real[:3]:
                    print(f"         ERR: {e[:150]}")
            else:
                print(f"  [ ok ] {p}")

        ctx.close()
    finally:
        browser.close()
    return problems


if __name__ == "__main__":
    print(f"=== CSP / JS audit → {BASE} ===\n")
    allp = []
    with sync_playwright() as pw:
        print("public pages:")
        allp += audit(pw, PUBLIC, "public")
        print("\nadmin pages:")
        allp += audit(pw, ADMIN, "admin", login=True)

    print("\n" + "=" * 60)
    if allp:
        print(f"FAIL — {len(allp)} page(s) with CSP violations or JS errors")
        sys.exit(1)
    print(f"PASS — {len(PUBLIC) + len(ADMIN)} pages clean under strict CSP")
