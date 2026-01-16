using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AttributeNetworkWrapperV2;
using PAMultiplayer.AttributeNetworkWrapperOverrides;
using PAMultiplayer.Managers;
using UnityEngine;

namespace Cutscenes;

public static class Multiplayer
{
    private static bool? _enabled = null;
    private static readonly Dictionary<ulong, bool> PlayersWithMod = new();
    
    public static bool Enabled {
        get {
            if (_enabled == null)
            {
                _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("me.ytarame.Multiplayer");

                if (_enabled.Value)
                {
                    Init();
                }
            }
            
            return _enabled.Value;
        }
    }


    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void Init()
    {
        RuntimeHelpers.RunClassConstructor(typeof(RpcFuncRegistersGenerated).TypeHandle);
        PaMNetworkManager.OnMultiplayerStart += hosting =>
        {
            if (!hosting || PaMNetworkManager.PamInstance == null)
            {
                return;
            }

            PlayersWithMod.Clear();

            PaMNetworkManager.PamInstance.OnClientModVersionReceived += (id, guid, version) =>
            {
                if (guid != MyPluginInfo.PLUGIN_GUID)
                {
                    return;
                }
                
                if (version.Major == 0 && version.Minor == 0 && version.Build == 0)
                {
                    PlayersWithMod[id] = false;
                    return;
                }
                
                PlayersWithMod[id] = true;
            };
                
            PaMNetworkManager.PamInstance.OnPlayerJoin += id =>
            {
                PlayersWithMod[id] = false;
                PaMNetworkManager.CallRpc_Client_AskForMod(new ClientNetworkConnection(PaMNetworkManager.PamInstance.SteamIdToNetId[id], id.ToString()), MyPluginInfo.PLUGIN_GUID);
            };
            
            PaMNetworkManager.PamInstance.OnPlayerLeave += id =>
            {
                PlayersWithMod.Remove(id);
            };
        };

        PaMNetworkManager.OnMultiplayerEnd += hosting =>
        {
            PlayersWithMod.Clear();
        };
    }
    
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static bool IsMultiplayer()
    {
        return GlobalsManager.IsMultiplayer;
    }
    
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static bool IsHosting()
    {
        return GlobalsManager.IsHosting;
    }
    
    public static bool EveryoneHasMod()
    {
        return !PlayersWithMod.ContainsValue(false);
    }
}