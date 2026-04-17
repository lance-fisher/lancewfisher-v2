#!/usr/bin/env python3
"""Build the Bloodlines web viewer index.html with inlined marked.js"""
import os

MARKED_PATH = 'D:/ProjectsHome/Bloodlines/15_PROTOTYPE/marked.min.js'
OUTPUT_PATH = 'D:/ProjectsHome/FisherSovereign/lancewfisher-v2/deploy/bloodlines/index.html'
PASSHASH = 'c65a82e6ec8047fa252b9ab14afe81ab92736c8c0947734fe7059f67189ad984'

with open(MARKED_PATH, 'r') as f:
    marked_js = f.read()

# Read the body template
BODY_PATH = 'D:/ProjectsHome/Bloodlines/15_PROTOTYPE/index_body.html'
with open(BODY_PATH, 'r', encoding='utf-8') as f:
    body_html = f.read()

# Assemble: head with inlined marked.js + body
head = '<!DOCTYPE html>\n<html lang="en">\n<head>\n'
head += '  <meta charset="UTF-8">\n'
head += '  <meta name="viewport" content="width=device-width, initial-scale=1.0">\n'
head += '  <title>Bloodlines \u2014 Design Archive</title>\n'
head += '  <meta name="robots" content="noindex, nofollow">\n'
head += '  <link rel="preconnect" href="https://fonts.googleapis.com">\n'
head += '  <link href="https://fonts.googleapis.com/css2?family=Cinzel:wght@400;600;700;900&family=Cinzel+Decorative:wght@700&family=Crimson+Pro:ital,wght@0,300;0,400;0,600;1,300;1,400&display=swap" rel="stylesheet">\n'
head += '  <script>\n'
head += marked_js
head += '\n  </script>\n'

full_html = head + body_html

with open(OUTPUT_PATH, 'w', encoding='utf-8') as f:
    f.write(full_html)

size = os.path.getsize(OUTPUT_PATH)
print(f"Built {OUTPUT_PATH}")
print(f"  marked.js inlined: {len(marked_js):,} chars")
print(f"  Total file size: {size:,} bytes")
