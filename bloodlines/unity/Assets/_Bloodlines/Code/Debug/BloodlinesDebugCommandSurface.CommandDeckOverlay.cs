using System.Globalization;
using System.Text;
using Bloodlines.HUD;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        [SerializeField] private bool showPlayerCommandDeckPanel = true;
        [SerializeField] private float playerCommandDeckPanelMinHeight = 152f;
        [SerializeField] private Color hudAlertStableColor = new(0.20f, 0.52f, 0.36f, 0.96f);
        [SerializeField] private Color hudAlertWarningColor = new(0.72f, 0.48f, 0.16f, 0.96f);
        [SerializeField] private Color hudAlertCriticalColor = new(0.62f, 0.18f, 0.16f, 0.98f);

        private void DrawPlayerCommandDeckPanel(EntityManager entityManager)
        {
            if (!showPlayerCommandDeckPanel ||
                !TryGetPlayerCommandDeckSnapshot(entityManager, out var snapshot))
            {
                return;
            }

            EnsureHudStyles();

            string title = PlayerCommandDeckOverlayPresenter.BuildTitle(in snapshot);
            string body = PlayerCommandDeckOverlayPresenter.BuildBody(in snapshot);
            float panelX = Mathf.Max(hudMargin.x, Screen.width - hudMargin.x - hudWidth);
            float bodyHeight = hudBodyStyle.CalcHeight(new GUIContent(body), hudWidth - 24f);
            float panelHeight = Mathf.Max(playerCommandDeckPanelMinHeight, bodyHeight + 56f);

            var panelRect = new Rect(panelX, hudMargin.y, hudWidth, panelHeight);
            var headerRect = new Rect(panelRect.x, panelRect.y, panelRect.width, 28f);
            var alertRect = new Rect(panelRect.x, panelRect.y + 28f, panelRect.width, 6f);
            var bodyRect = new Rect(panelRect.x + 12f, panelRect.y + 42f, panelRect.width - 24f, panelRect.height - 50f);

            var previousColor = GUI.color;
            GUI.color = hudBackground;
            GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
            GUI.color = hudHeaderBackground;
            GUI.DrawTexture(headerRect, Texture2D.whiteTexture);
            GUI.color = ResolveCommandDeckAlertColor(PlayerCommandDeckOverlayPresenter.ResolveAlertColorKey(in snapshot));
            GUI.DrawTexture(alertRect, Texture2D.whiteTexture);
            GUI.color = hudBorderColor;
            DrawRectangleBorder(panelRect, 2f);
            GUI.color = previousColor;

            GUI.Label(
                new Rect(headerRect.x + 10f, headerRect.y + 4f, headerRect.width - 20f, headerRect.height - 8f),
                title,
                hudTitleStyle);
            GUI.Label(bodyRect, body, hudBodyStyle);
        }

        public bool TryDebugGetBattlefieldCommandDeckOverlay(out string readout)
        {
            readout = string.Empty;
            if (!TryGetEntityManager(out var entityManager) ||
                !TryGetPlayerCommandDeckSnapshot(entityManager, out var snapshot))
            {
                return false;
            }

            string stageLine = PlayerCommandDeckOverlayPresenter.BuildStageLine(in snapshot);
            string alertLine = PlayerCommandDeckOverlayPresenter.BuildAlertLine(in snapshot);
            string victoryLine = PlayerCommandDeckOverlayPresenter.BuildVictoryLine(in snapshot);
            string dynastyLine = PlayerCommandDeckOverlayPresenter.BuildDynastyLine(in snapshot);
            string pressureLine = PlayerCommandDeckOverlayPresenter.BuildPressureLine(in snapshot);

            var builder = new StringBuilder(640);
            builder.Append("BattlefieldCommandDeckOverlay")
                .Append("|FactionId=").Append(snapshot.FactionId)
                .Append("|PrimaryAlertLabel=").Append(snapshot.PrimaryAlertLabel)
                .Append("|GreatReckoningActive=").Append(snapshot.GreatReckoningActive ? "true" : "false")
                .Append("|FortificationThreatActive=").Append(snapshot.FortificationThreatActive ? "true" : "false")
                .Append("|VictoryConditionId=").Append(snapshot.LeadingVictoryConditionId)
                .Append("|VictoryProgressPct=").Append(snapshot.LeadingVictoryProgressPct.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|VictoryRank=").Append(snapshot.VictoryRank)
                .Append("|StageLine=").Append(stageLine)
                .Append("|AlertLine=").Append(alertLine)
                .Append("|VictoryLine=").Append(victoryLine)
                .Append("|DynastyLine=").Append(dynastyLine)
                .Append("|PressureLine=").Append(pressureLine);

            readout = builder.ToString();
            return true;
        }

        private bool TryGetPlayerCommandDeckSnapshot(EntityManager entityManager, out PlayerCommandDeckHUDComponent snapshot)
        {
            snapshot = default;

            var controlledFactionKey = new FixedString32Bytes(controlledFactionId);
            Entity controlledFactionEntity = FindFactionRootEntity(entityManager, controlledFactionKey);
            if (controlledFactionEntity != Entity.Null &&
                entityManager.HasComponent<PlayerCommandDeckHUDComponent>(controlledFactionEntity))
            {
                snapshot = entityManager.GetComponentData<PlayerCommandDeckHUDComponent>(controlledFactionEntity);
                return true;
            }

            Entity playerEntity = FindFactionRootEntity(entityManager, new FixedString32Bytes("player"));
            if (playerEntity == Entity.Null ||
                !entityManager.HasComponent<PlayerCommandDeckHUDComponent>(playerEntity))
            {
                return false;
            }

            snapshot = entityManager.GetComponentData<PlayerCommandDeckHUDComponent>(playerEntity);
            return true;
        }

        private Color ResolveCommandDeckAlertColor(string alertKey)
        {
            return alertKey switch
            {
                "great_reckoning" => hudAlertCriticalColor,
                "fortification_threat" => hudAlertCriticalColor,
                "loyalty_crisis" => hudAlertCriticalColor,
                "victory_imminent" => hudAlertWarningColor,
                "world_pressure" => hudAlertWarningColor,
                _ => hudAlertStableColor,
            };
        }
    }
}
