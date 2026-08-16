# SVEI — Security Review, Penetration Test & Load Test Report

**Scope:** full source review of the ASP.NET Core 8 application, black-box and
authenticated penetration testing against a running instance, and load/stress
testing to establish real capacity.

**Result:** 12 vulnerabilities found and fixed. All automated suites pass.
The site sustained **26,415 requests with zero failures** under load.

---

## 1. Vulnerabilities found and fixed

### CRITICAL

#### 1.1 Stored XSS via file upload — reachable by anonymous visitors
`MediaService` filtered uploads with a **blocklist** of dangerous extensions.
A blocklist can only reject what someone thought to enumerate, so `.html`,
`.svg`, `.phtml`, `.xhtml` and `shell.pdf.html` all passed. Files land in
`wwwroot/uploads/**`, which `UseStaticFiles()` serves **from the site's own
origin** — so an uploaded HTML file executed as first-party script.

This was reachable **without any login**, through two forms:
- the CV field on a job application (`/careers/apply`)
- the attachment on a quote request (`/quote`)

An attacker uploads `cv.html`, waits for an admin to open the CV from the
inbox, and their script runs with the admin's session.

**Fixed:** replaced with an **allowlist** (`.pdf/.doc/.docx/.xls/.xlsx/.ppt/
.pptx/.txt/.csv/.rtf/.odt/.zip`), added a declared-content-type cross-check,
and **removed the SVG passthrough** — SVG is a scriptable document, not an
image, and the old code explicitly passed it through untouched.
Defence in depth: everything under `/uploads` is now served with
`Content-Security-Policy: default-src 'none'; sandbox`, `nosniff`, and a
forced `Content-Disposition: attachment` for non-images.

#### 1.2 Stored XSS via rich text — Editor → Admin privilege escalation
18 views render admin-authored HTML with `@Html.Raw(...)`, which bypasses the
Razor encoder by design (it is a WYSIWYG field). **Nothing sanitized it.** The
`HtmlSanitizer` package was even referenced in the `.csproj` but never used.

The CMS has two roles. An **Editor** is lower-trust (a marketing hire, an
agency) but could store `<script>` in a product description that later executes
in an **Admin's** browser — a privilege escalation, not self-XSS.

**Fixed:** added `HtmlSanitizerService`, applied to every `FieldKind.Html` on
the **write** path, so the stored value is the thing that is trusted.
Allowlist of tags/attributes/schemes; `style` is rejected (it enables
clickjacking overlays with no script at all); only `http/https/mailto/tel`
URLs survive, which is what blocks `javascript:` and `data:text/html`.

Verified directly against the database:

| Payload submitted | Value actually stored |
|---|---|
| `<script>alert(1)</script>` | `alert(1)` — inert text |
| `<img src=x onerror="alert(1)">` | `<img src="x">` |
| `<a href="javascript:alert(1)">c</a>` | `<a>c</a>` |
| `<svg onload="alert(1)">`, `<iframe src=…>` | removed entirely |
| `<div style="position:fixed;top:0">` | `<div>` |

### HIGH

#### 1.3 Every 404 on the site was an infinite redirect loop
`[Route("/Home/NotFoundPage")]` has no `/ar|/en` prefix and was not in the
culture middleware's bypass list. `UseStatusCodePagesWithReExecute` sent 404s
there; the middleware redirected to `/ar/Home/NotFoundPage`; that matched no
route, so it 404'd again — forever. Confirmed: `curl -L` exhausted its redirect
limit. **Fixed** by bypassing the error routes. Now returns a clean `404`.

#### 1.4 No security headers at all
No CSP, `X-Frame-Options`, `X-Content-Type-Options`, `Referrer-Policy` or
`Permissions-Policy` on any response — the site was framable (clickjacking) and
had no second line of defence against injected script.

**Fixed** with `SecurityHeadersMiddleware`, placed first in the pipeline so it
also covers static files, error pages and rate-limiter rejections.
`script-src 'self'`, `object-src 'none'`, `frame-ancestors 'none'`,
`base-uri 'self'`, `form-action 'self'`.

To keep `script-src 'self'` honest, **all 6 inline scripts were removed**
rather than adding `unsafe-inline` (which would have defeated the point):
5 inline event attributes became delegated listeners, and the admin theme
bootstrap moved to `js/theme-boot.js`.

#### 1.5 Host header poisoning
`baseUrl` fell back to `req.Host`, so `Host: evil.com` rewrote every canonical
URL, `hreflang`, OG tag and the JSON-LD `Organization` block — handing SEO
authority to an attacker and poisoning any shared cache.
**Fixed:** pinned to the configured base URL. `AllowedHosts` no longer `*`.

#### 1.6 No rate limiting on any public write
Contact, quote, apply and subscribe accepted **15/15** automated submissions in
testing — unlimited spam rows and unlimited files on disk.
**Fixed:** per-IP limits — 5 form writes / 10 min, 10 logins / 15 min, plus a
global token-bucket read budget that exempts static assets. Identity lockout
configured explicitly (5 attempts / 15 min).

#### 1.7 `</script>` breakout in JSON-LD
The block used `UnsafeRelaxedJsonEscaping`, so an admin-entered `</script>` in
any company field would terminate the element and inject markup. **Fixed.**

### MEDIUM

