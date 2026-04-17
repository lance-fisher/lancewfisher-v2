# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-16
Session: 95
Author: Claude

## Scope

Session 95 added Trueborn recognition diplomacy, giving kingdoms a formal diplomatic tool to engage with the Trueborn Rise arc. Kingdoms can now recognize the Trueborn City's historical claim to gain standing and reduce Rise pressure by 75%, at the cost of legitimacy and resources. This creates a genuine strategic dilemma: recognizing the claim reduces the anti-snowball pressure but also legitimizes a competitor's sovereignty.

## Changes Landed

### Recognition diplomacy system (`src/game/core/simulation.js`)

- **Constants**: `TRUEBORN_RECOGNITION_INFLUENCE_COST = 40`, `TRUEBORN_RECOGNITION_GOLD_COST = 60`, `TRUEBORN_RECOGNITION_STANDING_BONUS = 6`, `TRUEBORN_RECOGNITION_LEGITIMACY_COST = 5`.

- **`getTruebornRecognitionTerms(state, factionId)`**: Returns availability, cost, standing bonus, and legitimacy cost. Rejects: non-kingdoms, inactive Rise arc (stage 0), already recognized, insufficient resources.

- **`recognizeTruebornClaim(state, factionId)`**: Spends influence and gold, reduces legitimacy, sets `diplomacy.truebornRecognition = true`, grants +6 standing to Trueborn City, pushes message.

### Rise pressure exemption for recognized kingdoms

- All three stages of `tickTruebornRiseArc` now check each territory owner's `truebornRecognition` flag.
- Recognized kingdoms receive only 25% of the normal loyalty and legitimacy pressure (0.25x multiplier).
- This creates a meaningful 75% reduction, making recognition a powerful defensive tool against the Rise arc.

### Dynasty panel UI (`src/game/main.js`)

- Imported `getTruebornRecognitionTerms` and `recognizeTruebornClaim`.
- A "Recognize Trueborn Claim" action button now appears in the Diplomacy section when the Rise arc is at stage 1 or higher.
- The button shows cost, standing bonus, legitimacy cost, and the 75% Rise pressure reduction.
- After recognition, the button changes to "Trueborn Claim Recognized" with an informational detail line.
- Disabled with reason when unavailable (insufficient resources, already recognized, Rise not active).

### Runtime bridge test coverage (`tests/runtime-bridge.mjs`)

- 10 new assertions covering:
  - Recognition unavailable without resources.
  - Recognition available with sufficient resources.
  - Standing bonus included in terms.
  - Recognition succeeds and increases standing.
  - Recognition costs legitimacy.
  - Recognition flag is set correctly.
  - Duplicate recognition is rejected.
  - Recognized kingdoms suffer less Rise loyalty pressure than unrecognized.
  - Message log records recognition.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed (including 10 new recognition assertions).
- All syntax checks pass.

## Canonical Interdependency Check

Recognition diplomacy connects to:

1. **Trueborn Rise arc** (Session 94): Recognition is the primary diplomatic counter to Rise pressure.
2. **Trade standing** (Session 93): +6 standing bonus from recognition feeds into the acceptance profile endorsement.
3. **Legitimacy**: -5 legitimacy cost makes recognition a real trade-off, especially during governance recognition.
4. **Sovereignty path**: Reduced Rise pressure eases the late-game acceptance environment for recognized kingdoms.
5. **Non-aggression pacts** (Session 90): Recognition and pacts are complementary diplomatic tools.

## Gap Analysis

- Trueborn recognition diplomacy: NEW, moved to LIVE with terms, execution, pressure exemption, UI, and test coverage.
- The Trueborn City now has three live layers: trade relationships (Session 93), Rise arc (Session 94), and recognition diplomacy (Session 95).

## Session 96 Next Action

1. If continuing world depth: implement naval-world integration or broader theatre-of-war expansion.
2. If pivoting: open the Unity Play Mode verification shell.
3. If polish: deepen the match-stage system, add Trueborn military unit spawning for Stage 3, or extend trade-network intelligence.
