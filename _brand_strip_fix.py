#!/usr/bin/env python3
"""Convert the FSS brand strip from sticky to relative to avoid z-index
conflicts with existing per-page sticky banners (e.g. trading-desk DRY_RUN).
Idempotent: skips pages that already use the new style."""

from pathlib import Path

DEPLOY = Path(__file__).parent / "deploy"
PAGES = [
    "pip", "sovereign-trade-engine", "pm-executor", "prediction-market",
    "sovereign-hub", "trading-desk", "home-hub", "operator-console",
    "tax-platform", "auton",
]

OLD = (
    "position:sticky;top:0;z-index:99999;"
    "display:flex;align-items:center;justify-content:space-between;"
    "gap:14px;padding:9px 22px;"
    "background:rgba(8,8,10,0.96);"
    "backdrop-filter:blur(14px);-webkit-backdrop-filter:blur(14px);"
    "border-bottom:1px solid rgba(201,168,76,0.22);"
    "font-family:'Inter','Helvetica Neue',Arial,sans-serif;"
)

NEW = (
    "position:relative;"
    "display:flex;align-items:center;justify-content:space-between;"
    "gap:14px;padding:11px 22px;"
    "background:#08080a;"
    "border-bottom:1px solid rgba(201,168,76,0.28);"
    "font-family:'Inter','Helvetica Neue',Arial,sans-serif;"
)


def main():
    count = 0
    for slug in PAGES:
        p = DEPLOY / slug / "index.html"
        if not p.exists():
            print(f"  MISSING {slug}")
            continue
        s = p.read_text(encoding="utf-8")
        if OLD in s:
            s = s.replace(OLD, NEW)
            p.write_text(s, encoding="utf-8")
            count += 1
            print(f"  patched {slug}")
        elif NEW in s:
            print(f"  skip {slug} (already new style)")
        else:
            print(f"  NOT FOUND in {slug}")
    print(f"\nUpdated {count}/{len(PAGES)}")


if __name__ == "__main__":
    main()
