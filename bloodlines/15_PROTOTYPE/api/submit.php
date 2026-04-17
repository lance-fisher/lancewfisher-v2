<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit;
}

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
    http_response_code(405);
    echo json_encode(['error' => 'Method not allowed']);
    exit;
}

// ── Config ───────────────────────────────────────────────────────────────

$VALID_HASH = 'c65a82e6ec8047fa252b9ab14afe81ab92736c8c0947734fe7059f67189ad984';

// Data lives OUTSIDE the deploy folder so deploys never clobber it
// bloodlines/api/submit.php -> ../../bloodlines_data/
$DATA_DIR = realpath(__DIR__ . '/../..') . '/bloodlines_data/';
$CHANGELOG_FILE = $DATA_DIR . '_changelog.json';
$INBOX_FILE = $DATA_DIR . '_ideas_inbox.md';

// Ensure data directory exists
if (!is_dir($DATA_DIR)) {
    mkdir($DATA_DIR, 0755, true);
}

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

// ── Auth ─────────────────────────────────────────────────────────────────

$input = json_decode(file_get_contents('php://input'), true);
if (!$input || !isset($input['auth']) || $input['auth'] !== $VALID_HASH) {
    http_response_code(403);
    echo json_encode(['error' => 'Unauthorized']);
    exit;
}

$action = $input['action'] ?? '';

// ── Helpers ──────────────────────────────────────────────────────────────

function loadChangelog($file) {
    if (!file_exists($file)) return [];
    $data = json_decode(file_get_contents($file), true);
    return is_array($data) ? $data : [];
}

function saveChangelog($file, $entries) {
    file_put_contents($file, json_encode($entries, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE), LOCK_EX);
}

function generateId() {
    return bin2hex(random_bytes(6));
}

// ── Actions ──────────────────────────────────────────────────────────────

if ($action === 'apply') {
    $ideas = $input['ideas'] ?? [];
    if (empty($ideas)) {
        http_response_code(400);
        echo json_encode(['error' => 'No ideas provided']);
        exit;
    }

    $changelog = loadChangelog($CHANGELOG_FILE);
    $applied = [];
    $now = gmdate('Y-m-d H:i');

    foreach ($ideas as $idea) {
        $section = $idea['section'] ?? null;
        $sectionTitle = $idea['sectionTitle'] ?? 'Uncategorized';
        $text = trim($idea['text'] ?? '');
        if ($text === '') continue;

        $id = generateId();
        $targetFile = null;

        // Resolve target file path (for display/reference, not for writing)
        if ($section && isset($SECTION_FILE_MAP[$section])) {
            $targetFile = $SECTION_FILE_MAP[$section];
        }

        // Append to ideas inbox (in data dir)
        $inboxBlock = "<!-- idea:$id -->\n### $sectionTitle\n> *$now*\n\n$text\n\n";
        if (!file_exists($INBOX_FILE)) {
            file_put_contents($INBOX_FILE, "# Ideas Inbox\n\nContent submitted via the Bloodlines web editor. Each entry is tagged with its target bible section.\n\n---\n\n", LOCK_EX);
        }
        file_put_contents($INBOX_FILE, $inboxBlock, FILE_APPEND | LOCK_EX);

        // Add to changelog (in data dir)
        $entry = [
            'id' => $id,
            'timestamp' => $now,
            'section' => $section,
            'sectionTitle' => $sectionTitle,
            'file' => $targetFile,
            'text' => $text,
            'reverted' => false,
        ];
        array_unshift($changelog, $entry);
        $applied[] = $entry;
    }

    saveChangelog($CHANGELOG_FILE, $changelog);

    echo json_encode(['ok' => true, 'applied' => $applied, 'total' => count($applied)]);
    exit;
}

if ($action === 'changelog') {
    $changelog = loadChangelog($CHANGELOG_FILE);
    echo json_encode(['ok' => true, 'entries' => $changelog]);
    exit;
}

// Return active (non-reverted) ideas for a specific file path
if ($action === 'ideas_for_file') {
    $filePath = $input['file'] ?? '';
    if ($filePath === '') {
        http_response_code(400);
        echo json_encode(['error' => 'No file path provided']);
        exit;
    }

    $changelog = loadChangelog($CHANGELOG_FILE);
    $ideas = [];
    foreach ($changelog as $entry) {
        if (!$entry['reverted'] && $entry['file'] === $filePath) {
            $ideas[] = $entry;
        }
    }

    echo json_encode(['ok' => true, 'ideas' => $ideas]);
    exit;
}

if ($action === 'revert') {
    $id = $input['id'] ?? '';
    if ($id === '') {
        http_response_code(400);
        echo json_encode(['error' => 'No id provided']);
        exit;
    }

    $changelog = loadChangelog($CHANGELOG_FILE);
    $entry = null;
    $entryIdx = null;

    foreach ($changelog as $idx => $e) {
        if ($e['id'] === $id && !$e['reverted']) {
            $entry = $e;
            $entryIdx = $idx;
            break;
        }
    }

    if ($entry === null) {
        http_response_code(404);
        echo json_encode(['error' => 'Entry not found or already reverted']);
        exit;
    }

    // Remove from inbox
    if (file_exists($INBOX_FILE)) {
        $inbox = file_get_contents($INBOX_FILE);
        $pattern = '/<!-- idea:' . preg_quote($id, '/') . ' -->.*?\n\n/s';
        $newInbox = preg_replace($pattern, '', $inbox, 1);
        if ($newInbox !== $inbox) {
            file_put_contents($INBOX_FILE, $newInbox, LOCK_EX);
        }
    }

    // Mark reverted in changelog
    $changelog[$entryIdx]['reverted'] = true;
    $changelog[$entryIdx]['revertedAt'] = gmdate('Y-m-d H:i');
    saveChangelog($CHANGELOG_FILE, $changelog);

    echo json_encode(['ok' => true, 'reverted' => $id]);
    exit;
}

http_response_code(400);
echo json_encode(['error' => 'Unknown action: ' . $action]);
