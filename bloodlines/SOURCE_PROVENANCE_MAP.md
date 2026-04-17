# SOURCE_PROVENANCE_MAP

This map records where Bloodlines material came from and how it is preserved inside the canonical root.

## Canonical Entry Path

| Role | Path | Status |
|---|---|---|
| Canonical session root | `D:\ProjectsHome\Bloodlines` | Authoritative entry path for future sessions |
| Physical backing path | `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines` | Same content, not a separate root |

## Active Canon and Active Implementation

| Source path | Current in-root location | Relationship |
|---|---|---|
| `D:\ProjectsHome\Bloodlines` | root | Authoritative canonical root |
| `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines` | root | Physical backing path behind the canonical junction |

## Preserved Imported Sources

| Original source | In-root preserved location | Count | Bytes | Notes |
|---|---|---:|---:|---|
| `D:\ProjectsHome\_archive\2026-03-22\bloodlines game` | `archive_preserved_sources/2026-03-22_bloodlines-game-prompt-bundle/` | 3 | 159604 | Early canonical text bundle: prompt, bible, master prompt material |
| `D:\ProjectsHome\_archive\2026-04-13\bloodlines-single-root\external-repo-preconsolidation` | `archive_preserved_sources/2026-04-13_external-repo-preconsolidation/` | 410 | 9821573 | Prior pre-single-root snapshot already preserved outside the project |
| `D:\ProjectsHome\_archive\2026-04-13\bloodlines-single-root\repo-root-mirror-preconsolidation` | `archive_preserved_sources/2026-04-13_repo-root-mirror-preconsolidation/` | 42 | 676418 | Prior small mirror snapshot before earlier consolidation work |
| `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines` | `archive_preserved_sources/2026-04-14_deploy-bloodlines-compatibility-copy/` | 32905 | 2032763503 | Full preserved compatibility copy, retained because prior sessions treated it as canonical |
| `D:\ProjectsHome\frontier-bastion` | `archive_preserved_sources/2026-04-14_frontier-bastion-root/` | 2969 | 77498164 | Predecessor prototype root and exo carrier shell, superseded but still relevant |
| `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\preview-bloodlines-update.html` and Bloodlines thumb assets | `archive_preserved_sources/2026-04-14_lancewfisher-v2_bloodlines-web-surfaces/` | preserved | preserved | Bloodlines-specific presentation surfaces from the parent site |
| `D:\Lance\Downloads\Bloodlines_Master_Design_Doctrine.docx` and `D:\Lance\Downloads\Bloodlines_Master_Design_Doctrine2.docx` | `archive_preserved_sources/2026-04-14_downloads_bloodlines_design_doctrine_docx/` | 2 | 63030 | Externally supplied doctrine source bundle; byte-identical pair preserved separately |

## Imported Governance and Continuity Surfaces

| Original source | In-root location | Notes |
|---|---|---|
| `D:\ProjectsHome\.claude\rules\bloodlines.md` | `governance/imported_workspace_overlays/bloodlines.md` | Workspace Bloodlines rule overlay |
| `D:\ProjectsHome\.claude\agents\bloodlines-reviewer.md` | `governance/imported_review_agents/bloodlines-reviewer.md` | Historical reviewer-agent instruction surface |
| `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\CLAUDE.md` | `governance/imported_parent_repo_surfaces/CLAUDE.md` | Parent repo governance that shaped Bloodlines routing rules |
| `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\HANDOFF.md` | `governance/imported_parent_repo_surfaces/HANDOFF.md` | Parent repo handoff with Bloodlines-related operational context |

## Historical Relationships

### `frontier-bastion`

- Historical identity: Crown & Conquest / frontier-bastion RTS prototype.
- Relationship to Bloodlines: direct predecessor and concept lineage.
- Preservation decision: copied intact into `archive_preserved_sources/2026-04-14_frontier-bastion-root/`.
- Active status: preserved source only. Not canonical. Not active implementation.

### `deploy/bloodlines`

