# Web Editor PHP Relay Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Enable the Bloodlines web bible viewer to push organized ideas directly to the deployed markdown files on Hostinger via a PHP endpoint, with a changelog and revert capability.

**Architecture:** A single PHP file (`api/submit.php`) receives authenticated POST requests from the viewer JS. It appends content to markdown files, maintains an `_ideas_inbox.md` staging document, and writes a `_changelog.json` for tracking/reverting. The viewer JS replaces the localStorage-only workflow with live server calls, falling back to localStorage when offline.

**Tech Stack:** PHP 8.3 (Hostinger), vanilla JS (viewer), JSON (changelog), Markdown (content files)

---

### Task 1: Create PHP Backend (`api/submit.php`)

**Files:**
- Create: `15_PROTOTYPE/api/submit.php`
- Create: `15_PROTOTYPE/api/.htaccess`

**Step 1: Create the .htaccess for CORS and PHP routing**

```apache
# Allow CORS from the viewer origin
<IfModule mod_headers.c>
    Header set Access-Control-Allow-Origin "*"
    Header set Access-Control-Allow-Methods "POST, GET, OPTIONS"
    Header set Access-Control-Allow-Headers "Content-Type"
</IfModule>

# Handle preflight
RewriteEngine On
RewriteCond %{REQUEST_METHOD} OPTIONS
RewriteRule ^(.*)$ $1 [R=200,L]
```

**Step 2: Create submit.php with auth validation and action routing**

The PHP script must:
1. Read JSON body from `php://input`
2. Validate that `auth` field matches the fisher13 SHA-256 hash: `59195c6c541c8307f1da2d1e768d6f2280c984df217ad5f4c64c3542b04111a4`
3. Route to `apply`, `changelog`, or `revert` based on `action` field
4. Return JSON responses with appropriate HTTP status codes

**Section-to-file mapping (hardcoded in PHP):**

```php
$SECTION_FILE_MAP = [
    'section-1' => '01_CANON/BLOODLINES_MASTER_MEMORY.md',
    'section-2' => '01_CANON/BLOODLINES_MASTER_MEMORY.md',
    'section-3' => '05_LORE/TIMELINE.md',
    'section-4' => '01_CANON/BLOODLINES_MASTER_MEMORY.md',
    'section-5' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-6' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-7' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-8' => '09_WORLD/WORLD_INDEX.md',
    'section-9' => '09_WORLD/WORLD_INDEX.md',
    'section-10' => '04_SYSTEMS/DYNASTIC_SYSTEM.md',
    'section-11' => '04_SYSTEMS/DYNASTIC_SYSTEM.md',
    'section-12' => '04_SYSTEMS/DYNASTIC_SYSTEM.md',
    'section-13' => '04_SYSTEMS/DYNASTIC_SYSTEM.md',
    'section-14' => '04_SYSTEMS/DYNASTIC_SYSTEM.md',
    'section-15' => '04_SYSTEMS/DYNASTIC_SYSTEM.md',
    'section-16' => '09_WORLD/WORLD_INDEX.md',
    'section-17' => '09_WORLD/WORLD_INDEX.md',
    'section-18' => '09_WORLD/WORLD_INDEX.md',
    'section-19' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-20' => '09_WORLD/WORLD_INDEX.md',
    'section-21' => '07_FAITHS/FOUR_ANCIENT_FAITHS.md',
    'section-22' => '07_FAITHS/FOUR_ANCIENT_FAITHS.md',
    'section-23' => '07_FAITHS/FOUR_ANCIENT_FAITHS.md',
    'section-24' => '07_FAITHS/FOUR_ANCIENT_FAITHS.md',
    'section-25' => '07_FAITHS/FOUR_ANCIENT_FAITHS.md',
    'section-26' => '07_FAITHS/FOUR_ANCIENT_FAITHS.md',
    'section-27' => '07_FAITHS/FOUR_ANCIENT_FAITHS.md',
    'section-28' => '04_SYSTEMS/CONVICTION_SYSTEM.md',
    'section-29' => '04_SYSTEMS/CONVICTION_SYSTEM.md',
    'section-30' => '04_SYSTEMS/CONVICTION_SYSTEM.md',
    'section-31' => '04_SYSTEMS/CONVICTION_SYSTEM.md',
    'section-32' => '10_UNITS/UNIT_INDEX.md',
    'section-33' => '10_UNITS/UNIT_INDEX.md',
    'section-34' => '10_UNITS/UNIT_INDEX.md',
    'section-35' => '04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md',
    'section-36' => '10_UNITS/UNIT_INDEX.md',
    'section-37' => '10_UNITS/UNIT_INDEX.md',
    'section-38' => '10_UNITS/UNIT_INDEX.md',
    'section-39' => '11_MATCHFLOW/MATCH_STRUCTURE.md',
    'section-40' => '04_SYSTEMS/RESOURCE_SYSTEM.md',
    'section-41' => '04_SYSTEMS/RESOURCE_SYSTEM.md',
    'section-42' => '04_SYSTEMS/RESOURCE_SYSTEM.md',
    'section-43' => '04_SYSTEMS/RESOURCE_SYSTEM.md',
    'section-44' => '04_SYSTEMS/RESOURCE_SYSTEM.md',
    'section-45' => '11_MATCHFLOW/MATCH_STRUCTURE.md',
    'section-46' => '11_MATCHFLOW/MATCH_STRUCTURE.md',
    'section-47' => '11_MATCHFLOW/POLITICAL_EVENTS.md',
    'section-48' => '11_MATCHFLOW/MATCH_STRUCTURE.md',
    'section-49' => '11_MATCHFLOW/MATCH_STRUCTURE.md',
    'section-50' => '11_MATCHFLOW/MATCH_STRUCTURE.md',
    'section-51' => '11_MATCHFLOW/MATCH_STRUCTURE.md',
    'section-52' => '11_MATCHFLOW/MATCH_STRUCTURE.md',
    'section-53' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-54' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-55' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-56' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-57' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-58' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-59' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-60' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-61' => '06_FACTIONS/FOUNDING_HOUSES.md',
    'section-63' => '08_MECHANICS/OPERATIONS_SYSTEM.md',
    'section-64' => '08_MECHANICS/OPERATIONS_SYSTEM.md',
    'section-65' => '08_MECHANICS/OPERATIONS_SYSTEM.md',
    'section-66' => '08_MECHANICS/OPERATIONS_SYSTEM.md',
    'section-67' => '08_MECHANICS/OPERATIONS_SYSTEM.md',
    'section-68' => '07_FAITHS/FOUR_ANCIENT_FAITHS.md',
    'section-69' => '01_CANON/BLOODLINES_MASTER_MEMORY.md',
];
```

