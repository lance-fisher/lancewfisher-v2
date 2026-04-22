#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.GameTime;
using Bloodlines.HUD;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for the consolidated player command-deck HUD
    /// snapshot that consumes the already-landed HUD read models.
    /// </summary>
    public static class BloodlinesPlayerCommandDeckHUDSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-player-command-deck-hud-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Player Command Deck HUD Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerCommandDeckHUDSmokeValidation() => RunInternal(batchMode: true);

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception exception)
            {
                success = false;
                message = "Player command-deck HUD smoke validation errored: " + exception;
            }

            string artifact = "BLOODLINES_PLAYER_COMMAND_DECK_HUD_SMOKE " +
                              (success ? "PASS" : "FAIL") +
                              Environment.NewLine + message;
            UnityDebug.Log(artifact);
            try
            {
                string logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, artifact);
            }
            catch
            {
            }

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static bool RunAllPhases(out string report)
        {
            var lines = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunSummaryProjectionPhase(lines);
            ok &= RunGreatReckoningAlertPhase(lines);
            ok &= RunFortificationThreatAlertPhase(lines);
            ok &= RunVictoryImminentAlertPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunSummaryProjectionPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-command-deck-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 42f);
            SeedMatchHud(entityManager, "Encounter Establishment", "commitment", "watchful", 3, false);
            Entity player = SeedFaction(entityManager, "player", "yellow", "green", "green", 28f, "rising");
            SeedVictoryReadout(entityManager, player,
                ("TerritorialGovernance", 0.620f, false, 18f),
                ("DivineRight", 0.250f, false, float.NaN),
                ("CommandHallFall", 0.100f, false, float.NaN));
            SeedVictoryLeaderboard(entityManager,
                ("enemy", "DivineRight", 0.700f, false),
                ("player", "TerritorialGovernance", 0.620f, true));
            SeedDynastyRenownLeaderboard(entityManager,
                ("enemy", 1, 33f, 40f, false),
                ("player", 2, 28f, 34f, true));

            TickOnce(world);

            if (!TryReadSnapshot(debugScope.CommandSurface, out var fields, out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read player command-deck HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "StageLabel") != "Encounter Establishment" ||
                ReadField(fields, "PhaseLabel") != "commitment" ||
                ReadField(fields, "LeadingVictoryConditionId") != "TerritorialGovernance" ||
                Math.Abs(ParseFloat(fields, "LeadingVictoryProgressPct") - 0.62f) > 0.001f ||
                ReadField(fields, "VictoryRank") != "2" ||
                ReadField(fields, "RenownRank") != "2" ||
                Math.Abs(ParseFloat(fields, "RenownScore") - 28f) > 0.001f ||
                ReadField(fields, "PopulationBand") != "yellow" ||
                ReadField(fields, "PrimaryAlertLabel") != "stable")
            {
                lines.AppendLine($"Phase 1 FAIL: unexpected command-deck summary '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: command deck mirrors stage, victory, renown, and realm-band state for the player.");
            return true;
        }

        private static bool RunGreatReckoningAlertPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-command-deck-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 61f);
            SeedMatchHud(entityManager, "Final Convergence", "resolution", "reckoning", 8, true);
            Entity player = SeedFaction(entityManager, "player", "green", "green", "green", 41f, "ascendant");
            SeedVictoryReadout(entityManager, player, ("TerritorialGovernance", 0.880f, true, 5f));
            SeedVictoryLeaderboard(entityManager, ("player", "TerritorialGovernance", 0.880f, true));
            SeedDynastyRenownLeaderboard(entityManager, ("player", 1, 41f, 46f, true));

            TickOnce(world);

            if (!TryReadSnapshot(debugScope.CommandSurface, out var fields, out var readout) ||
                ReadField(fields, "PrimaryAlertLabel") != "great_reckoning" ||
                ReadField(fields, "GreatReckoningActive") != "true")
            {
                lines.AppendLine($"Phase 2 FAIL: expected great-reckoning alert, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: great reckoning takes alert precedence over all other command-deck states.");
            return true;
        }

        private static bool RunFortificationThreatAlertPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-command-deck-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 35f);
            SeedMatchHud(entityManager, "War Turning", "commitment", "rising", 5, false);
            Entity player = SeedFaction(entityManager, "player", "green", "green", "green", 30f, "rising");
            SeedVictoryReadout(entityManager, player, ("DivineRight", 0.400f, true, float.NaN));
            SeedVictoryLeaderboard(entityManager, ("player", "DivineRight", 0.400f, true));
            SeedDynastyRenownLeaderboard(entityManager, ("player", 1, 30f, 30f, true));
            SeedFortificationHud(entityManager, "player", true);

            TickOnce(world);

            if (!TryReadSnapshot(debugScope.CommandSurface, out var fields, out var readout) ||
                ReadField(fields, "FortificationThreatActive") != "true" ||
                ReadField(fields, "PrimaryAlertLabel") != "fortification_threat")
            {
                lines.AppendLine($"Phase 3 FAIL: expected fortification threat alert, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: active threatened fortifications raise the command-deck fortification alert.");
            return true;
        }

        private static bool RunVictoryImminentAlertPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-command-deck-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 72f);
            SeedMatchHud(entityManager, "Final Convergence", "resolution", "high", 4, false);
            Entity player = SeedFaction(entityManager, "player", "green", "green", "green", 49f, "ascendant");
            SeedVictoryReadout(entityManager, player,
                ("TerritorialGovernance", 0.910f, true, 2f),
                ("DivineRight", 0.320f, false, float.NaN));
            SeedVictoryLeaderboard(entityManager,
                ("player", "TerritorialGovernance", 0.910f, true),
                ("enemy", "DivineRight", 0.500f, false));
            SeedDynastyRenownLeaderboard(entityManager,
                ("player", 1, 49f, 50f, true),
                ("enemy", 2, 18f, 24f, false));

            TickOnce(world);

            if (!TryReadSnapshot(debugScope.CommandSurface, out var fields, out var readout) ||
                ReadField(fields, "VictoryRank") != "1" ||
                ReadField(fields, "PrimaryAlertLabel") != "victory_imminent")
            {
                lines.AppendLine($"Phase 4 FAIL: expected victory-imminent alert, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: the command deck flags the player as victory-imminent when leading above 0.850 progress.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            presentationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerCommandDeckHUDSystem>());
            presentationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            Entity entity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(entity, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });
        }

        private static void SeedMatchHud(
            EntityManager entityManager,
            string stageLabel,
            string phaseLabel,
            string worldPressureLabel,
            int worldPressureLevel,
            bool greatReckoningActive)
        {
            Entity entity = entityManager.CreateEntity(typeof(MatchProgressionHUDComponent));
            entityManager.SetComponentData(entity, new MatchProgressionHUDComponent
            {
                StageNumber = 4,
                StageId = new FixedString32Bytes("war_turning_of_tides"),
                StageLabel = new FixedString64Bytes(stageLabel),
                PhaseId = new FixedString32Bytes(phaseLabel),
                PhaseLabel = new FixedString32Bytes(phaseLabel),
                StageReadiness = 0.5f,
                NextStageId = new FixedString32Bytes("final_convergence"),
                NextStageLabel = new FixedString64Bytes("Final Convergence"),
                InWorldDays = 40f,
                InWorldYears = 4f,
                DeclarationCount = 1,
                WorldPressureLevel = worldPressureLevel,
                WorldPressureLabel = new FixedString32Bytes(worldPressureLabel),
                WorldPressureScore = worldPressureLevel,
                WorldPressureTargeted = worldPressureLevel >= 6,
                DominantLeaderFactionId = new FixedString32Bytes("player"),
                DominantLeaderTerritoryShare = 0.5f,
                GreatReckoningActive = greatReckoningActive,
                GreatReckoningTargetFactionId = greatReckoningActive ? new FixedString32Bytes("player") : default,
                GreatReckoningShare = greatReckoningActive ? 0.77f : 0f,
            });
        }

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            string populationBand,
            string loyaltyBand,
            string faithBand,
            float score,
            string band)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(RealmConditionHUDComponent),
                typeof(DynastyRenownHUDComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new RealmConditionHUDComponent
            {
                FactionId = new FixedString32Bytes(factionId),
                PopulationBand = new FixedString32Bytes(populationBand),
                LoyaltyBand = new FixedString32Bytes(loyaltyBand),
                FaithBand = new FixedString32Bytes(faithBand),
            });
            entityManager.SetComponentData(entity, new DynastyRenownHUDComponent
            {
                FactionId = new FixedString32Bytes(factionId),
                RenownRank = 2,
                RenownScore = score,
                PeakRenown = math.max(score, score + 2f),
                ScoreToPeakRatio = score / math.max(score, score + 2f),
                RenownBandLabel = new FixedString32Bytes(band),
                RenownBandColor = new FixedString32Bytes("green"),
                StatusLabel = new FixedString32Bytes("stable"),
            });
            entityManager.AddBuffer<VictoryConditionReadoutComponent>(entity);
            return entity;
        }

        private static void SeedVictoryReadout(
            EntityManager entityManager,
            Entity factionEntity,
            params (string conditionId, float progressPct, bool isLeading, float etaDays)[] entries)
        {
            DynamicBuffer<VictoryConditionReadoutComponent> buffer = entityManager.GetBuffer<VictoryConditionReadoutComponent>(factionEntity);
            buffer.Clear();
            for (int i = 0; i < entries.Length; i++)
            {
                buffer.Add(new VictoryConditionReadoutComponent
                {
                    ConditionId = new FixedString32Bytes(entries[i].conditionId),
                    ProgressPct = entries[i].progressPct,
                    IsLeading = entries[i].isLeading,
                    TimeRemainingEstimateInWorldDays = entries[i].etaDays,
                });
            }
        }

        private static void SeedVictoryLeaderboard(
            EntityManager entityManager,
            params (string factionId, string conditionId, float progressPct, bool isHumanPlayer)[] entries)
        {
            Entity entity = entityManager.CreateEntity(typeof(VictoryLeaderboardHUDSingleton));
            entityManager.SetComponentData(entity, new VictoryLeaderboardHUDSingleton
            {
                LastRefreshInWorldDays = 0f,
            });
            DynamicBuffer<VictoryLeaderboardHUDComponent> buffer = entityManager.AddBuffer<VictoryLeaderboardHUDComponent>(entity);
            for (int i = 0; i < entries.Length; i++)
            {
                buffer.Add(new VictoryLeaderboardHUDComponent
                {
                    FactionId = new FixedString32Bytes(entries[i].factionId),
                    LeadingConditionId = new FixedString32Bytes(entries[i].conditionId),
                    ProgressPct = entries[i].progressPct,
                    IsHumanPlayer = entries[i].isHumanPlayer,
                });
            }
        }

        private static void SeedDynastyRenownLeaderboard(
            EntityManager entityManager,
            params (string factionId, int rank, float score, float peak, bool isHumanPlayer)[] entries)
        {
            Entity entity = entityManager.CreateEntity(typeof(DynastyRenownLeaderboardHUDSingleton));
            entityManager.SetComponentData(entity, new DynastyRenownLeaderboardHUDSingleton
            {
                LastRefreshInWorldDays = 0f,
            });
            DynamicBuffer<DynastyRenownLeaderboardHUDComponent> buffer = entityManager.AddBuffer<DynastyRenownLeaderboardHUDComponent>(entity);
            for (int i = 0; i < entries.Length; i++)
            {
                buffer.Add(new DynastyRenownLeaderboardHUDComponent
                {
                    FactionId = new FixedString32Bytes(entries[i].factionId),
                    RenownRank = entries[i].rank,
                    RenownScore = entries[i].score,
                    PeakRenown = entries[i].peak,
                    IsHumanPlayer = entries[i].isHumanPlayer,
                    IsLeadingDynasty = entries[i].rank == 1,
                    Interregnum = false,
                    RulerMemberId = default,
                    RulerTitle = default,
                    BandLabel = new FixedString32Bytes("stable"),
                    StatusLabel = new FixedString32Bytes("stable"),
                });
            }
        }

        private static void SeedFortificationHud(EntityManager entityManager, string ownerFactionId, bool threatActive)
        {
            Entity entity = entityManager.CreateEntity(typeof(FortificationHUDComponent));
            entityManager.SetComponentData(entity, new FortificationHUDComponent
            {
                SettlementId = new FixedString64Bytes("ironmark"),
                OwnerFactionId = new FixedString32Bytes(ownerFactionId),
                Tier = 2,
                OpenBreachCount = threatActive ? 1 : 0,
                ReserveFrontage = 2,
                MusteredDefenderCount = 4,
                ReadyReserveCount = 2,
                RecoveringReserveCount = 0,
                ThreatActive = threatActive,
                SealingProgress01 = threatActive ? 0.35f : 0f,
                RecoveryProgress01 = 0f,
            });
        }

        private static bool TryReadSnapshot(
            BloodlinesDebugCommandSurface commandSurface,
            out Dictionary<string, string> fields,
            out string readout)
        {
            fields = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!commandSurface.TryDebugGetPlayerCommandDeckHUDSnapshot(out readout))
            {
                return false;
            }

            string[] parts = readout.Split('|');
            if (parts.Length == 0 || !string.Equals(parts[0], "PlayerCommandDeckHUD", StringComparison.Ordinal))
            {
                return false;
            }

            for (int i = 1; i < parts.Length; i++)
            {
                int equalsIndex = parts[i].IndexOf('=');
                if (equalsIndex <= 0)
                {
                    continue;
                }

                fields[parts[i].Substring(0, equalsIndex)] = parts[i].Substring(equalsIndex + 1);
            }

            return true;
        }

        private static string ReadField(Dictionary<string, string> fields, string key)
        {
            return fields.TryGetValue(key, out string value) ? value : string.Empty;
        }

        private static float ParseFloat(Dictionary<string, string> fields, string key)
        {
            string value = ReadField(fields, key);
            if (string.Equals(value, "NaN", StringComparison.OrdinalIgnoreCase))
            {
                return float.NaN;
            }

            return float.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}
#endif