- Historical identity: compatibility copy under the parent website deploy tree.
- Relationship to Bloodlines: multiple sessions treated this as canonical before the later single-root shift.
- Preservation decision: copied intact into `archive_preserved_sources/2026-04-14_deploy-bloodlines-compatibility-copy/`.
- Additional reconciliation work:
  - `88` deploy-only files were copied into the active canonical tree on 2026-04-14.
  - the newer deploy `index.html` was promoted into the active canonical tree.
  - the deploy-specific single-root note was preserved as `docs/CONSOLIDATION_NOTE_2026-04-13_SINGLE_ROOT_deploy_variant_2026-04-14.md` instead of overwriting the richer in-root note.
- Active status: preserved compatibility copy only. Not canonical as a root. Its latest unique working-tree delta has been brought into the canonical tree.

### 2026-04-14 doctrine DOCX bundle

- Historical identity: externally supplied Bloodlines doctrine drop received outside the project tree.
- Relationship to Bloodlines: authoritative doctrine reinforcement and continuity hardening source.
- Preservation decision: both DOCX files preserved separately in `archive_preserved_sources/2026-04-14_downloads_bloodlines_design_doctrine_docx/`; raw package expansion and text extraction preserved under `02_SESSION_INGESTIONS/2026-04-14_design_doctrine_docx_ingestion/`.
- Active status: active doctrinal source. Integrated into canon via `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md` and `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`.

### `_archive\2026-04-13\bloodlines-single-root`

- Historical identity: prior consolidation staging archive.
- Relationship to Bloodlines: preserved record of the earlier single-root move.
- Preservation decision: imported both preconsolidation trees into the canonical root.
- Active status: historical preservation only.

## Important Non-Source Finding

`D:\ProjectsHome\_archive\2026-03-22\_bookmark_bot\_merge_staging\bloodlines` was inspected during discovery and is not the Bloodlines strategy game. It is an unrelated X bookmark pipeline folder whose name happened to match `bloodlines`. It was not absorbed as a Bloodlines project source.

## Relationship Rules

- No outside path listed above should be treated as a competing active root.
- If future sessions find new Bloodlines material outside this root, import it into `archive_preserved_sources/` and extend this map.
- If a future session changes the physical storage path behind the canonical junction, update this file and `continuity/PROJECT_STATE.json`.

## 2026-04-15 Authoritative In-Root Continuation Surfaces

These are not imported external sources. They are new authoritative continuation surfaces authored inside the canonical root and should be treated as active session references.