**Apply action logic:**
1. Generate unique ID: `substr(bin2hex(random_bytes(6)), 0, 12)`
2. Build the markdown block with ID comment marker:
```
\n\n---\n\n<!-- idea:{id} -->\n> **Web Editor** | {date} | {sectionTitle}\n\n{text}\n
```
3. Append block to target markdown file (relative to `../` from `api/` dir)
4. Append to `_ideas_inbox.md` under the section heading
5. Add entry to `_changelog.json` array: `{id, timestamp, section, sectionTitle, file, text, reverted: false}`

**Revert action logic:**
1. Find entry in `_changelog.json` by ID
2. Read target markdown file, find block between `<!-- idea:{id} -->` and the next `---` or EOF
3. Remove that block (including the preceding `---` separator)
4. Write file back
5. Remove from `_ideas_inbox.md` (find by ID comment marker)
6. Mark `reverted: true` in changelog

**Step 3: Commit**

```bash
git add 15_PROTOTYPE/api/
git commit -m "Add PHP relay endpoint for web editor content submission"
```

---

### Task 2: Update Viewer JS -- Replace localStorage Workflow with Server Calls

**Files:**
- Modify: `15_PROTOTYPE/index_body.html` (JS section, lines ~400+)

**Step 1: Add API helper function and endpoint health check**

Add near the top of the editor JS section:
```javascript
var API_URL = 'api/submit.php';
var apiAvailable = false;

// Health check on editor open
function editorCheckApi() {
    fetch(API_URL + '?action=changelog', {
        method: 'POST',
        headers: {'Content-Type':'application/json'},
        body: JSON.stringify({auth: PASSHASHES[0], action:'changelog'})
    }).then(function(r){ apiAvailable = r.ok; }).catch(function(){ apiAvailable = false; });
}
```

**Step 2: Replace editorSave with editorApply**

New function `editorApply()`:
1. POST to `API_URL` with `{auth: PASSHASHES[0], action: 'apply', ideas: [...]}`
2. On success: clear textarea, show confirmation, refresh change log
3. On failure: fall back to localStorage save with a message explaining the server is unreachable

**Step 3: Replace editorExport/editorExportAll with changelog fetch**

New function `editorLoadChangelog()`:
1. POST to `API_URL` with `{auth: PASSHASHES[0], action: 'changelog'}`
2. Render entries in the Change Log tab
3. Each entry has a Revert button

New function `editorRevert(id)`:
1. POST to `API_URL` with `{auth: PASSHASHES[0], action: 'revert', id: id}`
2. On success: refresh the changelog display

