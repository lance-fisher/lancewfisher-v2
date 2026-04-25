using Bloodlines.Components;
using Bloodlines.HUD;
using Bloodlines.Systems;
using Unity.Entities;
using UnityEngine;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Early-game foundation debug panel.
    ///
    /// Exposes read-only status for:
    ///   Keep deployment state, build tier, water capacity, population breakdown,
    ///   draft slider (read + write), productivity states, squad duty counts.
    ///
    /// Also provides test commands:
    ///   - Deploy Keep (player faction)
    ///   - Adjust draft rate
    ///   - Spawn test militia squad
    ///   - Set squad assignment / stand down
    ///
    /// Canon: early-game-foundation prompt 2026-04-25 UI requirements.
    /// </summary>
    public sealed partial class BloodlinesDebugCommandSurface
    {
        bool _earlyGameFoldout = true;
        int  _debugDraftRate   = 0;

        void DrawEarlyGamePanel()
        {
            _earlyGameFoldout = GUILayout.Toggle(_earlyGameFoldout, "Early Game Foundation");
            if (!_earlyGameFoldout) return;

            GUILayout.BeginVertical(GUI.skin.box);

            if (!TryGetEntityManager(out var em))
            {
                GUILayout.Label("No ECS world.");
                GUILayout.EndVertical();
                return;
            }

            // --- HUD snapshot ---
            var hudQuery = em.CreateEntityQuery(ComponentType.ReadOnly<EarlyGameHUDComponent>());
            if (hudQuery.CalculateEntityCount() == 0)
            {
                GUILayout.Label("EarlyGameHUDComponent not found.");
                GUILayout.EndVertical();
                hudQuery.Dispose();
                return;
            }

            var hud = hudQuery.GetSingleton<EarlyGameHUDComponent>();
            hudQuery.Dispose();

            // Keep + tier
            GUILayout.Label($"Keep Deployed: {hud.KeepDeployed}  |  Build Tier: {hud.BuildTier}");
            GUILayout.Label($"Prereqs — Housing:{hud.HasHousing}  Water:{hud.HasWater}  Food:{hud.HasFoodSource}  TrainingYard:{hud.HasTrainingYard}");

            GUILayout.Space(4);

            // Population / water
            GUILayout.Label($"Pop: {hud.PopTotal}/{hud.PopCap}  Available: {hud.PopAvailable}");
            GUILayout.Label($"Water Capacity: {hud.WaterCapacity}  (pop supported by infrastructure)");

            GUILayout.Space(4);

            // Resources
            GUILayout.Label($"Food: {hud.Food:F1}  Water: {hud.Water:F1}  Wood: {hud.Wood:F1}");

            GUILayout.Space(4);

            // Draft
            GUILayout.Label("--- Draft ---");
            GUILayout.Label($"Rate: {hud.DraftRatePct}%  Pool: {hud.DraftPool}  Trained: {hud.TrainedMilitary}  Untrained: {hud.UntrainedDrafted}");
            GUILayout.Label($"Reserve: {hud.ReserveMilitary}  ActiveDuty: {hud.ActiveDutyMilitary}  OverDrafted: {hud.OverDrafted}");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Set Draft %:", GUILayout.Width(80));
            _debugDraftRate = int.TryParse(GUILayout.TextField(_debugDraftRate.ToString(), GUILayout.Width(50)), out int parsed) ? parsed : _debugDraftRate;
            if (GUILayout.Button("Apply", GUILayout.Width(60)))
                SetPlayerDraftRate(_debugDraftRate);
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            // Productivity
            GUILayout.Label("--- Productivity ---");
            GUILayout.Label($"Base: {hud.BaseProductivity:P0}  Effective: {hud.EffectiveProductivity:P0}");
            GUILayout.Label($"Food OK: {hud.FoodAdequate}  Water OK: {hud.WaterAdequate}  Housing OK: {hud.HousingAdequate}");

            GUILayout.Space(4);

            // Squads
            GUILayout.Label("--- Squads ---");
            GUILayout.Label($"Total: {hud.TotalSquads}  Reserve: {hud.ReserveSquads}  ActiveDuty: {hud.ActiveDutySquads}");

            GUILayout.Space(4);

            // Test commands
            GUILayout.Label("--- Test Commands ---");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Deploy Keep")) DeployPlayerKeep();
            if (GUILayout.Button("Spawn Test Squad")) SpawnTestSquad();
            if (GUILayout.Button("Scout (1st squad)")) SetFirstSquadAssignment(SquadAssignmentType.Scout);
            if (GUILayout.Button("Stand Down")) SetFirstSquadAssignment(SquadAssignmentType.None);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        static void DeployPlayerKeep()
        {
            if (!TryGetEntityManager(out var em)) return;

            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FoundingRetinueComponent>());
            using var entities = q.ToEntityArray(Unity.Collections.Allocator.Temp);
            foreach (var e in entities)
            {
                var fid = em.GetComponentData<FactionComponent>(e).FactionId;
                if (!fid.Equals(new Unity.Collections.FixedString32Bytes("player"))) continue;

                var retinue = em.GetComponentData<FoundingRetinueComponent>(e);
                retinue.IsDeployed = true;
                retinue.DeployPosition = UnityEngine.Vector3.zero;
                em.SetComponentData(e, retinue);
                UnityEngine.Debug.Log("[EarlyGame] Player Keep deployed via debug command.");
                break;
            }
            q.Dispose();
        }

        static void SetPlayerDraftRate(int rate)
        {
            if (!TryGetEntityManager(out var em)) return;

            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<MilitaryDraftComponent>());
            using var entities = q.ToEntityArray(Unity.Collections.Allocator.Temp);
            foreach (var e in entities)
            {
                var fid = em.GetComponentData<FactionComponent>(e).FactionId;
                if (!fid.Equals(new Unity.Collections.FixedString32Bytes("player"))) continue;

                var draft = em.GetComponentData<MilitaryDraftComponent>(e);
                draft.DraftRatePct = Mathf.Clamp((rate / 5) * 5, 0, 100);
                em.SetComponentData(e, draft);
                UnityEngine.Debug.Log($"[EarlyGame] Player draft rate set to {draft.DraftRatePct}%.");
                break;
            }
            q.Dispose();
        }

        static void SpawnTestSquad()
        {
            if (!TryGetEntityManager(out var em)) return;

            var squad = em.CreateEntity();
            em.AddComponentData(squad, new FactionComponent { FactionId = new Unity.Collections.FixedString32Bytes("player") });
            em.AddComponentData(squad, new MilitiaSquadComponent
            {
                SquadSize      = EarlyGameConstants.SquadSize,
                DutyState      = DutyState.Reserve,
                AssignmentType = SquadAssignmentType.None,
            });
            UnityEngine.Debug.Log("[EarlyGame] Test militia squad spawned in Reserve.");
        }

        static void SetFirstSquadAssignment(SquadAssignmentType assignmentType)
        {
            if (!TryGetEntityManager(out var em)) return;

            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<MilitiaSquadComponent>());
            using var entities = q.ToEntityArray(Unity.Collections.Allocator.Temp);
            foreach (var e in entities)
            {
                var fid = em.GetComponentData<FactionComponent>(e).FactionId;
                if (!fid.Equals(new Unity.Collections.FixedString32Bytes("player"))) continue;

                var squadData = em.GetComponentData<MilitiaSquadComponent>(e);
                squadData.AssignmentType = assignmentType;
                em.SetComponentData(e, squadData);
                UnityEngine.Debug.Log($"[EarlyGame] First player squad assignment -> {assignmentType}.");
                break;
            }
            q.Dispose();
        }
    }
}
