# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14  
Session: 8  
Canonical root: `D:\ProjectsHome\Bloodlines`

## Scope Of This Addendum

Session 8 closes the largest remaining attacker-side structural gap in the browser/spec lane after siege preparation became live in Session 6. Before this wave, Stonehelm could decide to lay siege, build engines, and form lines, but the operation still had no specialist labor corps and no vulnerable sustainment chain. That meant the runtime understood the idea of siege more than the operational reality of siege. This addendum records the arrival of that reality.

## New Live State

### Formal siege now has a logistics seat

- `supply_camp` is now live as a forward logistics anchor.
- `supply_wagon` is now live as the mobile sustainment unit that carries siege continuity forward from that anchor.
- Siege engines now care whether the supply chain is intact.

This matters because Bloodlines canon treats siege as a campaign operation with rear-area dependency, not as a self-contained engine blob. Session 8 moves the runtime into that doctrine.

### Engineer specialists are now real battlefield population

- `siege_engineer` is now live in the workshop roster.
- Engineers now provide nearby breach-support pressure.
- Engineers now repair damaged siege engines in the line.

This is important architecturally because engineers are no longer just future canon text. They are now a distinct operational population category with runtime behavior and therefore a real future expansion seam for mining, counter-mining, saps, berms, and breach assessment.

### Unsupplied siege is now a weaker branch

- Engines without a live camp-wagon link now lose operational efficiency.
- Supply continuity can be broken by destroying or separating the chain.
- The runtime now rewards sustained siege organization over merely possessing engines.

This is the correct direction for Bloodlines because the project needs siege failure and siege disruption to emerge from system interplay rather than only from flat numeric thresholds.

### Attacking AI now respects sustainment, not only preparation

- Stonehelm can now add a forward supply camp after workshop activation.
- Stonehelm now queues engineers and wagons after the opening bombard engine.
- Stonehelm now delays keep assault if engineers are absent, wagons are absent, or the engines remain unsupplied.

This materially improves AI credibility. The AI is still not at the final doctrinal level, but it no longer jumps straight from engine ownership to breach commitment.

## Updated Diagnosis

### Siege-system maturity

The browser/spec lane now has the following attacker-side layers live:

- refusal of underforce keep assault
- dedicated siege production infrastructure
- differentiated engine roster
- staged siege-line commitment
- specialist engineer population
- sustainment through camps and wagons
- supply-aware assault gating

That is a meaningful maturation step. The siege model now expresses an actual operational chain instead of a simple prerequisite list.

### Architecture strength

This wave was implemented through extensible seams rather than isolated exceptions:

- new unit and building definitions are data-driven
- engineer support extends the existing siege-support profile path
- supply continuity is simulation-time state on units, not a menu-only abstraction
- AI reads the same live unit and building state that the player does
- HUD/debug surfaces now expose siege sustainment state

That is the right foundation for future additions such as mining, counter-mining, road interdiction, relief-army pressure, and siege attrition.

### Remaining structural deficits after Session 8

The runtime still falls short of the full bible in several important ways:

- sabotage remains underbuilt: gate opening, fire raising, supply poisoning, and breach-enabling covert work are still absent
- commander keep-presence is still narrower than the full dynastic command doctrine
- long-siege adaptation is still shallow: relief windows, repeated assault windows, supply protection patrols, and post-repulse tactical adjustment are not yet mature
- the 11-state realm dashboard is still compressed to the current six-pill battlefield HUD
- next siege-support classes such as `ballista` and `mantlet` are still absent
- Unity remains blocked on version alignment and therefore still trails the browser/spec lane by a large runtime distance

## Gap Mapping Against Canon

### Missing capability: sabotage as siege enabler

Why it matters:
Bloodlines canon treats sabotage as a multiplier for siege, not as flavor. Without it, the covert layer still stops short of directly shaping breach opportunity.

Required expansion:
Extend the existing `dynasty.operations` framework so covert work can target fortification sections, supply continuity, or gate security instead of only captive recovery.

Implementation path:
Use the same timed-operation architecture now proven by Session 7, but route resolution into fortification state, ward state, and supply state.

### Missing capability: longer siege AI adaptation

Why it matters:
A sustained siege must react to repulse, supply loss, and shifting defender readiness. Opening preparation alone is not enough.

Required expansion:
AI needs repeated assault windows, supply-protection behavior, and branch changes after a failed advance.

Implementation path:
Build on the now-live supply-aware delay logic rather than replacing it.

### Missing capability: full command legibility

Why it matters:
Bloodlines is meant to surface realm condition and bloodline consequence continuously. The sustainment layer is now live, but the UI still compresses too much of the realm.

Required expansion:
Promote the current six-pill realm bar toward the full 11-state dashboard and fold siege sustainment into that full panel rather than leaving it mostly in debug/meta text.

## Next Correct Build Direction

1. Extend commander keep-presence into a fuller dynastic defensive-command layer.
2. Add sabotage and breach-enabling covert operations against fortification and supply state.
3. Add `ballista` and `mantlet` with distinct tactical jobs.
4. Extend AI from opening sustainment into longer siege conduct and response after repulse.
5. Expand the battlefield HUD into the full 11-state realm dashboard.

## Conclusion

Session 8 materially strengthens Bloodlines by making formal siege depend on labor, logistics, and continuity rather than only on the presence of engines. The browser/spec lane now behaves more like a house war fought through organized siege effort, which is much closer to the intended final game.
