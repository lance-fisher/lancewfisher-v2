#!/usr/bin/env python3
"""
Bloodlines Design Bible — Interactive Viewer
Run: python bible_server.py
Open: http://localhost:8089

Four tabs:
  Bible    — Read-only design bible viewer
  Q&A      — Design questions with answers, staging queue
  Feedback — Proposed/reviewed/accepted corrections to the bible
  Ideas    — Free-form idea inbox with intelligent organization
"""

import http.server
import socketserver
import os
import json
import re
from datetime import datetime

PORT = 8089
DIR = os.path.dirname(os.path.abspath(__file__))

# ── JSON helpers ──────────────────────────────────────────────────────────────

def load_json(filename, default=None):
    path = os.path.join(DIR, filename)
    if not os.path.exists(path):
        return default if default is not None else {}
    with open(path, 'r', encoding='utf-8') as f:
        return json.load(f)

def save_json(filename, data):
    path = os.path.join(DIR, filename)
    with open(path, 'w', encoding='utf-8') as f:
        json.dump(data, f, indent=2, ensure_ascii=False)

def append_to_bible(section_num, content, tag):
    """Append content to the bible markdown at the end of a given section."""
    path = os.path.join(DIR, 'BLOODLINES_COMPLETE_DESIGN_BIBLE.md')
    with open(path, 'r', encoding='utf-8') as f:
        text = f.read()

    # Find the section heading
    pattern = rf'^## Section {section_num}:'
    lines = text.split('\n')
    insert_idx = None
    in_section = False
    for i, line in enumerate(lines):
        if re.match(pattern, line):
            in_section = True
            continue
        if in_section and re.match(r'^## Section \d+:', line):
            insert_idx = i
            break
    if in_section and insert_idx is None:
        insert_idx = len(lines)

    if insert_idx is not None:
        now = datetime.now().strftime('%Y-%m-%d %H:%M')
        block = f"\n---\n\n*[{tag} — {now}]*\n\n{content}\n"
        lines.insert(insert_idx, block)
        with open(path, 'w', encoding='utf-8') as f:
            f.write('\n'.join(lines))
        return True
    return False


# ── HTML ──────────────────────────────────────────────────────────────────────

