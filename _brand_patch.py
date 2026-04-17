#!/usr/bin/env python3
"""
Apply Fisher Sovereign Systems brand block to portfolio showcase pages.

Patches:
  1. Inserts a sticky FSS wordmark strip right after <body>.
  2. Replaces the page's <footer>...</footer> (or appends one before </body>)
     with the branded FSS footer.
  3. Removes em-dashes (replaces with comma + space) per project rules.

Idempotent: skips pages that already contain the FSS_BRAND_MARKER.
"""

import re
import sys
from pathlib import Path

DEPLOY = Path(__file__).parent / "deploy"

PAGES = [
    "pip",
    "sovereign-trade-engine",
    "pm-executor",
    "prediction-market",
    "sovereign-hub",
    "trading-desk",
    "home-hub",
    "operator-console",
    "tax-platform",
    "auton",
]

FSS_BRAND_MARKER = "<!-- FSS-BRAND-BLOCK-V1 -->"

WORDMARK_SVG = (
    '<svg width="13" height="18" viewBox="0 0 20 28" fill="none" aria-hidden="true" '
    'style="color:rgba(201,168,76,0.92);flex-shrink:0">'
    '<line x1="4" y1="1" x2="4" y2="27" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>'
    '<line x1="16" y1="1" x2="16" y2="27" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>'
    '<line x1="4" y1="14" x2="16" y2="14" stroke="currentColor" stroke-width="1.5"/>'
    '<line x1="1" y1="1" x2="7" y2="1" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>'
    '<line x1="13" y1="1" x2="19" y2="1" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>'
    '<line x1="1" y1="27" x2="7" y2="27" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>'
    '<line x1="13" y1="27" x2="19" y2="27" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/>'
    '</svg>'
)

WORDMARK_SVG_LARGE = WORDMARK_SVG.replace('width="13" height="18"', 'width="16" height="22"')

# Sticky brand strip injected directly after <body>.
# Uses inline styles only so it cannot collide with existing CSS.
BRAND_STRIP = f"""{FSS_BRAND_MARKER}
<div style="position:sticky;top:0;z-index:99999;display:flex;align-items:center;justify-content:space-between;gap:14px;padding:9px 22px;background:rgba(8,8,10,0.96);backdrop-filter:blur(14px);-webkit-backdrop-filter:blur(14px);border-bottom:1px solid rgba(201,168,76,0.22);font-family:'Inter','Helvetica Neue',Arial,sans-serif;">
  <a href="../home.html" style="display:inline-flex;align-items:center;gap:10px;text-decoration:none;color:rgba(201,168,76,0.92);">
    {WORDMARK_SVG}
    <span style="font-family:'Crimson Pro','Cinzel',Georgia,serif;font-size:0.92rem;font-weight:600;letter-spacing:0.2em;color:rgba(201,168,76,0.92);">FISHER SOVEREIGN</span>
  </a>
  <a href="../home.html" style="font-size:0.72rem;font-weight:500;letter-spacing:0.06em;color:#6b6b75;text-decoration:none;">&larr; Portfolio</a>
</div>
"""

