# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14  
Session: 7  
Canonical root: `D:\ProjectsHome\Bloodlines`

## Scope Of This Addendum

Session 7 moves Bloodlines captivity from passive consequence into active dynastic command. Until now, capture mattered structurally but did not yet create a playable decision surface after the moment of loss. The ledger existed, the influence trickle existed, succession consequences existed, but the player still lacked the follow-through layer. This addendum records the arrival of that layer.

## New Live State

### Captivity now produces active dynastic operations

- Captured members can now be recovered through negotiated ransom.
- Captured members can now be targeted with covert rescue operations.
- Held captives can now be released by the captor through a direct ransom-demand action when the rival dynasty can satisfy the terms.

This matters because Bloodlines canon treats bloodline capture as an ongoing political and dynastic wound, not a one-time combat event. Session 7 makes that wound actionable.

### Dynasty operations now exist as state, not only as button clicks

- `dynasty.operations.active` tracks in-flight rescue and ransom operations.
- `dynasty.operations.history` records completed, failed, or voided outcomes.
- Operation cost is escrowed up front.
- Resolution happens in simulation time rather than immediately.

This is the correct architectural move. Bloodlines needs long-form dynastic pressure and delayed consequence. Immediate one-click resolution would have undercut the intended weight of bloodline recovery.

### Rescue resolution now respects fortification and covert pressure

Rescue is not arbitrary. Session 7 resolves rescue through a deterministic pressure comparison:

- source-house covert and diplomatic leverage
- commander support
- captive renown and role value
- captor fortification depth
- active keep ward profile
- captor covert resistance

This matters because a captive inside a hardened, warded dynastic keep should not be equivalent to a captive held in a weak rear seat. Session 7 makes the recovery layer respect the fortification lane that already exists.

### The dynasty panel now exposes captivity as command work

The browser/spec HUD now surfaces:

- active dynasty operations with progress
- `Negotiate Release` for captured members
- `Send Rescue Cell` for captured members
- `Demand Ransom` for held captives

This is important for Bloodlines specifically because bloodline consequence is supposed to be structurally visible in the command experience, not buried in a secondary ledger.

## Updated Diagnosis

### Dynasty-system maturity

The browser/spec lane now has a real captivity loop:

- capture event
- succession shock
- ledger persistence
- ongoing value to the captor
- active recovery / release decisions
- timed operational resolution

That is a major maturity jump for the dynasty system. Bloodline loss is no longer only bookkeeping plus legitimacy drift.

### System interplay maturity

Session 7 improves the way Bloodlines systems talk to each other:

- capture interacts with succession
- recovery interacts with diplomacy and covert roles
- rescue difficulty interacts with fortification depth and ward state
- legitimacy recovers through successful bloodline retrieval

This is the kind of layered systems behavior the project needs more of. It moves the runtime away from isolated mechanics and toward the intended civilizational consequence model.

### UI maturity

The dynasty panel is still not the final full-scale command surface, but it is now more structurally honest. A core dynastic crisis now appears where the player is already expected to read bloodline state, and the response options are directly attached to that state.

## Remaining Major Deficits After Session 7

### Captured Heir homeland penalty

The runtime now recovers captives, but it still does not express the deeper homeland penalty of a captured heir or captured ruling bloodline member at the full canonical weight.

### Other canonical capture outcomes

Ransom and rescue are now live, but execution, enslavement, marriage conversion, and broader political exploitation of captives are still absent.

### Engineer specialists and siege continuity

The attacker now prepares siege and the dynasty now resolves captivity, but the runtime still lacks engineer specialists, supply continuity, line interdiction, and extended siege sustainment.

### Sabotage and breach-enabling operations

Covert rescue now exists, but sabotage remains underbuilt. Gate opening, fire raising, counter-mining disruption, supply poisoning, and other siege-enabling covert branches still need to be added.

### Full realm dashboard

The command surface still surfaces only the six heaviest pressure bands rather than the complete 11-state realm dashboard.

### Unity runtime

The Unity production lane remains structurally prepared but still blocked by the unresolved editor-version decision.

## Next Correct Build Direction

### Browser/spec lane

The strongest next expansion after Session 7 is:

1. engineer specialists for siege, repair, mining, and earthworks
2. siege supply continuity and interdiction
3. sabotage and breach-enabling covert operations
4. commander keep-presence expansion beyond reserve tempo
5. full 11-state realm-condition dashboard
6. next siege-support classes such as ballista and mantlet

These follow directly from what is now live. Bloodlines now has attacker preparation, defender fortification depth, and dynastic captivity consequence. The next correct move is to deepen the operational layer that links those systems together.

### Unity lane

No change to the blocker:

1. resolve Unity version alignment
2. open and sync JSON content
3. begin the ECS foundation
4. seed the first playable scene
5. bring bloodline-forward HUD structure into the production lane

## Conclusion

Session 7 materially advances Bloodlines by making captured bloodline members part of live command play rather than passive aftermath. The dynasty layer now has a real operational response path, and that path meaningfully respects diplomacy, covert pressure, fortification, and the passage of campaign time.

Bloodlines is still far from complete, but the project now behaves more like a house-based war of succession, custody, leverage, and recovery, which is much closer to the intended final game.