HTML = r"""<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>Bloodlines — Design Bible</title>
<style>
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

:root {
  --bg:           #0b0a09;
  --bg-sidebar:   #0e0d0c;
  --bg-card:      #131210;
  --border:       #252015;
  --border-light: #302a1c;
  --text:         #cec3a8;
  --text-dim:     #6a6050;
  --text-bright:  #e4d9c0;
  --gold:         #c9a84c;
  --gold-dim:     #6e5c28;
  --gold-bright:  #e8c96a;
  --red:          #a04040;
  --green:        #4a8a4a;
  --blue:         #4a6a9a;
  --sidebar-w:    290px;
  --header-h:     54px;
}

html { scroll-behavior: smooth; }

body {
  font-family: 'Georgia', 'Times New Roman', serif;
  background: var(--bg);
  color: var(--text);
  line-height: 1.75;
  font-size: 16px;
}

#loading {
  position: fixed; inset: 0; background: var(--bg);
  display: flex; align-items: center; justify-content: center;
  z-index: 9999; flex-direction: column; gap: 14px;
}
.load-title { font-size: 30px; letter-spacing: 0.25em; text-transform: uppercase; color: var(--gold); font-weight: normal; }
.load-sub   { font-size: 11px; letter-spacing: 0.2em; text-transform: uppercase; color: var(--text-dim); }

/* ── Header ─────────────────────────────────────────── */
.site-header {
  position: fixed; top: 0; left: 0; right: 0;
  height: var(--header-h); background: var(--bg);
  border-bottom: 1px solid var(--border);
  display: flex; align-items: center; padding: 0 24px; z-index: 100; gap: 16px;
}
.header-title { font-size: 15px; letter-spacing: 0.18em; text-transform: uppercase; color: var(--gold); font-weight: normal; }
.header-title span { color: var(--text-dim); font-size: 11px; letter-spacing: 0.1em; margin-left: 14px; }

/* ── Tabs ─────────────────────────────────────────── */
.tab-bar { display: flex; gap: 0; margin-left: 12px; flex: 1; }
.tab-btn {
  padding: 4px 16px; font-family: inherit; font-size: 12px;
  letter-spacing: 0.1em; text-transform: uppercase;
  background: none; border: none; border-bottom: 2px solid transparent;
  color: var(--text-dim); cursor: pointer; transition: all 0.2s;
}
.tab-btn:hover { color: var(--text); }
.tab-btn.active { color: var(--gold); border-bottom-color: var(--gold); }
.tab-btn .tab-count {
  font-size: 10px; background: var(--bg-card); border: 1px solid var(--border);
  padding: 0 5px; border-radius: 8px; margin-left: 5px; color: var(--text-dim);
}

.header-count { font-size: 11px; color: var(--text-dim); letter-spacing: 0.05em; }
.search-wrap { position: relative; }
#search {
  background: var(--bg-card); border: 1px solid var(--border);
  color: var(--text); padding: 6px 30px 6px 12px;
  border-radius: 3px; font-family: inherit; font-size: 13px;
  width: 210px; outline: none; transition: border-color 0.2s;
}
#search:focus { border-color: var(--gold-dim); }
#search::placeholder { color: var(--text-dim); }
#searchClear {
  position: absolute; right: 8px; top: 50%; transform: translateY(-50%);
  background: none; border: none; color: var(--text-dim);
  cursor: pointer; font-size: 15px; padding: 0; display: none;
}
#searchClear.on { display: block; }

.save-flash {
  font-size: 10px; color: var(--green); letter-spacing: 0.08em;
  text-transform: uppercase; opacity: 0; transition: opacity 0.3s;
}
.save-flash.on { opacity: 1; }

/* ── Layout ─────────────────────────────────────────── */
.layout { display: flex; padding-top: var(--header-h); min-height: 100vh; }

.sidebar {
  position: fixed; top: var(--header-h); left: 0;
  width: var(--sidebar-w); height: calc(100vh - var(--header-h));
  background: var(--bg-sidebar); border-right: 1px solid var(--border);
  overflow-y: auto; z-index: 50; padding: 14px 0 24px;
}
.sidebar::-webkit-scrollbar { width: 3px; }
.sidebar::-webkit-scrollbar-thumb { background: var(--border-light); border-radius: 2px; }

.sb-label { font-size: 10px; letter-spacing: 0.22em; text-transform: uppercase; color: var(--text-dim); padding: 10px 18px 4px; }
.sb-item {
  display: flex; align-items: baseline; padding: 6px 18px;
  font-size: 13px; color: var(--text-dim); cursor: pointer;
  border-left: 2px solid transparent; line-height: 1.4; gap: 6px;
  text-decoration: none; transition: color 0.15s, background 0.15s;
}
.sb-item:hover { color: var(--text); background: rgba(201,168,76,0.04); }
.sb-item.active { color: var(--gold); border-left-color: var(--gold); background: rgba(201,168,76,0.07); }
.sb-item.hidden { display: none; }
.sb-num { color: var(--gold-dim); font-size: 11px; flex-shrink: 0; min-width: 22px; }
.sb-no-results { padding: 14px 18px; color: var(--text-dim); font-size: 13px; display: none; }

.main { margin-left: var(--sidebar-w); flex: 1; padding: 52px 64px 100px; max-width: 940px; }
.tab-view { display: none; }
.tab-view.active { display: block; }

/* ── Bible Styles ─────────────────────────────────────────── */
.doc-title { font-size: 26px; letter-spacing: 0.1em; text-transform: uppercase; color: var(--gold-bright); font-weight: normal; margin-bottom: 6px; line-height: 1.3; }

h2.s-head {
  font-size: 21px; color: var(--gold); font-weight: normal;
  letter-spacing: 0.05em; margin: 52px 0 14px;
  padding-bottom: 9px; border-bottom: 1px solid var(--border-light);
  scroll-margin-top: calc(var(--header-h) + 52px);
}
h2.s-head .s-num { color: var(--gold-dim); margin-right: 8px; font-size: 17px; }
h3 { font-size: 15px; color: var(--text-bright); font-weight: normal; letter-spacing: 0.04em; margin: 26px 0 9px; }
h4 { font-size: 13px; color: var(--gold-dim); font-weight: normal; letter-spacing: 0.12em; text-transform: uppercase; margin: 18px 0 7px; }

p { margin-bottom: 13px; }
strong { color: var(--text-bright); }
em { font-style: italic; }
a { color: var(--gold-dim); text-decoration: none; }
a:hover { color: var(--gold); }
hr { border: none; border-top: 1px solid var(--border); margin: 26px 0; }
ul, ol { margin: 8px 0 13px 22px; }
li { margin-bottom: 4px; }
code { font-family: 'Courier New', monospace; font-size: 13px; background: rgba(255,255,255,0.05); padding: 1px 5px; border-radius: 3px; color: var(--gold-bright); }

.badge { display: inline-block; padding: 1px 7px; border-radius: 3px; font-size: 10px; letter-spacing: 0.1em; text-transform: uppercase; font-family: 'Georgia', serif; vertical-align: middle; margin: 0 1px; }
.b-locked   { background: rgba(201,168,76,0.12); color: var(--gold);   border: 1px solid var(--gold-dim); }
.b-strong   { background: rgba(80,160,80,0.1);   color: #7ac87a;       border: 1px solid #2a6a2a; }
.b-partial  { background: rgba(160,130,60,0.1);  color: #b8a06a;       border: 1px solid #5a4a20; }
.b-proposed { background: rgba(80,110,180,0.1);  color: #7a9ad8;       border: 1px solid #2a4a88; }
.b-open     { background: rgba(120,80,80,0.1);   color: #b07878;       border: 1px solid #5a2a2a; }

table { width: 100%; border-collapse: collapse; margin: 14px 0 18px; font-size: 14px; }
th { background: rgba(201,168,76,0.07); color: var(--gold); text-align: left; padding: 7px 11px; border: 1px solid var(--border-light); font-weight: normal; letter-spacing: 0.04em; }
td { padding: 7px 11px; border: 1px solid var(--border); vertical-align: top; }
tr:nth-child(even) td { background: rgba(255,255,255,0.015); }

.ornament { text-align: center; color: var(--gold-dim); font-size: 12px; margin: 14px 0; letter-spacing: 6px; user-select: none; }

#toTop {
  position: fixed; bottom: 28px; right: 28px;
  background: var(--bg-card); border: 1px solid var(--border-light);
  color: var(--text-dim); padding: 8px 12px; border-radius: 3px;
  cursor: pointer; font-size: 12px; letter-spacing: 0.08em;
  text-transform: uppercase; font-family: 'Georgia', serif;
  opacity: 0; pointer-events: none; transition: opacity 0.2s, color 0.15s, border-color 0.15s;
}
#toTop.on { opacity: 1; pointer-events: auto; }
#toTop:hover { color: var(--gold); border-color: var(--gold-dim); }

/* ── Q&A Styles ─────────────────────────────────────────── */
.qa-filters { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 24px; }
.qa-filter-btn {
  padding: 4px 12px; font-size: 11px; letter-spacing: 0.06em;
  border: 1px solid var(--border); background: var(--bg-card);
  color: var(--text-dim); cursor: pointer; font-family: inherit;
  transition: all 0.2s; text-transform: uppercase;
}
.qa-filter-btn.active { border-color: var(--gold-dim); color: var(--gold); background: rgba(201,168,76,0.06); }
.qa-filter-btn:hover { color: var(--text); border-color: var(--border-light); }

.qa-group-title {
  font-size: 16px; color: var(--gold); margin: 32px 0 12px;
  padding-bottom: 6px; border-bottom: 1px solid var(--border);
  display: flex; align-items: center; gap: 8px;
}
.qa-group-title .link-icon { font-size: 12px; color: var(--text-dim); cursor: pointer; text-decoration: none; }
.qa-group-title .link-icon:hover { color: var(--gold); }

.qa-card {
  background: var(--bg-card); border: 1px solid var(--border);
  padding: 16px 20px; margin-bottom: 12px;
}
.qa-card-header { display: flex; align-items: center; gap: 10px; margin-bottom: 8px; }
.qa-card-id { font-size: 11px; color: var(--gold-dim); letter-spacing: 0.06em; min-width: 40px; }
.qa-card-text { font-size: 14px; color: var(--text-bright); flex: 1; }
.qa-severity {
  font-size: 9px; letter-spacing: 0.1em; text-transform: uppercase;
  padding: 2px 8px; border-radius: 2px; font-family: inherit;
}
.sev-blocking  { background: rgba(160,64,64,0.15); color: #c06060; border: 1px solid #5a2a2a; }
.sev-important { background: rgba(180,140,50,0.12); color: #c0a040; border: 1px solid #5a4a20; }
.sev-enriching { background: rgba(80,130,180,0.1);  color: #70a0d0; border: 1px solid #2a4a6a; }
.sev-future    { background: rgba(80,80,80,0.1);    color: #909090; border: 1px solid #404040; }

.qa-desc { font-size: 13px; color: var(--text-dim); margin-bottom: 10px; }
.qa-options { font-size: 13px; color: var(--text-dim); margin-bottom: 10px; padding-left: 16px; }
.qa-options li { margin-bottom: 3px; }

.qa-textarea {
  width: 100%; min-height: 80px; padding: 10px 12px;
  background: var(--bg); border: 1px solid var(--border);
  color: var(--text); font-family: inherit; font-size: 13px;
  line-height: 1.6; resize: vertical; outline: none;
}
.qa-textarea:focus { border-color: var(--gold-dim); }
.qa-textarea::placeholder { color: var(--text-dim); }

.qa-actions { display: flex; gap: 8px; margin-top: 8px; align-items: center; }
.qa-btn {
  padding: 5px 14px; font-size: 11px; letter-spacing: 0.06em;
  border: 1px solid var(--border); background: var(--bg);
  color: var(--text-dim); cursor: pointer; font-family: inherit;
  transition: all 0.2s; text-transform: uppercase;
}
.qa-btn:hover { color: var(--gold); border-color: var(--gold-dim); }
.qa-btn.staged { border-color: var(--green); color: var(--green); }

/* ── Staging Queue ─────────────────────────────────────────── */
.stage-queue {
  background: rgba(74,138,74,0.05); border: 1px solid rgba(74,138,74,0.2);
  padding: 16px 20px; margin-bottom: 24px; display: none;
}
.stage-queue.has-items { display: block; }
.stage-queue-title {
  font-size: 13px; color: var(--green); letter-spacing: 0.1em;
  text-transform: uppercase; margin-bottom: 12px;
}
.stage-item {
  display: flex; align-items: center; gap: 12px;
  padding: 8px 0; border-bottom: 1px solid var(--border);
  font-size: 13px;
}
.stage-item:last-child { border-bottom: none; }
.stage-item-text { flex: 1; color: var(--text); }
.stage-item-section { color: var(--text-dim); font-size: 11px; }

/* ── Feedback Styles ─────────────────────────────────────────── */
.fb-form {
  background: var(--bg-card); border: 1px solid var(--border);
  padding: 20px; margin-bottom: 24px;
}
.fb-form-title { font-size: 14px; color: var(--gold); margin-bottom: 12px; letter-spacing: 0.05em; }
.fb-form label { display: block; font-size: 12px; color: var(--text-dim); margin-bottom: 4px; letter-spacing: 0.04em; text-transform: uppercase; }
.fb-form select, .fb-form input[type=text] {
  width: 100%; padding: 8px 10px; background: var(--bg); border: 1px solid var(--border);
  color: var(--text); font-family: inherit; font-size: 13px; margin-bottom: 12px; outline: none;
}
.fb-form select:focus, .fb-form input[type=text]:focus { border-color: var(--gold-dim); }
.fb-form textarea {
  width: 100%; min-height: 120px; padding: 10px 12px;
  background: var(--bg); border: 1px solid var(--border);
  color: var(--text); font-family: inherit; font-size: 13px;
  line-height: 1.6; resize: vertical; outline: none; margin-bottom: 12px;
}
.fb-form textarea:focus { border-color: var(--gold-dim); }

.fb-item {
  background: var(--bg-card); border: 1px solid var(--border);
  padding: 16px 20px; margin-bottom: 10px;
}
.fb-item-header { display: flex; align-items: center; gap: 10px; margin-bottom: 8px; }
.fb-item-section { font-size: 12px; color: var(--gold-dim); }
.fb-item-title { font-size: 14px; color: var(--text-bright); flex: 1; }
.fb-status {
  font-size: 9px; letter-spacing: 0.1em; text-transform: uppercase;
  padding: 2px 8px; border-radius: 2px;
}
.fb-proposed { background: rgba(80,110,180,0.15); color: #7a9ad8; border: 1px solid #2a4a88; }
.fb-reviewed { background: rgba(180,140,50,0.12); color: #c0a040; border: 1px solid #5a4a20; }
.fb-accepted { background: rgba(80,160,80,0.15); color: #7ac87a; border: 1px solid #2a6a2a; }
.fb-item-body { font-size: 13px; color: var(--text); margin-bottom: 10px; white-space: pre-wrap; }
.fb-item-date { font-size: 11px; color: var(--text-dim); }
.fb-item-actions { display: flex; gap: 8px; margin-top: 8px; }

/* ── Ideas Styles ─────────────────────────────────────────── */
.ideas-input-area {
  background: var(--bg-card); border: 1px solid var(--border);
  padding: 20px; margin-bottom: 24px;
}
.ideas-input-title { font-size: 14px; color: var(--gold); margin-bottom: 8px; letter-spacing: 0.05em; }
.ideas-input-sub { font-size: 12px; color: var(--text-dim); margin-bottom: 12px; }
#ideasText {
  width: 100%; min-height: 300px; padding: 14px 16px;
  background: var(--bg); border: 1px solid var(--border);
  color: var(--text); font-family: inherit; font-size: 14px;
  line-height: 1.7; resize: vertical; outline: none;
}
#ideasText:focus { border-color: var(--gold-dim); }
#ideasText::placeholder { color: var(--text-dim); }

.ideas-results { margin-top: 20px; }
.ideas-group {
  margin-bottom: 20px;
}
.ideas-group-title {
  font-size: 14px; color: var(--gold); margin-bottom: 8px;
  padding-bottom: 4px; border-bottom: 1px solid var(--border);
}
.idea-card {
  background: var(--bg-card); border: 1px solid var(--border);
  padding: 12px 16px; margin-bottom: 8px;
}
.idea-card-text { font-size: 13px; color: var(--text); margin-bottom: 8px; white-space: pre-wrap; }
.idea-card-actions { display: flex; gap: 8px; align-items: center; }
.idea-card-actions select {
  padding: 4px 8px; background: var(--bg); border: 1px solid var(--border);
  color: var(--text); font-family: inherit; font-size: 11px; outline: none;
}
.idea-suggestions { display: flex; gap: 4px; flex-wrap: wrap; }
.idea-suggest-pill {
  font-size: 10px; padding: 2px 8px; border: 1px solid var(--gold-dim);
  color: var(--gold-dim); background: none; cursor: pointer;
  font-family: inherit; transition: all 0.2s;
}
.idea-suggest-pill:hover { color: var(--gold); border-color: var(--gold); background: rgba(201,168,76,0.06); }

.ideas-history { margin-top: 32px; }
.ideas-history-title {
  font-size: 13px; color: var(--text-dim); letter-spacing: 0.1em;
  text-transform: uppercase; margin-bottom: 12px; cursor: pointer;
}
.ideas-history-title:hover { color: var(--text); }

/* ── Progress bar ─────────────────────────────────────────── */
.progress-bar {
  display: flex; align-items: center; gap: 8px; font-size: 11px;
  color: var(--text-dim); letter-spacing: 0.04em;
}
.progress-track {
  width: 80px; height: 4px; background: var(--border);
  border-radius: 2px; overflow: hidden;
}
.progress-fill { height: 100%; background: var(--gold-dim); transition: width 0.3s; }

/* ── Reading Progress Bar ─────────────────────────────────── */
#readingProgress {
  position: fixed; top: 0; left: 0; right: 0;
  height: 2px; background: transparent; z-index: 200;
  pointer-events: none;
}
#readingProgressFill {
  height: 100%; width: 0%; background: var(--gold);
  transition: width 0.1s linear;
}

/* ── Hamburger Button ─────────────────────────────────────── */
.hamburger-btn {
  display: none; background: none; border: none;
  color: var(--gold); font-size: 22px; cursor: pointer;
  padding: 4px 8px; line-height: 1; flex-shrink: 0;
}
.hamburger-btn:hover { color: var(--gold-bright); }

/* ── Mobile Overlay ──────────────────────────────────────── */
.sidebar-backdrop {
  display: none; position: fixed; inset: 0;
  background: rgba(0,0,0,0.6); z-index: 90;
}
.sidebar-backdrop.open { display: block; }

/* ── Floating Section Indicator / Breadcrumb ─────────────── */
#sectionIndicator {
  position: fixed; top: var(--header-h); left: 0; right: 0;
  height: 32px; background: var(--bg-sidebar);
  border-bottom: 1px solid var(--border);
  display: flex; align-items: center; padding: 0 24px;
  z-index: 95; font-size: 12px; color: var(--gold);
  letter-spacing: 0.06em; cursor: pointer;
  opacity: 0; pointer-events: none;
  transition: opacity 0.2s;
}
#sectionIndicator.on {
  opacity: 1; pointer-events: auto;
}
#sectionIndicator:hover { color: var(--gold-bright); }
#sectionIndicator .indicator-arrow {
  color: var(--text-dim); font-size: 10px; margin-left: 6px;
}

/* ── Quick-Jump Dropdown ─────────────────────────────────── */
#quickJump {
  position: fixed; top: calc(var(--header-h) + 32px); left: 0;
  width: 340px; max-height: 60vh; overflow-y: auto;
  background: var(--bg-sidebar); border: 1px solid var(--border-light);
  z-index: 96; display: none; padding: 8px 0;
  box-shadow: 0 8px 24px rgba(0,0,0,0.5);
}
#quickJump.open { display: block; }
#quickJump .qj-item {
  display: flex; align-items: baseline; padding: 6px 16px;
  font-size: 12px; color: var(--text-dim); cursor: pointer;
  gap: 6px; transition: color 0.15s, background 0.15s;
  text-decoration: none;
}
#quickJump .qj-item:hover { color: var(--text); background: rgba(201,168,76,0.04); }
#quickJump .qj-item.active { color: var(--gold); background: rgba(201,168,76,0.07); }
#quickJump .qj-num { color: var(--gold-dim); font-size: 10px; min-width: 22px; flex-shrink: 0; }
#quickJump::-webkit-scrollbar { width: 3px; }
#quickJump::-webkit-scrollbar-thumb { background: var(--border-light); border-radius: 2px; }

/* ── Prev/Next Floating Nav ──────────────────────────────── */
#navPrevNext {
  position: fixed; bottom: 28px; left: 50%; transform: translateX(-50%);
  display: flex; gap: 8px; z-index: 80;
  opacity: 0; pointer-events: none; transition: opacity 0.2s;
}
#navPrevNext.on { opacity: 1; pointer-events: auto; }
.nav-pn-btn {
  background: var(--bg-card); border: 1px solid var(--border-light);
  color: var(--text-dim); padding: 6px 14px; border-radius: 3px;
  cursor: pointer; font-size: 12px; font-family: 'Georgia', serif;
  letter-spacing: 0.04em; transition: color 0.15s, border-color 0.15s;
  max-width: 200px; overflow: hidden; text-overflow: ellipsis;
  white-space: nowrap; text-align: center;
}
.nav-pn-btn:hover { color: var(--gold); border-color: var(--gold-dim); }
.nav-pn-btn:disabled { opacity: 0.3; cursor: default; }
.nav-pn-btn:disabled:hover { color: var(--text-dim); border-color: var(--border-light); }
.nav-pn-arrow { font-size: 14px; vertical-align: middle; }

/* ── Responsive ─────────────────────────────────────────── */
@media (max-width: 800px) {
  .hamburger-btn { display: block; }
  .sidebar {
    display: block; transform: translateX(-100%);
    transition: transform 0.25s ease; z-index: 91;
    top: 0; height: 100vh; padding-top: calc(var(--header-h) + 14px);
  }
  .sidebar.mobile-open { transform: translateX(0); }
  .main { margin-left: 0; padding: 24px 16px 60px; }
  .tab-bar { gap: 0; }
  .tab-btn { padding: 4px 8px; font-size: 10px; }
  #search { width: 140px; }
  .header-title span { display: none; }
  #sectionIndicator { padding: 0 16px; }
  #quickJump { width: calc(100vw - 24px); left: 12px; }
  #navPrevNext { left: 16px; right: 16px; transform: none; justify-content: space-between; }
  .nav-pn-btn { max-width: 45vw; }
}
</style>
</head>
<body>

<div id="loading">
  <div class="load-title">Bloodlines</div>
  <div class="load-sub">Loading Design Bible&hellip;</div>
</div>

<div id="readingProgress"><div id="readingProgressFill"></div></div>

<header class="site-header">
  <button class="hamburger-btn" id="hamburgerBtn" title="Menu">&#9776;</button>
  <div class="header-title">Bloodlines</div>
  <div class="tab-bar">
    <button class="tab-btn active" data-tab="bible">Bible</button>
    <button class="tab-btn" data-tab="qa">Q&amp;A <span class="tab-count" id="qaCount"></span></button>
    <button class="tab-btn" data-tab="feedback">Feedback <span class="tab-count" id="fbCount"></span></button>
    <button class="tab-btn" data-tab="ideas">Ideas</button>
  </div>
  <span class="header-count" id="sectionCount"></span>
  <span class="save-flash" id="saveFlash">Saved</span>
  <div class="search-wrap">
    <input type="text" id="search" placeholder="Search sections&hellip;" autocomplete="off" spellcheck="false">
    <button id="searchClear" title="Clear">&times;</button>
  </div>
</header>

<div class="sidebar-backdrop" id="sidebarBackdrop"></div>
<div id="sectionIndicator"><span id="indicatorText"></span><span class="indicator-arrow">&#9662;</span></div>
<div id="quickJump"></div>

<div id="navPrevNext">
  <button class="nav-pn-btn" id="navPrev" title="Previous section"><span class="nav-pn-arrow">&larr;</span> <span id="navPrevLabel">Prev</span></button>
  <button class="nav-pn-btn" id="navNext" title="Next section"><span id="navNextLabel">Next</span> <span class="nav-pn-arrow">&rarr;</span></button>
</div>

<div class="layout">
  <nav class="sidebar" id="sidebar">
    <div class="sb-label">Sections</div>
    <div id="sbItems"></div>
    <div class="sb-no-results" id="noResults">No sections found</div>
  </nav>

  <!-- BIBLE TAB -->
  <main class="main tab-view active" id="view-bible"></main>

  <!-- Q&A TAB -->
  <main class="main tab-view" id="view-qa">
    <div class="doc-title">Design Questions</div>
    <div class="progress-bar" id="qaProgress">
      <div class="progress-track"><div class="progress-fill" id="qaProgressFill"></div></div>
      <span id="qaProgressText">0 / 0 answered</span>
    </div>
    <div class="qa-filters" id="qaFilters">
      <button class="qa-filter-btn active" data-filter="all">All</button>
      <button class="qa-filter-btn" data-filter="blocking">Blocking</button>
      <button class="qa-filter-btn" data-filter="important">Important</button>
      <button class="qa-filter-btn" data-filter="enriching">Enriching</button>
      <button class="qa-filter-btn" data-filter="unanswered">Unanswered</button>
      <button class="qa-filter-btn" data-filter="answered">Answered</button>
    </div>
    <div class="stage-queue" id="stageQueue">
      <div class="stage-queue-title">&#9733; Staged for Bible Review</div>
      <div id="stageItems"></div>
    </div>
    <div id="qaCards"></div>
  </main>

  <!-- FEEDBACK TAB -->
  <main class="main tab-view" id="view-feedback">
    <div class="doc-title">Design Feedback</div>
    <p style="color:var(--text-dim);font-size:13px;margin-bottom:20px;">Submit corrections, additions, or refinements to the design bible. Items flow: Proposed &#8594; Reviewed &#8594; Accepted.</p>
    <div class="fb-form" id="fbForm">
      <div class="fb-form-title">Submit New Feedback</div>
      <label>Target Section</label>
      <select id="fbSection"></select>
      <label>Title</label>
      <input type="text" id="fbTitle" placeholder="Brief description of the feedback">
      <label>Content</label>
      <textarea id="fbContent" placeholder="Write your feedback, correction, or addition here..."></textarea>
      <button class="qa-btn" onclick="submitFeedback()">Submit as Proposed</button>
    </div>
    <div class="qa-filters" id="fbFilters">
      <button class="qa-filter-btn active" data-fbfilter="all">All</button>
      <button class="qa-filter-btn" data-fbfilter="proposed">Proposed</button>
      <button class="qa-filter-btn" data-fbfilter="reviewed">Reviewed</button>
      <button class="qa-filter-btn" data-fbfilter="accepted">Accepted</button>
    </div>
    <div id="fbItems"></div>
  </main>

  <!-- IDEAS TAB -->
  <main class="main tab-view" id="view-ideas">
    <div class="doc-title">Ideas Inbox</div>
    <div class="ideas-input-area">
      <div class="ideas-input-title">Paste or type game ideas</div>
      <div class="ideas-input-sub">Separate distinct ideas with blank lines. The system will categorize them by bible section.</div>
      <textarea id="ideasText" placeholder="Type your ideas here...&#10;&#10;Separate each idea with a blank line.&#10;&#10;The organizer will match keywords to bible sections and suggest where each idea belongs."></textarea>
      <div style="margin-top:12px;display:flex;gap:8px;">
        <button class="qa-btn" onclick="organizeIdeas()">Organize</button>
        <button class="qa-btn" id="ideasSaveBtn" onclick="saveIdeas()" style="display:none;">Save All</button>
      </div>
    </div>
    <div class="ideas-results" id="ideasResults"></div>
    <div class="ideas-history" id="ideasHistory"></div>
  </main>
</div>

<button id="toTop" onclick="window.scrollTo({top:0,behavior:'smooth'})">&#8593; Top</button>

<script>
// ── Globals ──────────────────────────────────────────────────────────────

let questionMap = null;
let qaResponses = {};
let feedbackItems = [];
let ideasData = { submissions: [] };
let organizedIdeas = [];
let activeTab = 'bible';
let activeId = null;

// ── Utilities ────────────────────────────────────────────────────────────

function escText(s) {
  return s.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;');
}

function buildInlineHTML(raw) {
  let out = '', i = 0;
  while (i < raw.length) {
    if (raw[i]==='*' && raw[i+1]==='*') {
      const end = raw.indexOf('**', i+2);
      if (end !== -1) {
        const inner = raw.slice(i+2, end).trim();
        const u = inner.toUpperCase();
        if      (u==='LOCKED')               out += '<span class="badge b-locked">Locked</span>';
        else if (u==='STRONGLY ESTABLISHED') out += '<span class="badge b-strong">Strongly Established</span>';
        else if (u==='PARTIALLY DEFINED')    out += '<span class="badge b-partial">Partially Defined</span>';
        else if (u==='PROPOSED')             out += '<span class="badge b-proposed">Proposed</span>';
        else if (u==='OPEN')                 out += '<span class="badge b-open">Open</span>';
        else                                 out += '<strong>' + escText(inner) + '</strong>';
        i = end + 2; continue;
      }
    }
    if (raw[i]==='*' && raw[i+1]!=='*') {
      const end = raw.indexOf('*', i+1);
      if (end !== -1) { out += '<em>' + escText(raw.slice(i+1,end)) + '</em>'; i = end+1; continue; }
    }
    if (raw[i]==='`') {
      const end = raw.indexOf('`', i+1);
      if (end !== -1) { out += '<code>' + escText(raw.slice(i+1,end)) + '</code>'; i = end+1; continue; }
    }
    if (raw[i]==='[') {
      const cb = raw.indexOf(']', i+1);
      if (cb !== -1 && raw[cb+1]==='(') {
        const pe = raw.indexOf(')', cb+2);
        if (pe !== -1) {
          const label = escText(raw.slice(i+1,cb));
          const href  = escText(raw.slice(cb+2,pe));
          out += '<a href="' + href + '">' + label + '</a>';
          i = pe+1; continue;
        }
      }
    }
    const c = raw[i];
    if      (c==='&') out += '&amp;';
    else if (c==='<') out += '&lt;';
    else if (c==='>') out += '&gt;';
    else               out += c;
    i++;
  }
  return out;
}

function slugify(t) {
  return t.toLowerCase().replace(/[^a-z0-9 -]/g,'').replace(/\s+/g,'-').replace(/-+/g,'-').trim();
}

function flashSave() {
  const el = document.getElementById('saveFlash');
  el.classList.add('on');
  setTimeout(() => el.classList.remove('on'), 1500);
}

async function postJSON(url, data) {
  const res = await fetch(url, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  });
  return res.json();
}

// ── Bible Parser ─────────────────────────────────────────────────────────

function parseMarkdown(md) {
  const lines   = md.split('\n');
  const sections = [];
  let cur = null;
  let nodes = [];

  let inUl = false, ulEl = null;
  let inOl = false, olEl = null;
  let inTbl = false, tblRows = [];

  function closeList() {
    if (inUl) { inUl = false; ulEl = null; }
    if (inOl) { inOl = false; olEl = null; }
  }

  function flushTable() {
    if (!inTbl) return;
    inTbl = false;
    const table = document.createElement('table');
    let thead = null, tbody = null;
    tblRows.forEach((row, ri) => {
      const cells = row.split('|').slice(1,-1).map(c => c.trim());
      if (ri === 1 && cells.every(c => /^[-: ]+$/.test(c))) return;
      const tr = document.createElement('tr');
      cells.forEach(c => {
        const cell = document.createElement(ri===0 ? 'th' : 'td');
        cell.insertAdjacentHTML('afterbegin', buildInlineHTML(c));
        tr.appendChild(cell);
      });
      if (ri === 0) {
        thead = document.createElement('thead');
        thead.appendChild(tr);
        table.appendChild(thead);
        tbody = document.createElement('tbody');
        table.appendChild(tbody);
      } else {
        (tbody || table).appendChild(tr);
      }
    });
    nodes.push(table);
    tblRows = [];
  }

  function pushSection() {
    closeList(); flushTable();
    if (cur) sections.push({ ...cur, nodes });
    nodes = [];
  }

  function makeInlineEl(tag, raw, attrs) {
    const el = document.createElement(tag);
    attrs && Object.entries(attrs).forEach(([k,v]) => el.setAttribute(k,v));
    el.insertAdjacentHTML('afterbegin', buildInlineHTML(raw));
    return el;
  }

  lines.forEach(line => {
    if (/^# /.test(line)) {
      pushSection();
      const t = line.slice(2).trim();
      cur = { id: 'title', num: '', title: t };
      const d = document.createElement('div');
      d.className = 'doc-title';
      d.textContent = t;
      nodes.push(d);
      return;
    }
    if (/^## /.test(line)) {
      pushSection();
      const t = line.slice(3).trim();
      const m = t.match(/^(\d+)\.\s+(.+)/) || t.match(/^Section\s+(\d+):\s+(.+)/);
      const num   = m ? m[1] : '';
      const title = m ? m[2] : t;
      const id    = slugify(t);
      cur = { id, num, title };
      const h = document.createElement('h2');
      h.className = 's-head';
      h.id = id;
      if (num) {
        const ns = document.createElement('span');
        ns.className = 's-num';
        ns.textContent = num + '.';
        h.appendChild(ns);
      }
      h.appendChild(document.createTextNode(title));
      nodes.push(h);
      return;
    }
    if (/^### /.test(line)) {
      closeList(); flushTable();
      nodes.push(makeInlineEl('h3', line.slice(4).trim()));
      return;
    }
    if (/^#### /.test(line)) {
      closeList(); flushTable();
      nodes.push(makeInlineEl('h4', line.slice(5).trim()));
      return;
    }
    if (/^---+$/.test(line.trim())) {
      closeList(); flushTable();
      const d = document.createElement('div');
      d.className = 'ornament';
      d.textContent = '\u25c6 \u25c6 \u25c6';
      nodes.push(d);
      return;
    }
    if (/^\|/.test(line)) {
      closeList();
      inTbl = true;
      tblRows.push(line);
      return;
    }
    if (inTbl) flushTable();

    if (/^[*-] /.test(line)) {
      if (inOl) { inOl = false; olEl = null; }
      if (!inUl) { ulEl = document.createElement('ul'); nodes.push(ulEl); inUl = true; }
      ulEl.appendChild(makeInlineEl('li', line.slice(2).trim()));
      return;
    }
    if (/^\d+\. /.test(line)) {
      if (inUl) { inUl = false; ulEl = null; }
      if (!inOl) { olEl = document.createElement('ol'); nodes.push(olEl); inOl = true; }
      olEl.appendChild(makeInlineEl('li', line.replace(/^\d+\. /,'').trim()));
      return;
    }
    if (line.trim() === '') { closeList(); return; }
    closeList();
    nodes.push(makeInlineEl('p', line.trim()));
  });

  pushSection();
  return sections;
}

// ── Tab Switching ────────────────────────────────────────────────────────

function switchTab(tab) {
  activeTab = tab;
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.toggle('active', b.dataset.tab === tab));
  document.querySelectorAll('.tab-view').forEach(v => v.classList.toggle('active', v.id === 'view-' + tab));
  const sidebar = document.getElementById('sidebar');
  const isMobile = window.innerWidth <= 800;
  if (!isMobile) {
    sidebar.style.display = tab === 'bible' ? '' : 'none';
  }
  document.querySelector('.main.tab-view.active').style.marginLeft = tab === 'bible' && !isMobile ? '' : '0';
  const indicator = document.getElementById('sectionIndicator');
  const navPN = document.getElementById('navPrevNext');
  if (tab !== 'bible') {
    indicator.classList.remove('on');
    navPN.classList.remove('on');
  }
  closeMobileMenu();
  location.hash = tab === 'bible' ? '' : tab;
}

// ── Mobile Menu ─────────────────────────────────────────────────────────

function openMobileMenu() {
  document.getElementById('sidebar').classList.add('mobile-open');
  document.getElementById('sidebarBackdrop').classList.add('open');
}

function closeMobileMenu() {
  document.getElementById('sidebar').classList.remove('mobile-open');
  document.getElementById('sidebarBackdrop').classList.remove('open');
}

function toggleMobileMenu() {
  const sidebar = document.getElementById('sidebar');
  if (sidebar.classList.contains('mobile-open')) closeMobileMenu();
  else openMobileMenu();
}

// ── Q&A Rendering ────────────────────────────────────────────────────────

let qaDebounceTimers = {};

function renderQA() {
  if (!questionMap) return;
  const container = document.getElementById('qaCards');
  container.textContent = '';

  // Group questions by bible section
  const groups = {};
  const sectionOrder = [];
  questionMap.questions.forEach(q => {
    const secId = q.bible_sections[0] || 'uncategorized';
    if (!groups[secId]) {
      groups[secId] = [];
      sectionOrder.push(secId);
    }
    groups[secId].push(q);
  });

  let answered = 0, total = questionMap.questions.length;
  questionMap.questions.forEach(q => { if (qaResponses[q.id]?.answer) answered++; });

  // Progress
  document.getElementById('qaProgressFill').style.width = total ? (answered/total*100) + '%' : '0%';
  document.getElementById('qaProgressText').textContent = answered + ' / ' + total + ' answered';
  document.getElementById('qaCount').textContent = answered + '/' + total;

  // Active filter
  const activeFilter = document.querySelector('.qa-filter-btn.active')?.dataset.filter || 'all';

  sectionOrder.forEach(secId => {
    const secInfo = questionMap.section_keywords[secId];
    const secTitle = secInfo ? secInfo.title : secId;
    const questions = groups[secId];

    // Filter
    const visible = questions.filter(q => {
      const hasAnswer = qaResponses[q.id]?.answer;
      if (activeFilter === 'unanswered' && hasAnswer) return false;
      if (activeFilter === 'answered' && !hasAnswer) return false;
      if (activeFilter === 'blocking' && q.severity?.toLowerCase() !== 'blocking') return false;
      if (activeFilter === 'important' && q.severity?.toLowerCase() !== 'important') return false;
      if (activeFilter === 'enriching' && q.severity?.toLowerCase() !== 'enriching') return false;
      return true;
    });
    if (visible.length === 0) return;

    const groupDiv = document.createElement('div');
    const titleDiv = document.createElement('div');
    titleDiv.className = 'qa-group-title';
    titleDiv.textContent = secTitle + ' ';
    const linkIcon = document.createElement('a');
    linkIcon.className = 'link-icon';
    linkIcon.textContent = '\u2197';
    linkIcon.title = 'View in Bible';
    linkIcon.href = '#';
    linkIcon.onclick = (e) => { e.preventDefault(); switchTab('bible'); setTimeout(() => { const el = document.getElementById(slugify('Section ' + secId.replace('section-','') + ': ' + secTitle)); if (el) el.scrollIntoView({behavior:'smooth'}); }, 100); };
    titleDiv.appendChild(linkIcon);
    groupDiv.appendChild(titleDiv);

    visible.forEach(q => {
      const card = document.createElement('div');
      card.className = 'qa-card';

      // Header
      const header = document.createElement('div');
      header.className = 'qa-card-header';
      const idSpan = document.createElement('span');
      idSpan.className = 'qa-card-id';
      idSpan.textContent = q.id;
      header.appendChild(idSpan);
      const textSpan = document.createElement('span');
      textSpan.className = 'qa-card-text';
      textSpan.textContent = q.text;
      header.appendChild(textSpan);
      if (q.severity) {
        const sev = document.createElement('span');
        sev.className = 'qa-severity sev-' + q.severity.toLowerCase();
        sev.textContent = q.severity;
        header.appendChild(sev);
      }
      card.appendChild(header);

      // Description
      if (q.description) {
        const desc = document.createElement('div');
        desc.className = 'qa-desc';
        desc.textContent = q.description;
        card.appendChild(desc);
      }

      // Options
      if (q.options && q.options.length > 0) {
        const optList = document.createElement('ul');
        optList.className = 'qa-options';
        q.options.forEach(o => {
          const li = document.createElement('li');
          li.textContent = o;
          optList.appendChild(li);
        });
        card.appendChild(optList);
      }

      // Textarea
      const ta = document.createElement('textarea');
      ta.className = 'qa-textarea';
      ta.placeholder = 'Your answer...';
      ta.value = qaResponses[q.id]?.answer || '';
      ta.addEventListener('input', () => {
        clearTimeout(qaDebounceTimers[q.id]);
        qaDebounceTimers[q.id] = setTimeout(() => saveQAResponse(q.id, ta.value), 2000);
      });
      card.appendChild(ta);

      // Actions
      const actions = document.createElement('div');
      actions.className = 'qa-actions';
      const stageBtn = document.createElement('button');
      stageBtn.className = 'qa-btn' + (qaResponses[q.id]?.staged ? ' staged' : '');
      stageBtn.textContent = qaResponses[q.id]?.staged ? 'Staged' : 'Stage for Bible';
      stageBtn.onclick = () => stageQA(q.id);
      actions.appendChild(stageBtn);
      card.appendChild(actions);

      groupDiv.appendChild(card);
    });

    container.appendChild(groupDiv);
  });

  renderStageQueue();
}

async function saveQAResponse(qid, answer) {
  qaResponses[qid] = { ...(qaResponses[qid] || {}), answer, timestamp: new Date().toISOString() };
  await postJSON('/qa', { questionId: qid, answer, timestamp: qaResponses[qid].timestamp });
  flashSave();
  // Update progress
  let answered = 0;
  questionMap.questions.forEach(q => { if (qaResponses[q.id]?.answer) answered++; });
  const total = questionMap.questions.length;
  document.getElementById('qaProgressFill').style.width = (answered/total*100) + '%';
  document.getElementById('qaProgressText').textContent = answered + ' / ' + total + ' answered';
  document.getElementById('qaCount').textContent = answered + '/' + total;
}

async function stageQA(qid) {
  qaResponses[qid] = { ...(qaResponses[qid] || {}), staged: true };
  await postJSON('/qa/stage', { questionId: qid });
  flashSave();
  renderQA();
}

async function approveStaged(qid) {
  const result = await postJSON('/qa/approve', { questionId: qid });
  if (result.ok) {
    qaResponses[qid].staged = false;
    qaResponses[qid].applied = true;
    flashSave();
    renderQA();
  }
}

function unstageQA(qid) {
  qaResponses[qid].staged = false;
  postJSON('/qa', { questionId: qid, answer: qaResponses[qid].answer, timestamp: qaResponses[qid].timestamp, staged: false });
  renderQA();
}

function renderStageQueue() {
  const container = document.getElementById('stageItems');
  const queue = document.getElementById('stageQueue');
  container.textContent = '';
  const staged = Object.entries(qaResponses).filter(([k,v]) => v.staged && v.answer);
  queue.classList.toggle('has-items', staged.length > 0);

  staged.forEach(([qid, data]) => {
    const q = questionMap.questions.find(q => q.id === qid);
    const item = document.createElement('div');
    item.className = 'stage-item';

    const text = document.createElement('span');
    text.className = 'stage-item-text';
    text.textContent = (q ? q.text : qid) + ': ' + data.answer.slice(0, 80) + (data.answer.length > 80 ? '...' : '');
    item.appendChild(text);

    const approveBtn = document.createElement('button');
    approveBtn.className = 'qa-btn';
    approveBtn.textContent = 'Approve';
    approveBtn.style.borderColor = 'var(--green)';
    approveBtn.style.color = 'var(--green)';
    approveBtn.onclick = () => approveStaged(qid);
    item.appendChild(approveBtn);

    const editBtn = document.createElement('button');
    editBtn.className = 'qa-btn';
    editBtn.textContent = 'Edit';
    editBtn.onclick = () => unstageQA(qid);
    item.appendChild(editBtn);

    container.appendChild(item);
  });
}

// ── Feedback Rendering ───────────────────────────────────────────────────

function renderFeedback() {
  const container = document.getElementById('fbItems');
  container.textContent = '';

  const activeFilter = document.querySelector('[data-fbfilter].active')?.dataset.fbfilter || 'all';

  const counts = { proposed: 0, reviewed: 0, accepted: 0 };
  feedbackItems.forEach(fb => { if (counts[fb.status] !== undefined) counts[fb.status]++; });
  document.getElementById('fbCount').textContent = feedbackItems.length ? feedbackItems.length.toString() : '';

  const filtered = feedbackItems.filter(fb => activeFilter === 'all' || fb.status === activeFilter);

  // Reverse chronological
  [...filtered].reverse().forEach(fb => {
    const item = document.createElement('div');
    item.className = 'fb-item';

    const header = document.createElement('div');
    header.className = 'fb-item-header';
    const secSpan = document.createElement('span');
    secSpan.className = 'fb-item-section';
    const secInfo = questionMap?.section_keywords[fb.section];
    secSpan.textContent = secInfo ? secInfo.title : fb.section;
    header.appendChild(secSpan);
    const titleSpan = document.createElement('span');
    titleSpan.className = 'fb-item-title';
    titleSpan.textContent = fb.title;
    header.appendChild(titleSpan);
    const status = document.createElement('span');
    status.className = 'fb-status fb-' + fb.status;
    status.textContent = fb.status;
    header.appendChild(status);
    item.appendChild(header);

    const body = document.createElement('div');
    body.className = 'fb-item-body';
    body.textContent = fb.content;
    item.appendChild(body);

    const date = document.createElement('div');
    date.className = 'fb-item-date';
    date.textContent = fb.timestamp;
    item.appendChild(date);

    if (fb.status !== 'accepted') {
      const actions = document.createElement('div');
      actions.className = 'fb-item-actions';

      if (fb.status === 'proposed') {
        const reviewBtn = document.createElement('button');
        reviewBtn.className = 'qa-btn';
        reviewBtn.textContent = 'Mark Reviewed';
        reviewBtn.onclick = () => updateFeedbackStatus(fb.id, 'reviewed');
        actions.appendChild(reviewBtn);
      }
      if (fb.status === 'reviewed') {
        const acceptBtn = document.createElement('button');
        acceptBtn.className = 'qa-btn';
        acceptBtn.textContent = 'Accept & Apply';
        acceptBtn.style.borderColor = 'var(--green)';
        acceptBtn.style.color = 'var(--green)';
        acceptBtn.onclick = () => updateFeedbackStatus(fb.id, 'accepted');
        actions.appendChild(acceptBtn);
        const rejectBtn = document.createElement('button');
        rejectBtn.className = 'qa-btn';
        rejectBtn.textContent = 'Back to Proposed';
        rejectBtn.onclick = () => updateFeedbackStatus(fb.id, 'proposed');
        actions.appendChild(rejectBtn);
      }
      item.appendChild(actions);
    }

    container.appendChild(item);
  });
}

async function submitFeedback() {
  const section = document.getElementById('fbSection').value;
  const title = document.getElementById('fbTitle').value.trim();
  const content = document.getElementById('fbContent').value.trim();
  if (!title || !content) return;

  const fb = {
    id: 'fb-' + Date.now(),
    section,
    title,
    content,
    status: 'proposed',
    timestamp: new Date().toISOString()
  };

  feedbackItems.push(fb);
  await postJSON('/feedback', { action: 'add', item: fb });
  flashSave();

  document.getElementById('fbTitle').value = '';
  document.getElementById('fbContent').value = '';
  renderFeedback();
}

async function updateFeedbackStatus(id, newStatus) {
  const fb = feedbackItems.find(f => f.id === id);
  if (!fb) return;
  fb.status = newStatus;
  const result = await postJSON('/feedback', { action: 'update', id, status: newStatus });
  if (result.ok) flashSave();
  renderFeedback();
}

// ── Ideas Rendering ──────────────────────────────────────────────────────

function organizeIdeas() {
  const raw = document.getElementById('ideasText').value.trim();
  if (!raw) return;

  const paragraphs = raw.split(/\n\s*\n/).map(p => p.trim()).filter(p => p);
  const keywords = questionMap.section_keywords;
  const sectionIds = Object.keys(keywords);

  organizedIdeas = paragraphs.map(text => {
    const tokens = text.toLowerCase().replace(/[^a-z0-9\s]/g, ' ').split(/\s+/);
    const scores = {};
    sectionIds.forEach(sid => {
      const kws = keywords[sid].keywords;
      let score = 0;
      kws.forEach(kw => { if (tokens.some(t => t.includes(kw) || kw.includes(t))) score++; });
      if (score > 0) scores[sid] = score;
    });

    const sorted = Object.entries(scores).sort((a,b) => b[1] - a[1]);
    const bestMatch = sorted[0] && sorted[0][1] >= 2 ? sorted[0][0] : null;
    const suggestions = sorted.slice(0, 3).map(s => s[0]);

    return { text, section: bestMatch, suggestions, applied: false };
  });

  renderOrganizedIdeas();
  document.getElementById('ideasSaveBtn').style.display = '';
}

function renderOrganizedIdeas() {
  const container = document.getElementById('ideasResults');
  container.textContent = '';

  // Group by section
  const groups = {};
  const uncategorized = [];
  organizedIdeas.forEach((idea, idx) => {
    idea._idx = idx;
    if (idea.section) {
      if (!groups[idea.section]) groups[idea.section] = [];
      groups[idea.section].push(idea);
    } else {
      uncategorized.push(idea);
    }
  });

  Object.entries(groups).forEach(([secId, ideas]) => {
    const secInfo = questionMap.section_keywords[secId];
    const groupDiv = document.createElement('div');
    groupDiv.className = 'ideas-group';
    const title = document.createElement('div');
    title.className = 'ideas-group-title';
    title.textContent = secInfo ? secInfo.title : secId;
    groupDiv.appendChild(title);

    ideas.forEach(idea => groupDiv.appendChild(makeIdeaCard(idea)));
    container.appendChild(groupDiv);
  });

  if (uncategorized.length > 0) {
    const groupDiv = document.createElement('div');
    groupDiv.className = 'ideas-group';
    const title = document.createElement('div');
    title.className = 'ideas-group-title';
    title.textContent = 'Uncategorized';
    title.style.color = 'var(--text-dim)';
    groupDiv.appendChild(title);

    uncategorized.forEach(idea => groupDiv.appendChild(makeIdeaCard(idea, true)));
    container.appendChild(groupDiv);
  }
}

function makeIdeaCard(idea, showSuggestions) {
  const card = document.createElement('div');
  card.className = 'idea-card';
  const text = document.createElement('div');
  text.className = 'idea-card-text';
  text.textContent = idea.text;
  card.appendChild(text);

  const actions = document.createElement('div');
  actions.className = 'idea-card-actions';

  // Reassign dropdown
  const sel = document.createElement('select');
  const optNone = document.createElement('option');
  optNone.value = '';
  optNone.textContent = '-- Reassign --';
  sel.appendChild(optNone);
  Object.entries(questionMap.section_keywords).forEach(([sid, info]) => {
    const opt = document.createElement('option');
    opt.value = sid;
    opt.textContent = info.title;
    if (sid === idea.section) opt.selected = true;
    sel.appendChild(opt);
  });
  sel.onchange = () => {
    organizedIdeas[idea._idx].section = sel.value || null;
    renderOrganizedIdeas();
  };
  actions.appendChild(sel);

  // Suggestions for uncategorized
  if (showSuggestions && idea.suggestions && idea.suggestions.length > 0) {
    const sugDiv = document.createElement('div');
    sugDiv.className = 'idea-suggestions';
    idea.suggestions.forEach(sid => {
      const pill = document.createElement('button');
      pill.className = 'idea-suggest-pill';
      const info = questionMap.section_keywords[sid];
      pill.textContent = info ? info.title : sid;
      pill.onclick = () => {
        organizedIdeas[idea._idx].section = sid;
        renderOrganizedIdeas();
      };
      sugDiv.appendChild(pill);
    });
    actions.appendChild(sugDiv);
  }

  card.appendChild(actions);
  return card;
}

async function saveIdeas() {
  const raw = document.getElementById('ideasText').value.trim();
  const result = await postJSON('/ideas', {
    raw,
    organized: organizedIdeas.map(i => ({ text: i.text, section: i.section, applied: i.applied })),
    timestamp: new Date().toISOString()
  });
  if (result.ok) {
    flashSave();
    document.getElementById('ideasText').value = '';
    document.getElementById('ideasSaveBtn').style.display = 'none';
    organizedIdeas = [];
    document.getElementById('ideasResults').textContent = '';
    loadIdeasHistory();
  }
}

async function applyIdea(submissionIdx, ideaIdx) {
  const result = await postJSON('/ideas/apply', { submissionIdx, ideaIdx });
  if (result.ok) {
    flashSave();
    loadIdeasHistory();
  }
}

async function loadIdeasHistory() {
  const data = await fetch('/ideas').then(r => r.json());
  ideasData = data;
  const container = document.getElementById('ideasHistory');
  container.textContent = '';
  if (!data.submissions || data.submissions.length === 0) return;

  const title = document.createElement('div');
  title.className = 'ideas-history-title';
  title.textContent = '\u25bc Previous Submissions (' + data.submissions.length + ')';
  container.appendChild(title);

  data.submissions.slice().reverse().forEach((sub, si) => {
    const realIdx = data.submissions.length - 1 - si;
    const groupDiv = document.createElement('div');
    groupDiv.className = 'ideas-group';
    const subTitle = document.createElement('div');
    subTitle.className = 'ideas-group-title';
    subTitle.textContent = sub.timestamp;
    subTitle.style.fontSize = '12px';
    groupDiv.appendChild(subTitle);

    (sub.organized || []).forEach((idea, ii) => {
      const card = document.createElement('div');
      card.className = 'idea-card';
      const text = document.createElement('div');
      text.className = 'idea-card-text';
      text.textContent = idea.text;
      card.appendChild(text);
      const secInfo = questionMap?.section_keywords[idea.section];
      if (secInfo) {
        const sec = document.createElement('div');
        sec.style.cssText = 'font-size:11px;color:var(--gold-dim);';
        sec.textContent = '\u2192 ' + secInfo.title;
        card.appendChild(sec);
      }
      if (!idea.applied) {
        const btn = document.createElement('button');
        btn.className = 'qa-btn';
        btn.textContent = 'Apply to Bible';
        btn.style.marginTop = '6px';
        btn.onclick = () => applyIdea(realIdx, ii);
        card.appendChild(btn);
      } else {
        const applied = document.createElement('div');
        applied.style.cssText = 'font-size:11px;color:var(--green);margin-top:4px;';
        applied.textContent = '\u2713 Applied';
        card.appendChild(applied);
      }
      groupDiv.appendChild(card);
    });

    container.appendChild(groupDiv);
  });
}

// ── Init ─────────────────────────────────────────────────────────────────

async function init() {
  // Load all data in parallel
  const [mdRes, qmRes, qaRes, fbRes, idRes] = await Promise.all([
    fetch('/content'),
    fetch('/questions'),
    fetch('/qa'),
    fetch('/feedback'),
    fetch('/ideas')
  ]);

  const md = await mdRes.text();
  questionMap = await qmRes.json();
  qaResponses = (await qaRes.json()) || {};
  feedbackItems = ((await fbRes.json()) || {}).items || [];
  ideasData = (await idRes.json()) || { submissions: [] };

  // Bible view
  const sections = parseMarkdown(md);
  const mainEl = document.getElementById('view-bible');
  const sbEl   = document.getElementById('sbItems');

  sections.forEach(sec => {
    sec.nodes.forEach(n => mainEl.appendChild(n));
    if (sec.id === 'title') return;

    const a   = document.createElement('a');
    a.className = 'sb-item';
    a.href      = '#' + sec.id;
    a.dataset.id = sec.id;

    const numSpan = document.createElement('span');
    numSpan.className = 'sb-num';
    numSpan.textContent = sec.num ? sec.num + '.' : '';
    a.appendChild(numSpan);
    a.appendChild(document.createTextNode(sec.title));

    a.addEventListener('click', e => {
      e.preventDefault();
      const el = document.getElementById(sec.id);
      if (el) {
        pushNavState(sec.id);
        el.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
      closeMobileMenu();
    });
    sbEl.appendChild(a);
  });

  // Build section ID list for prev/next nav
  sectionIds = sections.filter(s => s.id !== 'title').map(s => s.id);

  const count = sections.filter(s => s.id !== 'title').length;
  document.getElementById('sectionCount').textContent = count + ' sections';

  // Build quick-jump dropdown
  buildQuickJump(sections);

  // Scroll spy
  const observer = new IntersectionObserver(entries => {
    entries.forEach(e => { if (e.isIntersecting) setActive(e.target.id); });
  }, { rootMargin: '-15% 0px -75% 0px' });
  document.querySelectorAll('h2.s-head').forEach(h => observer.observe(h));

  // Search
  const inp = document.getElementById('search');
  const clr = document.getElementById('searchClear');
  const noR = document.getElementById('noResults');
  inp.addEventListener('input', () => {
    const q = inp.value.trim().toLowerCase();
    clr.classList.toggle('on', q.length > 0);
    let any = false;
    document.querySelectorAll('.sb-item').forEach(item => {
      const match = !q || item.textContent.toLowerCase().includes(q);
      item.classList.toggle('hidden', !match);
      if (match) any = true;
    });
    noR.style.display = (!any && q) ? 'block' : 'none';
  });
  clr.addEventListener('click', () => {
    inp.value = '';
    clr.classList.remove('on');
    document.querySelectorAll('.sb-item').forEach(i => i.classList.remove('hidden'));
    noR.style.display = 'none';
    inp.focus();
  });

  // Scroll-to-top + reading progress + prev/next visibility
  const toTop = document.getElementById('toTop');
  window.addEventListener('scroll', () => {
    toTop.classList.toggle('on', window.scrollY > 400);
    updateReadingProgress();
    if (activeTab === 'bible' && activeId) {
      document.getElementById('navPrevNext').classList.toggle('on', window.scrollY > 200);
    }
  });

  // Tab events
  document.querySelectorAll('.tab-btn').forEach(btn => {
    btn.addEventListener('click', () => switchTab(btn.dataset.tab));
  });

  // Q&A filter events
  document.querySelectorAll('.qa-filter-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('.qa-filter-btn').forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      renderQA();
    });
  });

  // Feedback filter events
  document.querySelectorAll('[data-fbfilter]').forEach(btn => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('[data-fbfilter]').forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      renderFeedback();
    });
  });

  // Populate feedback section dropdown
  const fbSelect = document.getElementById('fbSection');
  Object.entries(questionMap.section_keywords).forEach(([sid, info]) => {
    const opt = document.createElement('option');
    opt.value = sid;
    opt.textContent = info.title;
    fbSelect.appendChild(opt);
  });

  // Render all tabs
  renderQA();
  renderFeedback();
  loadIdeasHistory();

  // Handle URL hash
  const hash = location.hash.slice(1);
  if (['qa', 'feedback', 'ideas'].includes(hash)) switchTab(hash);

  // Keyboard shortcuts
  document.addEventListener('keydown', e => {
    // Escape always works (close mobile menu, close quick-jump)
    if (e.key === 'Escape') {
      closeMobileMenu();
      closeQuickJump();
      return;
    }
    // / focuses search from anywhere except inputs
    if (e.key === '/' && e.target.tagName !== 'TEXTAREA' && e.target.tagName !== 'INPUT' && e.target.tagName !== 'SELECT') {
      e.preventDefault();
      document.getElementById('search').focus();
      return;
    }
    if (e.target.tagName === 'TEXTAREA' || e.target.tagName === 'INPUT' || e.target.tagName === 'SELECT') return;
    if (e.key === '1') switchTab('bible');
    if (e.key === '2') switchTab('qa');
    if (e.key === '3') switchTab('feedback');
    if (e.key === '4') switchTab('ideas');
    if (e.key === 'ArrowLeft' && activeTab === 'bible') { e.preventDefault(); navigatePrev(); }
    if (e.key === 'ArrowRight' && activeTab === 'bible') { e.preventDefault(); navigateNext(); }
    if (e.key === 'Home' && activeTab === 'bible') { e.preventDefault(); window.scrollTo({ top: 0, behavior: 'smooth' }); }
  });

  // Mobile hamburger + backdrop
  document.getElementById('hamburgerBtn').addEventListener('click', toggleMobileMenu);
  document.getElementById('sidebarBackdrop').addEventListener('click', closeMobileMenu);

  // Section indicator click -> toggle quick-jump
  document.getElementById('sectionIndicator').addEventListener('click', toggleQuickJump);

  // Close quick-jump when clicking outside
  document.addEventListener('click', e => {
    const qj = document.getElementById('quickJump');
    const indicator = document.getElementById('sectionIndicator');
    if (qj.classList.contains('open') && !qj.contains(e.target) && !indicator.contains(e.target)) {
      closeQuickJump();
    }
  });

  // Prev/Next button handlers
  document.getElementById('navPrev').addEventListener('click', navigatePrev);
  document.getElementById('navNext').addEventListener('click', navigateNext);

  // Popstate handler for back/forward
  window.addEventListener('popstate', handlePopState);

  document.getElementById('loading').style.display = 'none';
}

function setActive(id) {
  if (activeId === id) return;
  activeId = id;
  document.querySelectorAll('.sb-item').forEach(item => {
    const on = item.dataset.id === id;
    item.classList.toggle('active', on);
    if (on) item.scrollIntoView({ block: 'nearest' });
  });
  updateSectionIndicator(id);
  updatePrevNext(id);
  updateQuickJumpActive(id);
}

// ── Section Indicator / Breadcrumb ──────────────────────────────────────

function updateSectionIndicator(id) {
  const indicator = document.getElementById('sectionIndicator');
  const textEl = document.getElementById('indicatorText');
  if (!id || activeTab !== 'bible') {
    indicator.classList.remove('on');
    return;
  }
  const items = Array.from(document.querySelectorAll('.sb-item'));
  const match = items.find(i => i.dataset.id === id);
  if (match) {
    textEl.textContent = match.textContent.trim();
    indicator.classList.add('on');
  }
}

function buildQuickJump(sections) {
  const container = document.getElementById('quickJump');
  container.textContent = '';
  sections.forEach(sec => {
    if (sec.id === 'title') return;
    const item = document.createElement('div');
    item.className = 'qj-item';
    item.dataset.id = sec.id;
    const numSpan = document.createElement('span');
    numSpan.className = 'qj-num';
    numSpan.textContent = sec.num ? sec.num + '.' : '';
    item.appendChild(numSpan);
    item.appendChild(document.createTextNode(sec.title));
    item.addEventListener('click', () => {
      const el = document.getElementById(sec.id);
      if (el) {
        pushNavState(sec.id);
        el.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
      closeQuickJump();
    });
    container.appendChild(item);
  });
}

function toggleQuickJump() {
  const qj = document.getElementById('quickJump');
  qj.classList.toggle('open');
}

function closeQuickJump() {
  document.getElementById('quickJump').classList.remove('open');
}

function updateQuickJumpActive(id) {
  document.querySelectorAll('#quickJump .qj-item').forEach(item => {
    item.classList.toggle('active', item.dataset.id === id);
  });
}

// ── Prev/Next Navigation ────────────────────────────────────────────────

let sectionIds = [];

function updatePrevNext(id) {
  const navPN = document.getElementById('navPrevNext');
  const prevBtn = document.getElementById('navPrev');
  const nextBtn = document.getElementById('navNext');
  const prevLabel = document.getElementById('navPrevLabel');
  const nextLabel = document.getElementById('navNextLabel');

  if (!id || activeTab !== 'bible' || sectionIds.length === 0) {
    navPN.classList.remove('on');
    return;
  }

  const idx = sectionIds.indexOf(id);
  if (idx === -1) { navPN.classList.remove('on'); return; }

  navPN.classList.toggle('on', window.scrollY > 200);

  const prevSec = idx > 0 ? sectionIds[idx - 1] : null;
  const nextSec = idx < sectionIds.length - 1 ? sectionIds[idx + 1] : null;

  prevBtn.disabled = !prevSec;
  nextBtn.disabled = !nextSec;

  const items = Array.from(document.querySelectorAll('.sb-item'));
  const prevItem = prevSec ? items.find(i => i.dataset.id === prevSec) : null;
  const nextItem = nextSec ? items.find(i => i.dataset.id === nextSec) : null;

  prevLabel.textContent = prevItem ? prevItem.textContent.trim() : 'Prev';
  nextLabel.textContent = nextItem ? nextItem.textContent.trim() : 'Next';
  prevBtn.title = prevItem ? prevItem.textContent.trim() : '';
  nextBtn.title = nextItem ? nextItem.textContent.trim() : '';
}

function navigatePrev() {
  if (!activeId || sectionIds.length === 0) return;
  const idx = sectionIds.indexOf(activeId);
  if (idx > 0) navigateToSection(sectionIds[idx - 1]);
}

function navigateNext() {
  if (!activeId || sectionIds.length === 0) return;
  const idx = sectionIds.indexOf(activeId);
  if (idx < sectionIds.length - 1) navigateToSection(sectionIds[idx + 1]);
}

function navigateToSection(id) {
  const el = document.getElementById(id);
  if (el) {
    pushNavState(id);
    el.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
}

// ── History State for Section Navigation ────────────────────────────────

let suppressPopState = false;

function pushNavState(sectionId) {
  if (sectionId && activeTab === 'bible') {
    suppressPopState = true;
    history.pushState({ section: sectionId }, '', '#' + sectionId);
    setTimeout(() => { suppressPopState = false; }, 100);
  }
}

function handlePopState(e) {
  if (suppressPopState) return;
  if (e.state && e.state.section) {
    const el = document.getElementById(e.state.section);
    if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
  } else {
    const hash = location.hash.slice(1);
    if (['qa', 'feedback', 'ideas'].includes(hash)) {
      switchTab(hash);
    } else if (hash) {
      const el = document.getElementById(hash);
      if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
}

// ── Reading Progress ────────────────────────────────────────────────────

function updateReadingProgress() {
  const fill = document.getElementById('readingProgressFill');
  const scrollTop = window.scrollY;
  const docHeight = document.documentElement.scrollHeight - window.innerHeight;
  const pct = docHeight > 0 ? Math.min(100, (scrollTop / docHeight) * 100) : 0;
  fill.style.width = pct + '%';
}

init().catch(err => {
  const el  = document.getElementById('loading');
  const msg = document.createElement('div');
  msg.className = 'load-sub';
  msg.textContent = err.message;
  el.querySelector('.load-title').textContent = 'Load Error';
  el.querySelector('.load-title').style.color = '#c05050';
  el.appendChild(msg);
});
</script>
</body>
</html>
"""


