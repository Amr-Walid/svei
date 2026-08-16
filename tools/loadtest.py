#!/usr/bin/env python3
"""
Load / stress test for the SVEI site.

Drives concurrent traffic against the real pages a visitor actually hits and
reports latency percentiles, throughput and error rate per URL, plus a combined
verdict. Designed to be re-run as a regression gate after performance work.

Usage:
    python3 tools/loadtest.py [base_url] [--users N] [--seconds S]

Notes
-----
The rate limiter is per client IP and this whole test comes from one address,
so it is expected to start returning 429 under a heavy run. A 429 is the server
working correctly, not a failure — it is counted separately from real errors
(5xx / timeouts / connection resets), which are the things that mean the site
actually fell over.
"""
from __future__ import annotations

import argparse
import statistics
import sys
import threading
import time
from collections import defaultdict

import requests

PATHS = [
    "/ar", "/en",
    "/ar/about", "/en/about",
    "/ar/products", "/en/products",
    "/ar/services", "/en/services",
    "/ar/production-lines", "/en/production-lines",
    "/ar/news", "/en/news",
    "/ar/careers", "/en/careers",
    "/ar/events", "/en/events",
    "/ar/gallery", "/en/gallery",
    "/ar/contact", "/en/contact",
    "/ar/quote", "/en/quote",
    "/ar/products?q=cable", "/en/products?q=cable",
]

BYPASS: dict[str, str] = {}

lock = threading.Lock()
samples: dict[str, list[float]] = defaultdict(list)
codes: dict[int, int] = defaultdict(int)
errors: list[str] = []
stop_at = 0.0


def worker(base: str) -> None:
    sess = requests.Session()
    sess.headers["User-Agent"] = "svei-loadtest/1.0"
    sess.headers.update(BYPASS)
    i = 0
    while time.time() < stop_at:
        path = PATHS[i % len(PATHS)]
        i += 1
        t0 = time.perf_counter()
        try:
            r = sess.get(base + path, timeout=30)
            dt = (time.perf_counter() - t0) * 1000
            with lock:
                samples[path].append(dt)
                codes[r.status_code] += 1
                if r.status_code >= 500:
                    errors.append(f"{path} -> {r.status_code}")
        except Exception as e:
            with lock:
                codes[0] += 1
                errors.append(f"{path} -> {type(e).__name__}")


def pct(xs: list[float], p: float) -> float:
    if not xs:
        return 0.0
    xs = sorted(xs)
    k = min(len(xs) - 1, int(round((p / 100) * (len(xs) - 1))))
    return xs[k]


def main() -> int:
    global stop_at

    ap = argparse.ArgumentParser()
    ap.add_argument("base", nargs="?", default="http://127.0.0.1:5020")
    ap.add_argument("--users", type=int, default=30)
    ap.add_argument("--seconds", type=int, default=25)
    ap.add_argument("--token", default=None,
                    help="Security:LoadTestToken value — bypasses the per-IP read "
                         "budget so the test measures server capacity instead of "
                         "measuring the rate limiter.")
    a = ap.parse_args()
    base = a.base.rstrip("/")
    if a.token:
        BYPASS["X-SVEI-LoadTest"] = a.token

    print(f"=== SVEI load test → {base} ===")
    print(f"    {a.users} concurrent users · {a.seconds}s · {len(PATHS)} distinct URLs\n")

    # Warm the app so JIT + EF model build + first-hit caching are not measured
    # as if they were steady-state latency.
    warm = requests.Session()
    warm.headers.update(BYPASS)
    for p in PATHS:
        try:
            warm.get(base + p, timeout=30)
        except Exception:
            pass
    print("warm-up complete, starting measured run…\n")

    stop_at = time.time() + a.seconds
    started = time.time()

    threads = [threading.Thread(target=worker, args=(base,), daemon=True)
               for _ in range(a.users)]
    for t in threads:
        t.start()
    for t in threads:
        t.join()

    elapsed = time.time() - started
    allx = [x for v in samples.values() for x in v]
    total = sum(codes.values())

    if not allx:
        print("no samples collected")
        return 1

    print(f"{'URL':<34}{'n':>6}{'p50':>9}{'p95':>9}{'p99':>9}{'max':>9}")
    print("-" * 76)
    for path in PATHS:
        v = samples.get(path)
        if not v:
            continue
        print(f"{path:<34}{len(v):>6}{pct(v,50):>8.0f}m{pct(v,95):>8.0f}m"
              f"{pct(v,99):>8.0f}m{max(v):>8.0f}m")

    ok = sum(c for s, c in codes.items() if 200 <= s < 400)
    limited = codes.get(429, 0)
    failed = sum(c for s, c in codes.items() if s == 0 or s >= 500)

    print("\n" + "=" * 76)
    print(f"requests      : {total} in {elapsed:.1f}s")
    print(f"throughput    : {total/elapsed:.1f} req/s")
    print(f"success (2-3xx): {ok}  ({100*ok/total:.1f}%)")
    print(f"rate-limited   : {limited}  ({100*limited/total:.1f}%)  [expected: single test IP]")
    print(f"hard failures  : {failed}  ({100*failed/total:.1f}%)  [5xx / timeouts / resets]")
    print(f"latency p50/p95/p99: {pct(allx,50):.0f}ms / {pct(allx,95):.0f}ms / {pct(allx,99):.0f}ms")
    print("status codes  : " + ", ".join(f"{k}={v}" for k, v in sorted(codes.items())))

    if errors:
        uniq = sorted(set(errors))
        print(f"\nfailure detail ({len(errors)} total, {len(uniq)} distinct):")
        for e in uniq[:10]:
            print("  " + e)

    verdict = failed == 0 and pct(allx, 95) < 2000
    print("\nVERDICT: " + ("PASS — no hard failures, p95 within budget"
                           if verdict else "REVIEW — see failures above"))
    return 0 if verdict else 1


if __name__ == "__main__":
    sys.exit(main())