# Branded footer inserted before </body>. Replaces any existing <footer>...</footer>.
BRAND_FOOTER = f"""<!-- FSS-BRAND-FOOTER-V1 -->
<footer style="text-align:center;padding:60px 32px 26px;border-top:1px solid #26262c;background:#0a0a0b;font-family:'Inter','Helvetica Neue',Arial,sans-serif;">
  <div style="display:inline-flex;align-items:center;gap:14px;margin-bottom:14px;">
    {WORDMARK_SVG_LARGE}
    <span style="font-family:'Crimson Pro','Cinzel',Georgia,serif;font-size:1.4rem;font-weight:600;color:rgba(201,168,76,0.92);letter-spacing:0.22em;">FISHER SOVEREIGN</span>
  </div>
  <div style="font-size:0.68rem;font-weight:500;color:#6b6b75;text-transform:uppercase;letter-spacing:0.22em;margin-bottom:18px;">Building the Architecture for Independence</div>
  <p style="font-family:'Crimson Pro',Georgia,serif;font-style:italic;font-size:0.98rem;color:#a1a1aa;max-width:520px;margin:0 auto 28px;line-height:1.6;">Fisher Sovereign Systems is being built to return control to the individual.</p>
  <a href="../home.html" style="display:inline-block;padding:10px 24px;border:1px solid #8a7433;border-radius:6px;font-size:0.74rem;font-weight:500;color:#e3c068;letter-spacing:0.08em;text-transform:uppercase;text-decoration:none;background:rgba(201,168,76,0.04);">Visit lancewfisher.com &rarr;</a>
  <div style="margin-top:32px;padding-top:20px;border-top:1px solid rgba(255,255,255,0.05);font-size:0.66rem;color:#6b6b75;letter-spacing:0.06em;">&copy; 2026 Fisher Sovereign Systems, LLC. All rights reserved.</div>
</footer>
"""


def patch_page(path: Path) -> dict:
    if not path.exists():
        return {"ok": False, "reason": "missing"}

    src = path.read_text(encoding="utf-8")

    if FSS_BRAND_MARKER in src:
        return {"ok": True, "reason": "already-branded", "lines": src.count("\n") + 1}

    out = src

    # 1. Inject brand strip immediately after <body...> (preserving any attrs).
    out, n_strip = re.subn(
        r"(<body\b[^>]*>)",
        r"\1\n" + BRAND_STRIP,
        out,
        count=1,
        flags=re.IGNORECASE,
    )
    if n_strip == 0:
        return {"ok": False, "reason": "no <body>"}

    # 2. Replace any existing <footer>...</footer> with branded footer.
    #    Uses non-greedy so multiple footers stay independent.
    new_out, n_footer = re.subn(
        r"<footer\b[^>]*>.*?</footer>",
        BRAND_FOOTER.replace("\\", "\\\\"),
        out,
        flags=re.IGNORECASE | re.DOTALL,
    )
    if n_footer == 0:
        # No existing footer, insert before </body>.
        new_out, n_inject = re.subn(
            r"(</body\s*>)",
            BRAND_FOOTER.replace("\\", "\\\\") + r"\n\1",
            out,
            count=1,
            flags=re.IGNORECASE,
        )
        if n_inject == 0:
            return {"ok": False, "reason": "no </body>"}
        out = new_out
        footer_action = "appended"
    else:
        out = new_out
        footer_action = f"replaced({n_footer})"

    # 3. Em-dash removal: per Lance's non-negotiables.
    em_count_before = out.count("\u2014")
    out = out.replace("\u2014", ", ")

    # Idempotency safety: collapse double-replacements like ", , " just in case.
    out = re.sub(r",\s+,", ",", out)

    path.write_text(out, encoding="utf-8")

    return {
        "ok": True,
        "reason": "patched",
        "lines": out.count("\n") + 1,
        "footer": footer_action,
        "em_dashes_removed": em_count_before,
    }


def main():
    print("FSS brand patch pass starting...\n")
    results = []
    for slug in PAGES:
        path = DEPLOY / slug / "index.html"
        result = patch_page(path)
        result["page"] = slug
        results.append(result)
        if result["ok"]:
            extras = []
            if "footer" in result:
                extras.append(f"footer={result['footer']}")
            if "em_dashes_removed" in result:
                extras.append(f"emdash-={result['em_dashes_removed']}")
            extra_str = " ".join(extras)
            print(f"  [{result['reason']:>16}] {slug:24s} {result.get('lines','-')}L  {extra_str}")
        else:
            print(f"  [{'FAIL':>16}] {slug:24s} reason={result['reason']}")
    print()
    ok = sum(1 for r in results if r["ok"])
    print(f"Done: {ok}/{len(PAGES)} pages branded.")
    return 0 if ok == len(PAGES) else 1


if __name__ == "__main__":
    sys.exit(main())