# ── Server ────────────────────────────────────────────────────────────────

class Handler(http.server.BaseHTTPRequestHandler):

    def _send_json(self, data, status=200):
        body = json.dumps(data, ensure_ascii=False).encode('utf-8')
        self.send_response(status)
        self.send_header('Content-Type', 'application/json; charset=utf-8')
        self.send_header('Content-Length', str(len(body)))
        self.end_headers()
        self.wfile.write(body)

    def _send_text(self, text, content_type='text/plain'):
        body = text.encode('utf-8')
        self.send_response(200)
        self.send_header('Content-Type', content_type + '; charset=utf-8')
        self.send_header('Content-Length', str(len(body)))
        self.end_headers()
        self.wfile.write(body)

    def _read_body(self):
        length = int(self.headers.get('Content-Length', 0))
        return json.loads(self.rfile.read(length)) if length else {}

    def do_GET(self):
        path = self.path.split('?')[0]

        if path in ('/', '/index.html'):
            self._send_text(HTML, 'text/html')

        elif path == '/content':
            fpath = os.path.join(DIR, 'BLOODLINES_COMPLETE_DESIGN_BIBLE.md')
            with open(fpath, 'r', encoding='utf-8') as f:
                self._send_text(f.read())

        elif path == '/questions':
            data = load_json('question_map.json', {})
            self._send_json(data)

        elif path == '/qa':
            data = load_json('qa_responses.json', {})
            self._send_json(data)

        elif path == '/feedback':
            data = load_json('feedback.json', {"items": []})
            self._send_json(data)

        elif path == '/ideas':
            data = load_json('ideas_inbox.json', {"submissions": []})
            self._send_json(data)

        else:
            self.send_response(404)
            self.end_headers()

    def do_POST(self):
        path = self.path.split('?')[0]
        body = self._read_body()

        if path == '/qa':
            data = load_json('qa_responses.json', {})
            qid = body.get('questionId', '')
            data[qid] = {
                'answer': body.get('answer', ''),
                'timestamp': body.get('timestamp', ''),
                'staged': body.get('staged', data.get(qid, {}).get('staged', False)),
                'applied': data.get(qid, {}).get('applied', False)
            }
            save_json('qa_responses.json', data)
            self._send_json({"ok": True})

        elif path == '/qa/stage':
            data = load_json('qa_responses.json', {})
            qid = body.get('questionId', '')
            if qid in data:
                data[qid]['staged'] = True
                save_json('qa_responses.json', data)
            self._send_json({"ok": True})

        elif path == '/qa/approve':
            data = load_json('qa_responses.json', {})
            qid = body.get('questionId', '')
            if qid in data and data[qid].get('answer'):
                # Find the question to get section info
                qmap = load_json('question_map.json', {})
                question = None
                for q in qmap.get('questions', []):
                    if q['id'] == qid:
                        question = q
                        break
                if question:
                    sec_id = question['bible_sections'][0] if question.get('bible_sections') else ''
                    sec_num = sec_id.replace('section-', '') if sec_id.startswith('section-') else ''
                    if sec_num:
                        content = f"**{question['text']}**\n\n{data[qid]['answer']}"
                        append_to_bible(sec_num, content, 'Q&A Response')
                data[qid]['staged'] = False
                data[qid]['applied'] = True
                save_json('qa_responses.json', data)
            self._send_json({"ok": True})

        elif path == '/feedback':
            action = body.get('action')
            fb_data = load_json('feedback.json', {"items": []})

            if action == 'add':
                fb_data['items'].append(body.get('item', {}))
                save_json('feedback.json', fb_data)

            elif action == 'update':
                fb_id = body.get('id')
                new_status = body.get('status')
                for item in fb_data['items']:
                    if item.get('id') == fb_id:
                        item['status'] = new_status
                        # If accepted, apply to bible
                        if new_status == 'accepted':
                            sec_id = item.get('section', '')
                            sec_num = sec_id.replace('section-', '') if sec_id.startswith('section-') else ''
                            if sec_num:
                                content = f"**{item.get('title', 'Feedback')}**\n\n{item.get('content', '')}"
                                append_to_bible(sec_num, content, 'Feedback Accepted')
                        break
                save_json('feedback.json', fb_data)

            self._send_json({"ok": True})

        elif path == '/ideas':
            data = load_json('ideas_inbox.json', {"submissions": []})
            data['submissions'].append({
                'raw': body.get('raw', ''),
                'organized': body.get('organized', []),
                'timestamp': body.get('timestamp', '')
            })
            save_json('ideas_inbox.json', data)
            self._send_json({"ok": True})

        elif path == '/ideas/apply':
            data = load_json('ideas_inbox.json', {"submissions": []})
            si = body.get('submissionIdx', 0)
            ii = body.get('ideaIdx', 0)
            if si < len(data['submissions']):
                sub = data['submissions'][si]
                if ii < len(sub.get('organized', [])):
                    idea = sub['organized'][ii]
                    sec_id = idea.get('section', '')
                    sec_num = sec_id.replace('section-', '') if sec_id.startswith('section-') else ''
                    if sec_num:
                        append_to_bible(sec_num, idea.get('text', ''), 'Ideas Inbox')
                    idea['applied'] = True
                    save_json('ideas_inbox.json', data)
            self._send_json({"ok": True})

        else:
            self.send_response(404)
            self.end_headers()

    def log_message(self, fmt, *args):
        pass  # suppress access logs


if __name__ == '__main__':
    with socketserver.TCPServer(('', PORT), Handler) as httpd:
        print('Bloodlines Design Bible — Interactive Viewer')
        print(f'  http://localhost:{PORT}')
        print(f'  Tabs: Bible | Q&A | Feedback | Ideas')
        print('Press Ctrl+C to stop.')
        httpd.serve_forever()
