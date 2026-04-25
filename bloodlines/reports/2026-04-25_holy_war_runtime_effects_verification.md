# Holy War Runtime Effects Verification (2026-04-25)

Per `docs/migration/unity_port_backlog.md` P2 entry "Holy War Runtime
Economic/Combat Effects": verify first whether any existing Unity system
ticks holy war runtime effects, then implement only if a gap exists.

## Verification Result

**Status: NOT A GAP. System exists.**

Holy war runtime effects are implemented in
`unity/Assets/_Bloodlines/Code/AI/AIHolyWarResolutionSystem.cs`,
specifically the `TickActiveHolyWars` method (line 279) which constitutes
Phase B of the system.

## Side-by-Side With Browser `tickFaithHolyWars`

Browser source: `src/game/core/simulation.js` `tickFaithHolyWars`
(line 4160). For each faction's `activeHolyWars` list:

| Browser Effect | Unity Implementation | Status |
|---|---|---|
| Prune expired entries (`expiresAt <= meta.elapsed`) | `if (entry.ExpiresAtInWorldDays <= inWorldDays) warBuffer.RemoveAt(j)` | match |
| Push expiration narrative if player-related | `if (sourceFaction.FactionId.Equals(playerId) || entry.TargetFactionId.Equals(playerId)) PushExpirationMessage(...)` | match |
| Adjust target legitimacy (`adjustLegitimacy(target, -loyaltyPulse * SUSTAINED_LEGITIMACY_DRAIN_MULTIPLIER * dt)`) | -- | **deferred** (no canonical FactionLegitimacyComponent until dynasty-core lane lands; documented in source comment lines 53-57) |
| Sustained loyalty drain on target's lowest-loyalty CP per dt (`targetPoint.loyalty -= loyaltyPulse * SUSTAINED_LOYALTY_DRAIN_MULTIPLIER * dt`) | `ApplySustainedLoyaltyDrain(em, targetEntity, ..., entry.LoyaltyPulse * SustainedLoyaltyDrainMultiplier * dt)` | match |
| Pulse interval boost source faith intensity (`faction.faith.intensity += intensityPulse`) | `faith.Intensity += intensityPulse` | match |
| Pulse interval drain target's lowest CP (`targetPoint.loyalty -= loyaltyPulse`) | `ApplyPulseEffects(em, ..., entry.LoyaltyPulse)` | match |
| Pulse interval fallback when target has no CP: drain legitimacy | -- | **deferred** (same legitimacy reason) |

## Deferrals

The two effects marked deferred above both depend on a canonical
faction-level `Legitimacy` field. The Unity port has not introduced this
field yet; it is reserved for the dynasty-core lane. Once that lane
lands, the holy war Phase B tick can be extended to apply both the
per-tick legitimacy drain and the no-CP fallback drain.

This is documented inline in
`unity/Assets/_Bloodlines/Code/AI/AIHolyWarResolutionSystem.cs` lines
54-60 and is consistent with how the entire dynasty-core lane has been
deferred across multiple slices.

## Backlog Item Resolution

The backlog suggested adding `attack bonus to belligerents` and
`conviction events on sustained actions` as part of holy war runtime
effects. Both suggestions go beyond canonical browser behavior:

- **Attack bonus to belligerents:** `tickFaithHolyWars` does not apply a
  combat damage multiplier. There is no attack bonus tied to active holy
  war in the browser source. Adding one would be a non-canonical
  invention and must be specced in canon (`01_CANON/UNITY_CANONICAL_ADVANCEMENTS_2026-04-25.md`)
  before implementation.
- **Conviction events on sustained actions:** the browser fires
  conviction events at declaration resolution (`startHolyWarDeclaration`
  -> `applyConvictionEvent`) but not during the sustained-tick phase.
  Phase A of `AIHolyWarResolutionSystem` already handles the declaration-
  time conviction event (see lines 12-34 of the source comment). No
  sustained-tick conviction event is part of canonical browser behavior.

Both backlog suggestions are stricken from the implementation scope.
The remaining canonical work (legitimacy drain) is correctly deferred.

## Action Taken

No code change. The backlog entry should be marked verified and the
canonical gap (legitimacy drain) folded into the dynasty-core lane's
list of items to wire on landing.