**Step 4: Update button visibility logic**

After `editorOrganize()` runs:
- If `apiAvailable`: show "Apply to Bible" button (primary style)
- If not: show "Save to Browser" + "Export Markdown" buttons (existing fallback)

**Step 5: Commit**

```bash
git add 15_PROTOTYPE/index_body.html
git commit -m "Update viewer JS to use PHP relay for content submission"
```

---

### Task 3: Update Viewer HTML -- Change Log Tab

**Files:**
- Modify: `15_PROTOTYPE/index_body.html` (HTML section)

**Step 1: Replace three tabs with two**

Change the editor tabs from Write/Results/History to Write/Change Log:
```html
<button class="editor-tab active" data-tab="write" onclick="editorSwitchTab('write',this)">Write</button>
<button class="editor-tab" data-tab="changelog" onclick="editorSwitchTab('changelog',this)">Change Log</button>
```

Replace `editorTabResults` and `editorTabHistory` divs with a single `editorTabChangelog` div.

**Step 2: Update tab switching logic**

`editorSwitchTab` now handles `write` and `changelog` only. Switching to `changelog` calls `editorLoadChangelog()`.

**Step 3: Add Apply to Bible button alongside existing buttons**

In the editor actions div, add:
```html
<button class="editor-btn editor-btn-primary" id="editorApplyBtn" onclick="editorApply()" style="display:none">Apply to Bible</button>
```

**Step 4: Commit**

```bash
git add 15_PROTOTYPE/index_body.html
git commit -m "Simplify editor to Write + Change Log tabs with Apply button"
```

---

### Task 4: Add Editor CSS for Change Log Entries

**Files:**
- Modify: `15_PROTOTYPE/index_body.html` (CSS section)

**Step 1: Add changelog-specific styles**

```css
.changelog-entry { ... }  /* similar to editor-history-item */
.changelog-reverted { opacity: 0.5; }
.changelog-revert-btn { ... }  /* danger style, small */
.changelog-status { ... }  /* badge showing active/reverted */
```

Reuse existing editor CSS classes where possible. Remove unused `.editor-history-*` classes that are no longer needed.

**Step 2: Commit**

```bash
git add 15_PROTOTYPE/index_body.html
git commit -m "Add changelog CSS styles, remove unused history styles"
```

---

### Task 5: Build, Test Locally, Deploy

**Files:**
- Run: `15_PROTOTYPE/build_index.py`
- Output: `FisherSovereign/lancewfisher-v2/bloodlines/index.html`
- Copy: `15_PROTOTYPE/api/` to `FisherSovereign/lancewfisher-v2/bloodlines/api/`

**Step 1: Run build**

```bash
cd D:/ProjectsHome/Bloodlines && python 15_PROTOTYPE/build_index.py
```

Verify output file size increased and contains `editorApply`, `API_URL`, `editorLoadChangelog`.

**Step 2: Copy PHP files to deploy directory**

```bash
cp -r D:/ProjectsHome/Bloodlines/15_PROTOTYPE/api/ D:/ProjectsHome/FisherSovereign/lancewfisher-v2/bloodlines/api/
```

**Step 3: Verify offline fallback**

Open `index.html` directly in browser (file://). The editor should detect API unavailable and show localStorage fallback buttons.

**Step 4: Verify PHP locally (optional)**

If PHP is available locally:
```bash
cd D:/ProjectsHome/FisherSovereign/lancewfisher-v2/bloodlines && php -S localhost:8089
```
Test the full apply/changelog/revert flow.

**Step 5: Final commit**

```bash
cd D:/ProjectsHome/Bloodlines
git add 15_PROTOTYPE/ docs/
git commit -m "Complete web editor PHP relay: apply, changelog, revert"
```

---

### Task 6: Hostinger Deployment Notes

**Not automated -- manual steps for Lance:**

1. Upload `bloodlines/api/submit.php` and `bloodlines/api/.htaccess` to Hostinger via SFTP or File Manager
2. Ensure `bloodlines/` directory and all markdown subdirectories have write permissions (PHP needs to append)
3. Test from lancewfisher.com: open bible viewer, write a test idea, click Apply to Bible, verify it appears in the markdown file, then revert it

**Verification checklist:**
- [ ] Passkey gate works with fisher13
- [ ] Sidebar nav loads all documents
- [ ] Editor FAB appears after unlocking
- [ ] Organize categorizes ideas correctly
- [ ] Apply to Bible succeeds (check a markdown file on server)
- [ ] Change Log shows the applied entry
- [ ] Revert removes the content from the markdown file
- [ ] Offline fallback shows Save to Browser when API unreachable