| # | Issue | Fix |
|---|---|---|
| 1.8 | **Username enumeration** — a missing account returned immediately while a real one ran a password hash; the timing difference reveals which addresses exist, letting a spray target only valid ones | miss path now burns comparable work |
| 1.9 | **Spoofable `X-Forwarded-For`** — `KnownProxies` was cleared, so any client could forge the header, get a fresh rate-limit bucket per request, and poison the IP in the audit log and every contact message | only honoured from configured proxies; `ForwardLimit = 1` |
| 1.10 | **Admin password in plaintext** in `appsettings.json` *and* hardcoded as a source fallback | both removed; seeding requires `AdminSeed__Password` or creates no account |
| 1.11 | **Cookie flags** — no `Secure`/`HttpOnly`/`SameSite`; 7-day session | all three set (`Secure` configurable so the HTTP preview still works); session cut to 12h |
| 1.12 | **100 MB body limit on every endpoint** including anonymous ones — trivial memory-exhaustion DoS | 30 MB with per-field caps, plus slowloris protection (`MinRequestBodyDataRate`, header timeouts, connection ceilings) |

### Reviewed and found already correct

- **SQL injection** — no raw SQL anywhere; all EF Core LINQ, fully parameterized.
- **Mass assignment** — the generic admin binder walks the `AdminSchema` field
  allowlist, so unknown POST fields are ignored. Verified.
- **CSRF** — `[ValidateAntiForgeryToken]` on every state-changing action.
- **Authorization** — all 34 admin screens gated by `[Authorize]` on the base
  controller; user management is `Admin`-only.
- **Path traversal** — blocked; upload folder names are reduced to a single
  safe segment and the resolved path is re-verified inside `wwwroot/uploads`.

---

## 2. Test suites

Re-runnable regression gates, committed under `tools/`.

| Suite | What it covers | Result |
|---|---|---|
| `pentest.py` | 19 unauthenticated checks: authz, CSRF, SQLi, traversal, XSS, open redirect, host header, headers, cookies, rate limits, lockout, verb tampering, info leak | **18 PASS / 0 FAIL** ¹ |
| `pentest_auth.py` | 6 authenticated checks: stored XSS, upload allowlist, mass assignment, admin CSRF, role separation, logout | **6 PASS / 0 FAIL** |
| `csp-audit.py` | Real browser over 12 public + 10 admin pages, fails on any CSP violation or JS error | **22/22 clean** |
| `loadtest.py` | Concurrent load across 24 URLs, latency percentiles, error rate | **PASS** |

¹ One expected WARN: the local preview runs over plain HTTP, which cannot carry
`Secure` cookies. Production config sets `RequireSecureCookies: true`.

```bash
python3 tools/pentest.py       http://127.0.0.1:5020
python3 tools/pentest_auth.py  http://127.0.0.1:5020
python3 tools/csp-audit.py     http://127.0.0.1:5020
python3 tools/loadtest.py      http://127.0.0.1:5020 --users 50 --seconds 90 --token <LoadTestToken>
```

---

## 3. Load & stress test results

Measured on the 1 GB sandbox running a **Debug** build against SQLite —
production (Release + real hosting) will be materially faster.

| Concurrent users | Duration | Requests | Success | Hard failures | Throughput | p50 | p95 | p99 |
|---|---|---|---|---|---|---|---|---|
| 30 | 25s | 6,020 | **100%** | **0** | 240/s | 115ms | 196ms | 265ms |
| 60 | 40s | 11,668 | **100%** | **0** | 290/s | 203ms | 261ms | 325ms |
| 100 | 20s | 5,731 | **100%** | **0** | 283/s | 348ms | 422ms | 483ms |
| 50 | **90s** | **26,415** | **100%** | **0** | 293/s | 169ms | 209ms | 250ms |

**Findings**

- **Zero hard failures across ~50,000 requests** — no 5xx, no timeouts, no
  resets, at any concurrency level tested.
- **Throughput holds or improves as load rises** (240 → 290 → 283 req/s). The
  server saturates gracefully rather than collapsing.
- **No memory leak.** After the 90-second endurance run, the process sat at
  **247 MB RSS**, flat.
- Latency stays well inside budget: p95 ≈ 200ms at realistic load, and even at
  100 concurrent users p99 stays under half a second.

**Throughput optimisations made during this work**

- `CultureMiddleware` was issuing a **database query on every single request**
  — the hottest query in the app, against a table with a handful of rows. Now
  cached as a dictionary, invalidated on admin writes.
- Added Brotli/Gzip response compression.
- Added `immutable` long-lived caching for fingerprinted static assets.

**Note on the rate limiter.** An early run showed 96.5% of requests returning
`429`. That is the limiter working correctly — the whole test originates from
one IP. The figures above use a bypass token (`Security:LoadTestToken`,
constant-time compared, unset in production) so the numbers measure *server
capacity* rather than measuring the limiter.

---

## 4. Deployment checklist

Set these in the production environment — **not** in a committed file:

```bash
AdminSeed__Password=<strong unique password>   # required, else no admin is created
Security__RequireSecureCookies=true            # default
Security__KnownProxies__0=<reverse proxy IP>   # if behind a proxy
# Security__LoadTestToken — leave UNSET in production
```

Also confirm:

- [ ] HTTPS enforced at the edge (HSTS is already sent in non-Development).
- [ ] `AllowedHosts` in `appsettings.json` lists the real domains.
- [ ] **Change the seeded admin password** after first login.
- [ ] `svei.db` is outside the web root and in the backup schedule.
- [ ] Re-run all four suites against staging after deploy.