| In-root surface | Role |
|---|---|
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md` | Session 46 state-of-game report, civilizational stability feedback |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_47.md` | Session 47 state-of-game report, minor-house territorial foothold |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_48.md` | Session 48 state-of-game report, minor-house AI activation and territorial defense |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_49.md` | Session 49 state-of-game report, save-state counter continuity |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_50.md` | Session 50 state-of-game report, breakaway-march territorial levy and retinue growth |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_51.md` | Session 51 state-of-game report, mixed-bloodline instability weighting |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_52.md` | Session 52 state-of-game report, faith-compatibility weighting in AI marriage logic |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_53.md` | Session 53 state-of-game report, marriage death and dissolution consequence layer |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_54.md` | Session 54 state-of-game report, water-infrastructure tier 1 and field-army sustainment |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_55.md` | Session 55 state-of-game report, espionage and assassination covert operations |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_56.md` | Session 56 state-of-game report, faith operations, missionary pressure, and holy war declaration |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_57.md` | Session 57 state-of-game report, marriage governance by head of household |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_58.md` | Session 58 state-of-game report, field-water attrition and desertion |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_59.md` | Session 59 state-of-game report, counter-intelligence and bloodline-targeting defense |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_60.md` | Session 60 state-of-game report, world pressure and late-stage escalation |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_61.md` | Session 61 state-of-game report, counter-intelligence dossiers and retaliation |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_62.md` | Session 62 state-of-game report, cadet-house marital anchoring and branch pressure |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_63.md` | Session 63 state-of-game report, Hartvale playability and house-gated unique-unit access |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_64.md` | Session 64 state-of-game report, world-pressure internal dynastic destabilization |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_65.md` | Session 65 state-of-game report, world-pressure splinter opportunism |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_66.md` | Session 66 state-of-game report, dossier-backed sabotage retaliation |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_67.md` | Session 67 state-of-game report, player-facing dossier sabotage actionability |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_68.md` | Session 68 state-of-game report, convergence-tier world-pressure escalation |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_69.md` | Session 69 state-of-game report, Ironmark axeman unique-unit lane |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_70.md` | Session 70 state-of-game report, Ironmark axeman AI awareness |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_71.md` | Session 71 state-of-game report, world-pressure source breakdown and off-home tribal targeting |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_72.md` | Session 72 state-of-game report, source-aware rival response to off-home overextension |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_73.md` | Session 73 state-of-game report, source-aware faith backlash under holy-war-led pressure |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_74.md` | Session 74 state-of-game report, source-aware covert backlash under hostile-operations pressure |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_75.md` | Session 75 state-of-game report, source-aware dark-extremes backlash |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_76.md` | Session 76 state-of-game report, source-aware captive backlash |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_77.md` | Session 77 state-of-game report, source-aware territory-expansion backlash |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_78.md` | Session 78 state-of-game report, Hartvale Verdant Warden local support layer |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_79.md` | Session 79 state-of-game report, Scout Rider stage-2 cavalry and infrastructure raiding |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_80.md` | Session 80 state-of-game report, Scout Rider worker and resource-seam harassment plus first counter-raid response |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_81.md` | Session 81 state-of-game report, Scout Rider moving-logistics interception and convoy sustainment disruption |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_82.md` | Session 82 state-of-game report, convoy escort discipline and post-interdiction reconsolidation |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_83.md` | Session 83 state-of-game report, first live match progression and dual-clock legibility |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_84.md` | Session 84 state-of-game report, imminent-engagement warnings and Stage 5 Divine Right declaration windows |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_85.md` | Session 85 state-of-game report, first live political-event architecture through Succession Crisis |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_86.md` | Session 86 state-of-game report, Covenant Test runtime and Territorial Governance Recognition |
| `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_87.md` | Session 87 state-of-game report, Territorial Governance sovereignty hold and victory resolution |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_78.md` | Session 78 continuation prompt, Hartvale Verdant Warden support follow-up |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_79.md` | Session 79 continuation prompt, live Scout Rider stage-2 cavalry and raiding follow-up |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_80.md` | Session 80 continuation prompt, Scout Rider worker harassment and counter-raid response follow-up |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_81.md` | Session 81 continuation prompt, Scout Rider moving-logistics interception follow-up |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_82.md` | Session 82 continuation prompt, convoy escort discipline and post-interdiction reconsolidation follow-up |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_54.md` | Reusable continuation prompt for the next model after Session 53 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_55.md` | Reusable continuation prompt for the next model after Session 54 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_56.md` | Reusable continuation prompt for the next model after Session 55 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_57.md` | Reusable continuation prompt for the next model after Session 56 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_58.md` | Reusable continuation prompt for the next model after Session 57 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_59.md` | Reusable continuation prompt for the next model after Session 58 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_60.md` | Reusable continuation prompt for the next model after Session 59 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_61.md` | Reusable continuation prompt for the next model after Session 60 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_62.md` | Reusable continuation prompt for the next model after Session 61 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_63.md` | Reusable continuation prompt for the next model after Session 62 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_64.md` | Reusable continuation prompt for the next model after Session 63 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_65.md` | Reusable continuation prompt for the next model after Session 64 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_66.md` | Reusable continuation prompt for the next model after Session 65 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_67.md` | Reusable continuation prompt for the next model after Session 66 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_68.md` | Reusable continuation prompt for the next model after Session 67 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_69.md` | Reusable continuation prompt for the next model after Session 68 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_70.md` | Reusable continuation prompt for the next model after Session 69 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_71.md` | Reusable continuation prompt for the next model after Session 70 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_72.md` | Reusable continuation prompt for the next model after Session 71 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_73.md` | Reusable continuation prompt for the next model after Session 72 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_74.md` | Reusable continuation prompt for the next model after Session 73 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_75.md` | Reusable continuation prompt for the next model after Session 74 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_76.md` | Reusable continuation prompt for the next model after Session 75 |
| `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_77.md` | Reusable continuation prompt for the next model after Session 76 |
| `HANDOFF.md` | Active interrupted-session handoff refreshed during the Sessions 46-79 continuation wave |
