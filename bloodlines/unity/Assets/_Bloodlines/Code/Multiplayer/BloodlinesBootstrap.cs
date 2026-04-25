using Unity.Entities;
using Unity.NetCode;
using UnityEngine.Scripting;

namespace Bloodlines.Multiplayer
{
    // Bloodlines runs in offline single-world mode by default. When NfE is installed,
    // its ClientServerBootstrap would normally take over and create client/server worlds
    // immediately. This bootstrap intercepts that flow and creates a local simulation world
    // using LocalSimulation systems only, which excludes NfE server/client systems and
    // entities.graphics Presentation systems (those trigger a Unity ECS sorter crash when
    // running with -nographics because their NfE-integration ordering dependencies reference
    // systems absent from a local-only world). Networked worlds are created on-demand.
    [Preserve]
    public class BloodlinesBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            var world = new World(defaultWorldName, WorldFlags.Game);
            World.DefaultGameObjectInjectionWorld = world;

            var systems = DefaultWorldInitialization.GetAllSystems(
                WorldSystemFilterFlags.LocalSimulation);
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
            return true;
        }
    }
}
